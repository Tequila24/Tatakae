using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//[RequireComponent(typeof(Rigidbody))]
//[RequireComponent(typeof(Collider))]

namespace CharControl
{
    public enum CharState 
    {
        None = 0,
        Freefalling,
        Sliding,
        Walking,
        Jumping,
        Grappling,
        Flying
    }
    
    public struct InputState
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

        
    public class CharController : MonoBehaviour
    {
        private SurfaceController _surface;
        private Rigidbody _charBody;
        private Collider _charCollider;
    
        private InputState _inputs;

        private CharState _currentState;
        private CharState _previousState;

        private Dictionary<CharState, Motion> _charMotions;

    
        void OnValidate()
        {
            Start();
        }
        void Start()
        {
            _charBody = gameObject.GetComponent<Rigidbody>();
            _charCollider = gameObject.GetComponent<Collider>();
            _surface = new SurfaceController(_charCollider);

            _charMotions = new Dictionary<CharState, Motion>();
            _charMotions.Add(CharState.Freefalling, new FreefallMotion(_charBody, _charCollider) );
            _charMotions.Add(CharState.Walking, new WalkMotion(_charBody, _charCollider, _surface) );
            _charMotions.Add(CharState.Sliding, new SlideMotion(_charBody, _charCollider, _surface) );
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
    
        void Update()
        {
            UpdateInputs();
        }
    
        void FixedUpdate()
        {
            UpdateState();

            if ( _charMotions.ContainsKey(_currentState) ) {
                _charMotions[_currentState].UpdateInputs(_inputs);
                _charMotions[_currentState].ProcessMotion();
            }
        }


         private void UpdateState()
        {
            _previousState = _currentState;

            // GET SURFACE STATE
            _surface.Check();
            if (_surface.contactSeparation < (_charCollider.bounds.extents.y * 2.0f) ) {

                if ( Vector3.Angle(_surface.contactPointNormal, Vector3.up) < 50.0f )
                    _currentState = CharState.Walking;              // GROUNDED
                else 
                    _currentState = CharState.Sliding;              // SLIDING

            } else {
                _currentState = CharState.Freefalling;              // FALLING
            }

            if (_previousState != _currentState)
            {
                Debug.Log("State:" + _previousState + " => State:" + _currentState);

                if ( _charMotions.ContainsKey(_currentState) )
                    if (_charMotions.ContainsKey(_previousState))
                        _charMotions[_currentState].BeginMotion( _charMotions[_previousState].EndMotion() );
                    else
                        _charMotions[_currentState].BeginMotion( Vector3.zero );
            }
        }
    
    
        public Quaternion GetLookRotation()
            {
                return Quaternion.Euler(_inputs.mousePositionY, _inputs.mousePositionX, 0);
            }
    }
}