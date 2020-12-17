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

    public class CharStateController
    {
        private SurfaceController _surface = null;
        private Rigidbody _charBody = null;

        private CharState _currentState;
        private CharState _previousState;


        public CharStateController( SurfaceController newSurfaceControl, Rigidbody newCharBody)
        {
            _surface = newSurfaceControl;

            _charBody = newCharBody;
        }

        public void UpdateState()
        {
            _previousState = _currentState;

            // GET SURFACE STATE
            _surface.Check();
            if (_surface.contactSeparation < 0.5f) {
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
                    //_charBody.isKinematic = false;
                    break;
                }
                case CharState.Walking: {
                    //_charBody.isKinematic = true;
                    break;
                }
                default:
                    break;
            }
        }
        


        public CharState GetCurrentState()
        {
            return _currentState;
        }


        public CharState GetPreviousState()
        {
            return _previousState;
        }
    }
}