using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NiftyClub.Controllers
{
	public class PlayerController : MonoBehaviour
	{
		[Tooltip("Rotation speed multiplier"), Range(2.0f, 12.0f), SerializeField]
		private float rotateSpeed = 4.0f;

		[Header("Look Around Limits")]
		[Tooltip("Max vertical viewing angle, both seated and standing")]
		[Range(0.0f, 90.0f)]
		[SerializeField] private float maxVerticalViewAngle = 60.0f;
		
		private Vector2 _rotation;
		private Vector3 _characterVelocity;
		private Vector2 _look;
		private Vector2 _move;
		
		private bool _isJumping = false;
		private bool _jump = false;
		
		[BoxGroup ("Links"), SerializeField] private Transform _transform;
		[BoxGroup ("Links"), SerializeField] private CharacterController _controller;

		#region Unity Methods
		
		void Awake ()
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
			
			// play sounds locally
			makeFootstepSound += PlayFootstepSound;
			makeJumpSound += PlayJumpSound;
		}
		
		void Update()
		{
			Look(_look);
		}

		#endregion
		
		private bool ShouldProcessInput()
		{
			// the cursor is unlocked when GUI is on
			return Cursor.lockState == CursorLockMode.Locked;
		}
		
		public void OnMove(InputAction.CallbackContext context)
		{
			_move = !ShouldProcessInput() ? Vector2.zero : context.ReadValue<Vector2>();
		}

		public void OnLook(InputAction.CallbackContext context)
		{
			_look = ShouldProcessInput() ? context.ReadValue<Vector2>() : Vector2.zero;
		}

		public void OnJump(InputAction.CallbackContext context)
		{
			if (context.performed && !_isJumping && ShouldProcessInput()) _jump = true;
		}
		
		private float _lastTimeJumped = 0f;
		private bool isGrounded = false;
		private const float k_JumpGroundingPreventionTime = 0.2f;
		private const float k_GroundCheckDistance = 0.2f;
		
		void GroundCheck() {
			isGrounded = false;
			// if we're grounded, collect info about the ground normal with a downward capsule cast representing our character capsule
			if (Time.time >= _lastTimeJumped + k_JumpGroundingPreventionTime) {
				if (Physics.CapsuleCast(GetCapsuleBottomHemisphere(),
					GetCapsuleTopHemisphere(_controller.height),
					_controller.radius, Vector3.down, out RaycastHit hit, k_GroundCheckDistance)) {
					// Only consider this a valid ground hit if the ground normal goes in the same direction as the character up
					// and if the slope angle is lower than the character controller's limit
					if (Vector3.Dot(hit.normal, _transform.up) > 0f &&
					    IsNormalUnderSlopeLimit(hit.normal)) {
						isGrounded = true;
						_isJumping = false;

						// handle snapping to the ground
						if (hit.distance > _controller.skinWidth) {
							_controller.Move(Vector3.down * hit.distance);
						}
					}
				}
			}
		}

		private void Move (Vector2 direction)
		{
			
		}
		
		private void Look(Vector2 rotate) {
			float scaledRotateSpeed = rotateSpeed * Time.deltaTime;

			_rotation.x += rotate.x * scaledRotateSpeed;
			if (false) { // _sitting || _isSittingDown) { //rotate just the camera, not the character
				/* _rotation.x = Mathf.Clamp(_rotation.x, -maxSideViewAngle, maxSideViewAngle);
				_camera.localEulerAngles = new Vector3(_camera.localEulerAngles.x, _rotation.x, 0.0f); */
			}
			else //rotate whole character (including camera)
				_transform.localEulerAngles = new Vector3(0.0f, _rotation.x, 0.0f);

			_rotation.y -= rotate.y * scaledRotateSpeed;
			_rotation.y = Mathf.Clamp(_rotation.y, -maxVerticalViewAngle, maxVerticalViewAngle);
			// _camera.localEulerAngles = new Vector3(_rotation.y, _camera.localEulerAngles.y, 0.0f);
		}
		
		// Returns true if the slope angle represented by the given normal is under the slope angle limit of the character controller
		bool IsNormalUnderSlopeLimit(Vector3 normal) {
			return Vector3.Angle(_transform.up, normal) <= _controller.slopeLimit;
		}

		Vector3 GetCapsuleBottomHemisphere() {
			return _transform.position + (_transform.up * _controller.radius);
		}

		Vector3 GetCapsuleTopHemisphere(float atHeight) {
			return _transform.position + (_transform.up * (atHeight - _controller.radius));
		}

		#region Local Sounds

		// use delegates for playing sounds over network for other players
		private delegate void MakeSound();

		private MakeSound makeFootstepSound;
		private MakeSound makeJumpSound;
		
		private void PlayFootstepSound() {
			// audioSource.PlayOneShot(footstepSFX, footstepSFXVolume);
		}

		private void PlayJumpSound() {
			// audioSource.PlayOneShot(jumpSFX, jumpSFXVolume);
		}

		#endregion
	}
}