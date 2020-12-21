using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharControl
{
    

    public class CharMoveController
    {
        private SurfaceController _surface = null;
        private Rigidbody _charBody = null;
        private Collider _charCollider = null;

        private CharState _currentState;
        private CharState _previousState;

        private Vector3 _frameVelocity;

        


        public CharMoveController( SurfaceController newSurfaceControl, Rigidbody newCharBody, Collider newCharCollider)
        {
            _surface = newSurfaceControl;
            _charBody = newCharBody;
            _charCollider = newCharCollider;
        }


        public void Process()
        {
        }


        

        private void UpdateState()
        {
            _previousState = _currentState;

            // GET SURFACE STATE
            _surface.Check();
            if (_surface.contactSeparation < (_charCollider.bounds.extents.y + 0.2f) ) {

                if ( Vector3.Angle(_surface.contactPointNormal, Vector3.up) < 30 )
                    _currentState = CharState.Walking;              // GROUNDED
                else 
                    _currentState = CharState.Sliding;              // SLIDING

            } else {
                _currentState = CharState.Freefalling;              // FALLING
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
                case CharState.Sliding: {
                    _charBody.useGravity = true;
                    _charBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                    break;
                }
                default:
                    break;
            }
        }        
        
    }
}