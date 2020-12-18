using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharControl
{
    public enum CharState 
    {
        None = 0,
        Freefalling,
        Slidefall,
        Walking,
        Grappling,
        Flying
    }

    public class CharMoveController
    {
        private SurfaceController _surface = null;
        private Rigidbody _charBody = null;
        private Collider _charCollider = null;
        private InputMaster _input

        private CharState _currentState;
        private CharState _previousState;


        public CharMoveController( SurfaceController newSurfaceControl, Rigidbody newCharBody, Collider newCharCollider)
        {
            _surface = newSurfaceControl;
            _charBody = newCharBody;
            _charCollider = newCharCollider;
        }

        private void UpdateState()
        {
            _previousState = _currentState;

            // GET SURFACE STATE
            _surface.Check();
            if (_surface.contactSeparation < _charCollider.bounds.size.y) {
                _currentState = CharState.Walking;
            } else {
                _currentState = CharState.Freefalling;
            }

            if (_previousState != _currentState)
            {
                SwitchState();
            }
        }


        void SwitchState()
        {

            Debug.Log("State:" + _previousState + " => State:" + _currentState);
            switch (_currentState)
            {
                case CharState.Freefalling: {
                    _charBody.useGravity = true;
                    _charBody.constraints = 0;
                    break;
                }
                case CharState.Walking: {
                    _charBody.useGravity = false;
                    _charBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                    break;
                }
                default:
                    break;
            }
        }
        
        public void Process()
        {
            UpdateState();

            switch (_currentState)
            {
                case CharState.Freefalling:
                    Fall();
                    break;
                case CharState.Walking:
                    Walk();
                    break;
                default:
                    break;
            }
        }


        void Fall()
        {
        }


        void Walk()
        {
            Quaternion lookRotation = Quaternion.Euler(lookAngles.x, lookAngles.y, 0);
            Vector3 heightAdjust = new Vector3(0, _surface.contactSeparation - 1.3f, 0) * 0.1f;
            Vector3 step = _surface.rotationToNormal * charYaw * stepInput * 0.075f;

            // APPLY VELOCITIES
            Vector3 frameVelocity = step;
            _charBody.MovePosition(this.transform.position + heightAdjust + frameVelocity);
            _charBody.velocity = Vector3.zero;

            // APPLY ROTATION
            Quaternion rotationAdjust = Quaternion.Lerp(transform.rotation, charYaw * Quaternion.identity, 0.2f);
            _charBody.MoveRotation(rotationAdjust);
        }
        }
}