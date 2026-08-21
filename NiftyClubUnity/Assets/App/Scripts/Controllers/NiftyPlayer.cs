using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace NiftyClub.Controllers
{
	public class NiftyPlayer : MonoBehaviour
	{
		[Header ("Parameters"), SerializeField] private float moveLerpSpeed = 10f;

		[Header ("Links"), SerializeField] private Transform targetTransform;
		[Header ("Links"), SerializeField] private PlayerController playerController;
		[Header ("Links"), SerializeField] private TextMeshProUGUI nameText;
		[Header ("Links"), SerializeField] private CharacterAnimator characterAnimator;
		
		[Header ("Links"), SerializeField] private Transform followAimTransform;
		public Transform FollowAimTransform => followAimTransform;

		private static NiftyPlayer _local;
		public static NiftyPlayer Local => _local;

		private ushort _id;
		public ushort ID => _id;

		private string _nickname;
		public string Nickname => _nickname;
		
		private bool _isLocal;
		public bool IsLocal => _isLocal;

		private Vector3 oldPosition;
		private Vector3 newPosition;

		private bool isInitialized;

		public event EventHandler<bool> OnInitialized;
		
		private float time;

		public Vector3 CharacterVelocity => playerController != null ? playerController.CharacterVelocity.normalized : GetVelocityVector ();
		public Vector3 Velocity => playerController != null ? playerController.Velocity : GetVelocityVector ();
		public bool OnGround => playerController != null ? playerController.OnGround : true;

		#region Unity Methods
		
		void Update ()
		{
			if (!isInitialized || _isLocal)
				return;

			targetTransform.position = Vector3.Lerp (targetTransform.position, newPosition, Time.deltaTime * moveLerpSpeed);
		}

		#endregion

		private Vector3 GetVelocityVector ()
		{
			Vector3 deltaVector = newPosition - targetTransform.position;
			if (deltaVector.magnitude > moveLerpSpeed)
			{
				deltaVector = deltaVector.normalized * moveLerpSpeed;
			}

			return deltaVector;
		}
		
		public void Initialize (
			Vector3 position,
			ushort id,
			string nickname,
			bool isLocal,
			byte characterIndex)
		{
			newPosition = targetTransform.localPosition = position;

			_id = id;
			_nickname = nickname;
			_isLocal = isLocal;
			
			if (_isLocal)
			{
				_local = this;
			}

			nameText.text = nickname;

			isInitialized = true;

			OnInitialized?.Invoke (this, _isLocal);

			SetPositionAsync (position);
			
			characterAnimator.SetCharacter (characterIndex);
		}

		private async Task SetPositionAsync (Vector3 position)
		{
			await Task.Yield ();

			newPosition = targetTransform.localPosition = position;
		}

		public void SetMovePosition (Vector3 position)
		{
			time = Time.time;
			
			oldPosition = newPosition;
			newPosition = position;
		}
	}
}
