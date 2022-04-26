using System;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace NiftyClub.Controllers
{
	public class NiftyPlayer : MonoBehaviour
	{
		[BoxGroup ("Parameters"), SerializeField] private float moveLerpSpeed = 10f;
		[BoxGroup ("Parameters"), SerializeField] private float rotateLerpSpeed = 50f;

		[BoxGroup ("Links"), SerializeField] private Transform targetTransform;
		[BoxGroup ("Links"), SerializeField] private TextMeshProUGUI nameText;
		[BoxGroup ("Links"), SerializeField] private CharacterAnimator characterAnimator;

		private ushort _id;
		public ushort ID => _id;

		private string _nickname;
		public string Nickname => _nickname;
		
		private bool _isLocal;
		public bool IsLocal => _isLocal;

		private Vector3 oldPosition;
		private Vector3 newPosition;
		private Quaternion newRotation;

		private bool isInitialized;

		public event EventHandler<bool> OnInitialized;
		
		private float time;
		
		#region Unity Methods

		void Update ()
		{
			if (!isInitialized || _isLocal)
				return;

			targetTransform.position = Vector3.Lerp (targetTransform.position, newPosition, Time.deltaTime * moveLerpSpeed);

			// TODO: Use Quaternion math instead
			Quaternion oldRotation = targetTransform.rotation;
			targetTransform.rotation = new Quaternion (
				Mathf.LerpAngle (oldRotation.x, newRotation.x, Time.deltaTime * rotateLerpSpeed),
				Mathf.LerpAngle (oldRotation.y, newRotation.y, Time.deltaTime * rotateLerpSpeed),
				Mathf.LerpAngle (oldRotation.z, newRotation.z, Time.deltaTime * rotateLerpSpeed),
				Mathf.LerpAngle (oldRotation.w, newRotation.w, Time.deltaTime * rotateLerpSpeed)
			);
		}

		#endregion
		
		public void Initialize (
			Vector3 position,
			Quaternion rotation,
			ushort id,
			string nickname,
			bool isLocal,
			byte characterIndex)
		{
			newPosition = targetTransform.localPosition = position;
			newRotation = targetTransform.rotation = new Quaternion (rotation.x, rotation.y, rotation.z, rotation.w);

			_id = id;
			_nickname = nickname;
			_isLocal = isLocal;

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

		public void SetMovePosition (Vector3 position, Quaternion rotation)
		{
			time = Time.time;
			oldPosition = newPosition;
			newPosition = position;
			newRotation = rotation;
		}

		public Vector3 GetPosition ()
		{
			return targetTransform.position;
		}
	}
}