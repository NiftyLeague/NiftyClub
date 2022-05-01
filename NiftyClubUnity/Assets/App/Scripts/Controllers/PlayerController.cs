using System;
using DynamicBox.EventManagement;
using NiftyClub.Domain;
using NiftyClub.GameEvents;
using NiftyClubPlugins.Common.Enums;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NiftyClub.Controllers
{
	public class PlayerController : MonoBehaviour
	{
		[SerializeField] private NiftyPlayer niftyPlayer;
		[SerializeField] private Transform groundRaycastPoint;
		
		[Tooltip ("Rotation speed multiplier"), Range (2.0f, 12.0f), SerializeField]
		private float rotateSpeed = 4.0f;

		[Header ("Ground Movement")]
		[Tooltip ("Max base speed when grounded (before speed Multiplier)")]
		[SerializeField] private float maxSpeedOnGround = 4.0f;

		[Tooltip (
			"Sharpness for the movement when grounded, a low value will make the player accelerate and decelerate slowly, a high value will do the opposite")]
		public float movementSharpnessOnGround = 1f;

		[Header ("Look Around Limits")]
		[Tooltip ("Max vertical viewing angle, both seated and standing")]
		[Range (0.0f, 90.0f)]
		[SerializeField] private float maxVerticalViewAngle = 60.0f;

		[Header ("Air Movement")]
		[Tooltip ("Acceleration speed when in the air")]
		public float accelerationSpeedInAir = 2f;

		[Tooltip ("Max movement speed when not grounded")]
		public float maxSpeedInAir = 4f;

		[Header ("Jump Height")]
		[Tooltip ("Height reached at end of jump")]
		[Range (0.2f, 20.0f)]
		[SerializeField] private float jumpHeight = 1.0f;

		[Tooltip ("Higher values are unrealistic, but feel more 'gamey'")]
		[Range (1.0f, 50.0f)]
		[SerializeField] private float gravityValue = 16.0f;

		[BoxGroup ("Links"), SerializeField] private Transform _transform;
		[BoxGroup ("Links"), SerializeField] private CharacterController _controller;

		private Vector2 _rotation;
		private Vector3 _characterVelocity;
		private Vector2 _look;
		private Vector2 _move;

		private bool _isJumping = false;
		private bool _jump = false;

		public bool OnGround => isGrounded;

		public Vector2 CharacterVelocity => new Vector2 (_characterVelocity.x, _characterVelocity.z);
		public Vector3 Velocity
		{
			get => _move;
			set => _move = value;
		}

		private ChatBoxModel _chatBoxModel;

		#region Unity Methods

		void Awake ()
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}

		private void OnEnable ()
		{
			EventManager.Instance.AddListener<ChatBoxSetupEvent> (ChatBoxSetupHandler);
		}

		private void OnDisable ()
		{
			EventManager.Instance.RemoveListener<ChatBoxSetupEvent> (ChatBoxSetupHandler);
		}

		void Update ()
		{
			if (!niftyPlayer.IsLocal)
				return;
			
			Look (_look);

			GroundCheck ();
			Move (_move);
		}

		#endregion

		#region Event Handlers

		private void ChatBoxSetupHandler (ChatBoxSetupEvent eventDetails)
		{
			_chatBoxModel = eventDetails.ChatBox;
		}

		#endregion

		private bool ShouldProcessInput ()
		{
			// the cursor is unlocked when GUI is on
			return Cursor.lockState == CursorLockMode.Locked;
		}

		public void OnMove (InputAction.CallbackContext context)
		{
			if (_chatBoxModel is { ChatMode: ChatMode.Input })
				return;
			
			_move = !ShouldProcessInput () ? Vector2.zero : context.ReadValue<Vector2> ();
		}

		public void OnLook (InputAction.CallbackContext context)
		{
			if (_chatBoxModel is { ChatMode: ChatMode.Input })
				return;

			_look = ShouldProcessInput () ? context.ReadValue<Vector2> () : Vector2.zero;
		}

		public void OnJump (InputAction.CallbackContext context)
		{
			if (_chatBoxModel is { ChatMode: ChatMode.Input })
				return;

			if (context.performed && !_isJumping && ShouldProcessInput ())
			{
				_jump = true;
			}
		}

		private float _lastTimeJumped = 0f;
		private bool isGrounded;
		private const float k_JumpGroundingPreventionTime = 0.2f;
		private const float k_GroundCheckDistance = 0.2f;

		void GroundCheck ()
		{
			isGrounded = false;
			// if we're grounded, collect info about the ground normal with a downward capsule cast representing our character capsule
			if (Time.time >= _lastTimeJumped + k_JumpGroundingPreventionTime)
			{
				/* if (Physics.CapsuleCast (GetCapsuleBottomHemisphere (),
					GetCapsuleTopHemisphere (_controller.height),
					_controller.radius, Vector3.down, out RaycastHit hit, k_GroundCheckDistance)) */
				var hits = Physics.RaycastAll (groundRaycastPoint.position, Vector3.down, 0.1f);
				if (hits != null && hits.Length > 0)
				{
					var hit = hits[0];
					
					// Only consider this a valid ground hit if the ground normal goes in the same direction as the character up
					// and if the slope angle is lower than the character controller's limit
					if (Vector3.Dot (hit.normal, _transform.up) > 0f &&
					    IsNormalUnderSlopeLimit (hit.normal))
					{
						isGrounded = true;
						_isJumping = false;

						// handle snapping to the ground
						if (hit.distance > _controller.skinWidth)
						{
							_controller.Move (Vector3.down * hit.distance);
						}
					}
				}
			}
		}

		private void Move (Vector2 direction)
		{
			float speedModifier = 1f; // _running ? runSpeedMultiplier : 1f;
			// Rotate direction according to world Y rotation of player.
			Vector3 moveDirection = Quaternion.Euler (0f, _transform.localEulerAngles.y, 0f) *
			                        new Vector3 (direction.x, 0f, direction.y);
			Vector3 movement = moveDirection;

			if (isGrounded)
			{
				if (_jump)
				{
					_lastTimeJumped = Time.time;
					// playerSync.JumpSync ();
					float _yVelocity = Mathf.Sqrt (2.0f * jumpHeight * gravityValue);
					_characterVelocity = new Vector3 (_characterVelocity.x, _yVelocity, _characterVelocity.z);
					_isJumping = true;
					_jump = false;
				}
			}
			else
			{
				// add horizontal air acceleration
				if (!isGrounded)
					_characterVelocity += movement * (accelerationSpeedInAir * Time.deltaTime);

				// apply the gravity to the velocity
				_characterVelocity.y -= gravityValue * Time.deltaTime;

				// limit air speed to a maximum, but only horizontally
				float verticalVelocity = _characterVelocity.y;
				Vector3 horizontalVelocity = Vector3.ProjectOnPlane (_characterVelocity, Vector3.up);
				horizontalVelocity = Vector3.ClampMagnitude (horizontalVelocity, maxSpeedInAir * speedModifier);
				_characterVelocity = horizontalVelocity + (Vector3.up * verticalVelocity);
			}

			Vector3 targetVelocity = movement * (maxSpeedOnGround * speedModifier);
			_characterVelocity = Vector3.Lerp (_characterVelocity, targetVelocity,
				movementSharpnessOnGround * Time.deltaTime);

			_controller.Move (_characterVelocity * Time.deltaTime);
		}

		private void Look (Vector2 rotate)
		{
			float scaledRotateSpeed = rotateSpeed * Time.deltaTime;

			_rotation.x += rotate.x * scaledRotateSpeed;
			//rotate whole character (including camera)
			_transform.localEulerAngles = new Vector3 (0.0f, _rotation.x, 0.0f);

			_rotation.y -= rotate.y * scaledRotateSpeed;
			_rotation.y = Mathf.Clamp (_rotation.y, -maxVerticalViewAngle, maxVerticalViewAngle);
		}

		// Returns true if the slope angle represented by the given normal is under the slope angle limit of the character controller
		bool IsNormalUnderSlopeLimit (Vector3 normal)
		{
			return Vector3.Angle (_transform.up, normal) <= _controller.slopeLimit;
		}
	}
}