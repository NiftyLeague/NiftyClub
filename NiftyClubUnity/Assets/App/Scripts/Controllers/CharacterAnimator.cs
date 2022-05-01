using System;
using System.Collections.Generic;
using System.Linq;
using NiftyClubPlugins.Common.Enums;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace NiftyClub.Controllers
{
	public class CharacterAnimator : MonoBehaviour
	{
		[BoxGroup ("Links"), SerializeField] private SpriteRenderer rend;
		[BoxGroup ("Links"), SerializeField] private NiftyPlayer niftyPlayer;

		[BoxGroup ("Dynamic Load"), SerializeField]
		private bool isDynamicallyLoaded;

		[BoxGroup ("Dynamic Load"), SerializeField]
		private int loadedSheetIndex;

		[BoxGroup ("Dynamic Load"), SerializeField]
		private AssetReference[] spriteSheetRefs;

		[BoxGroup ("Dynamic Load"), SerializeField]
		private Sprite[] spriteSheet;

		[BoxGroup ("Sprites"), SerializeField] private Sprite[] idleRight;
		[BoxGroup ("Sprites"), SerializeField] private Sprite[] idleUp;
		[BoxGroup ("Sprites"), SerializeField] private Sprite[] idleLeft;
		[BoxGroup ("Sprites"), SerializeField] private Sprite[] idleDown;
		[BoxGroup ("Sprites"), SerializeField] private Sprite[] runRight;
		[BoxGroup ("Sprites"), SerializeField] private Sprite[] runUp;
		[BoxGroup ("Sprites"), SerializeField] private Sprite[] runLeft;
		[BoxGroup ("Sprites"), SerializeField] private Sprite[] runDown;

		private int frame;
		float frameCounter;

		float transitionTime;

		private float t;

		private AnimState animState;
		private CharacterState state;

		private RunDirection lastRunDirection = RunDirection.Down;

		#region Unity Methods

		void Start ()
		{
			if (!isDynamicallyLoaded)
				return;

			SetCharacter (loadedSheetIndex);
		}

		void Update ()
		{
			t = Time.deltaTime;
		}

		void LateUpdate ()
		{
			var newAnimState = DetermineAnimState ();

			if (animState != newAnimState)
			{
				frame = 0;
				frameCounter = 0f;
			}

			animState = newAnimState;

			switch (animState)
			{
				case AnimState.Idle:
					AnimateIdle ();

					break;
				case AnimState.Running:
					AnimateRun (GetRunDirection (niftyPlayer.Velocity));

					break;
			}
		}

		#endregion

		private int ParseSpriteIndex (string spriteName)
		{
			int index = spriteName.IndexOf ("_", StringComparison.Ordinal);
			string numberString = spriteName.Substring (index + 1);
			int.TryParse (numberString, out int number);

			return number;
		}
		
		public void SetCharacter (int characterIndex)
		{
			AssetReference spriteSheetRef = spriteSheetRefs[characterIndex];
			Addressables.LoadAssetAsync<Sprite[]> (spriteSheetRef).Completed += handle =>
			{
				spriteSheet = handle.Result;
				
				List<Sprite> spriteList = spriteSheet.ToList ();
				spriteList.Sort ((c1, c2) =>
				{
					int number1 = ParseSpriteIndex (c1.name);
					int number2 = ParseSpriteIndex (c2.name);
					
					return number1.CompareTo (number2);
				});
				spriteSheet = spriteList.ToArray ();

				idleRight = new[] { spriteSheet[32] };
				idleUp = new[] { spriteSheet[33] };
				idleLeft = new[] { spriteSheet[34] };
				idleDown = new[] { spriteSheet[35] };

				List<Sprite> stateSprites = new List<Sprite> ();
				for (int i = 0; i < 8; i++)
				{
					stateSprites.Add (spriteSheet[i]);
				}

				runRight = stateSprites.ToArray ();

				stateSprites = new List<Sprite> ();
				for (int i = 8; i < 16; i++)
				{
					stateSprites.Add (spriteSheet[i]);
				}

				runUp = stateSprites.ToArray ();

				stateSprites = new List<Sprite> ();
				for (int i = 16; i < 24; i++)
				{
					stateSprites.Add (spriteSheet[i]);
				}

				runLeft = stateSprites.ToArray ();

				stateSprites = new List<Sprite> ();
				for (int i = 24; i < 32; i++)
				{
					stateSprites.Add (spriteSheet[i]);
				}

				runDown = stateSprites.ToArray ();

				AnimateIdle ();
			};
		}

		private void AnimateIdle ()
		{
			switch (lastRunDirection)
			{
				case RunDirection.Right:
					rend.sprite = idleRight[0];

					break;
				case RunDirection.Up:
					rend.sprite = idleUp[0];

					break;
				case RunDirection.Left:
					rend.sprite = idleLeft[0];

					break;
				case RunDirection.Down:
					rend.sprite = idleDown[0];

					break;
				default:
					throw new ArgumentOutOfRangeException ();
			}
		}

		private void AnimateRun (Sprite[] runSprites)
		{
			int frameBefore = frame;
			RunAnimation (runSprites, 0.04f);
			if (frame != frameBefore && frame % 2 == 1)
			{
				// FreeLives.SoundController.PlaySoundEffect("Footstep", 0.1f, transform.position);
				// EffectsController.CreateDustPuff(transform.position, character.FacingDirection);
			}
		}

		private Sprite[] GetRunDirection (Vector2 moveDirection)
		{
			if (Mathf.Abs (moveDirection.x) > Mathf.Abs (moveDirection.y))
			{
				if (moveDirection.x > 0)
				{
					lastRunDirection = RunDirection.Right;

					return runRight;
				}
				else
				{
					lastRunDirection = RunDirection.Left;

					return runLeft;
				}
			}
			else
			{
				if (moveDirection.y > 0)
				{
					lastRunDirection = RunDirection.Up;

					return runUp;
				}
				else
				{
					lastRunDirection = RunDirection.Down;

					return runDown;
				}
			}
		}

		private void RunAnimation (Sprite[] frames, float frameDelay, bool clamp = false,
			bool ignoreCharacterTimescale = false)
		{
			if (ignoreCharacterTimescale)
				frameCounter += Time.deltaTime;
			else
				frameCounter += t;

			if (frameCounter > frameDelay || frame < 0)
			{
				frame++;
				frameCounter -= frameDelay;
			}

			if (clamp)
				rend.sprite = frames[Mathf.Clamp (frame, 0, frames.Length - 1)];
			else
				rend.sprite = frames[frame % frames.Length];
		}

		private AnimState DetermineAnimState ()
		{
			// if (character.OnGround && Mathf.Abs(character.Velocity.magnitude) > 0f)
			if (Mathf.Abs (niftyPlayer.Velocity.magnitude) > 0.01f)
			{
				return AnimState.Running;
			}
			else if (!niftyPlayer.OnGround)
			{
				return AnimState.Jumping;
			}

			return AnimState.Idle;
		}
	}
}