using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NiftyClub
{
    public class MovementHelper : MonoBehaviour
    {
        [SerializeField] private float jumpHeight = 3.0f;
        [SerializeField] private float gravityValue = 50.0f;
        
        private float _lastTimeJumped;
        
        /* public Vector3 GetMovementVector (
            Transform _transform,
            Vector2 direction,
            bool _jump)
        {
            float speedModifier = 1f;
            Vector3 moveDirection = Quaternion.Euler (
                                        0f,
                                        _transform.localEulerAngles.y, 0f) * new Vector3 (direction.x, 0f, direction.y);
            Vector3 movement = moveDirection;

            if (isGrounded)
            {
                
            }
            if (_jump)
            {
                _lastTimeJumped = Time.time;
                
                float _yVelocity = Mathf.Sqrt (2.0f * jumpHeight * gravityValue);
                _characterVelocity = new Vector3 (_characterVelocity.x, _yVelocity, _characterVelocity.z);
                _isJumping = true;
                _jump = false;
            }
        } */
    }
}
