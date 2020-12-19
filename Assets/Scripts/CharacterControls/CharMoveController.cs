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

        private CharState _currentState;
        private CharState _previousState;

        private Vector3 _frameVelocity;

        private struct InputState
        {
            public int forward;
            public int backward;
            public int left;
            public int right;
            public int jump;

            public float mouseDeltaX;
            public float mouseDeltaY;

            public float mousePositionX;
            public float mousePositionY;
        };
        InputState _inputs;


        public CharMoveController( SurfaceController newSurfaceControl, Rigidbody newCharBody, Collider newCharCollider)
        {
            _surface = newSurfaceControl;
            _charBody = newCharBody;
            _charCollider = newCharCollider;
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


        public void UpdateInputs()
        {
            _inputs.forward = Input.GetKey(KeyCode.W) ? 1 : 0;
            _inputs.backward = Input.GetKey(KeyCode.S) ? 1 : 0;
            _inputs.left = Input.GetKey(KeyCode.A) ? 1 : 0;
            _inputs.right = Input.GetKey(KeyCode.D) ? 1 : 0;
            _inputs.jump = Input.GetKeyUp(KeyCode.Space) ? 1 : 0;

            _inputs.mouseDeltaX = Input.GetAxis("Mouse X");
            _inputs.mouseDeltaY = -Input.GetAxis("Mouse Y");

            _inputs.mousePositionX += _inputs.mouseDeltaX;
            _inputs.mousePositionY += _inputs.mouseDeltaY;
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
        

        void Fall()
        {
            _frameVelocity = Vector3.Lerp(_frameVelocity, Vector3.zero, 0.001f);
            _charBody.MovePosition(_charBody.transform.position + _frameVelocity);
        }


        void Walk()
        {
            Vector3 heightAdjust = new Vector3(0, _surface.contactSeparation - 1.3f, 0) * 0.4f;
            Vector3 step =  _surface.rotationToNormal *                                                         //rotation to surface
                            Quaternion.Euler(0, _inputs.mousePositionX, 0) *                                    // rotation to horizontal lookDirection
                            new Vector3(_inputs.right - _inputs.left, 0 , _inputs.forward - _inputs.backward) * // step 
                            0.1f ;                                                                              // walk speed


            // APPLY VELOCITIES
            _frameVelocity = Vector3.Lerp(_frameVelocity, step, 0.2f);
            //_charBody.MovePosition(_charBody.transform.position - heightAdjust + _frameVelocity);
            _charBody.velocity = Vector3.zero;


            // APPLY ROTATION
            _charBody.transform.rotation = Quaternion.Euler(0, _inputs.mousePositionX, 0);
        }

        
        public Quaternion GetLookRotation()
        {
            return Quaternion.Euler(_inputs.mousePositionY, _inputs.mousePositionX, 0);
        }
    }
}