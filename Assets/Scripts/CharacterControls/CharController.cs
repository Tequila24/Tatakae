using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
        public int shift;

        public int mouse1Held;
        public int mouse2Held;

        public float mouseDeltaX;
        public float mouseDeltaY;

        public float mousePositionX;
        public float mousePositionY;
    };


    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]

    public class CharController : MonoBehaviour
    {
        private Rigidbody _charBody;
        private Collider _charCollider;

        private InputState _inputs;

        [SerializeField]
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
            _charBody.isKinematic = true;
            _charCollider = gameObject.GetComponent<Collider>();

            _charMotions = new Dictionary<CharState, Motion>();
            _charMotions.Add(CharState.Freefalling, new FreefallMotion(_charBody));
            _charMotions.Add(CharState.Walking, new WalkMotion(_charBody, _charCollider));
            _charMotions.Add(CharState.Grappling, new GrappleMotion(_charBody));
        }

        void Update()
        {
            Cursor.lockState = CursorLockMode.Locked;
            UpdateInputs();
        }


        public void UpdateInputs()
        {
            _inputs.forward = Input.GetKey(KeyCode.W) ? 1 : 0;
            _inputs.backward = Input.GetKey(KeyCode.S) ? 1 : 0;
            _inputs.left = Input.GetKey(KeyCode.A) ? 1 : 0;
            _inputs.right = Input.GetKey(KeyCode.D) ? 1 : 0;
            _inputs.jump = Input.GetKeyUp(KeyCode.Space) ? 1 : 0;
            _inputs.shift = Input.GetKey(KeyCode.LeftShift) ? 1 : 0;

            _inputs.mouse1Held = Input.GetKey(KeyCode.Mouse0) ? 1 : 0;
            _inputs.mouse2Held = Input.GetKey(KeyCode.Mouse1) ? 1 : 0;

            _inputs.mouseDeltaX = Input.GetAxis("Mouse X");
            _inputs.mouseDeltaY = -Input.GetAxis("Mouse Y");

            _inputs.mousePositionX += _inputs.mouseDeltaX;
            _inputs.mousePositionY = Mathf.Clamp(_inputs.mousePositionY + _inputs.mouseDeltaY, -90, 90);

            UpdateState();
        }


        void FixedUpdate()
        {
            if (_charMotions.ContainsKey(_currentState))
            {
                _charMotions[_currentState].UpdateInputs(_inputs);
                _charMotions[_currentState].ProcessMotion();
            }
        }


        private void UpdateState()
        {
            _previousState = _currentState;

            if (Input.GetKeyDown(KeyCode.Mouse0) && Input.GetKey(KeyCode.Mouse0)) 
            {
                if ( (_charMotions[CharState.Grappling] as GrappleMotion).TryGrapple(Quaternion.Euler(_inputs.mousePositionY, _inputs.mousePositionX, 0)) )
                {
                    _currentState = CharState.Grappling; 
                }
            } else {
                if ( (_currentState != CharState.Grappling) || (!Input.GetKey(KeyCode.Mouse0)) )
                if (Physics.Raycast(_charBody.transform.position, Physics.gravity, _charCollider.bounds.size.y * 0.8f))
                {
                    _currentState = CharState.Walking;              // GROUNDED
                }
                else
                {
                    _currentState = CharState.Freefalling;          // FALLING
                }
            }

            

            if (_previousState != _currentState)
            {
                //Debug.Log(_previousState + " => " + _currentState);

                if (_charMotions.ContainsKey(_currentState))
                    if (_charMotions.ContainsKey(_previousState))   
                    {
                        _charMotions[_currentState].BeginMotion(_charMotions[_previousState].GetVelocity());
                    }
                    else
                    {
                        _charMotions[_currentState].BeginMotion(Vector3.zero);
                    }
            }

        }


        public Quaternion GetLookRotation()
        {
            return Quaternion.Euler(_inputs.mousePositionY, _inputs.mousePositionX, 0);
        }

        public void OnCollisionStay()
        {
            /*Vector3 depenetrationDirection;
                        float depenetrationDistance;
                        Physics.ComputePenetration( _charCollider, _charBody.transform.position + _sumVelocity, _charBody.transform.rotation,
                                                    hit.collider, hit.collider.transform.position, hit.collider.transform.rotation,
                                                    out depenetrationDirection, out depenetrationDistance);*/
        }
    }
}