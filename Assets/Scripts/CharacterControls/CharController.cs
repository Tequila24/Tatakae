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

        private Dictionary<CharState, Motion> _charMotions = new Dictionary<CharState, Motion>();

        public Quaternion lookRotation;
        public Vector3 lookPoint;

        void OnValidate()
        {
            Init();
        }

        void Start()
        {
            Init();
        }

        void Init()
        {
            if (_charBody == null)
                _charBody = gameObject.GetComponent<Rigidbody>();
            _charBody.isKinematic = true;

            if (_charCollider == null) 
                _charCollider = gameObject.GetComponent<Collider>();

            if (!_charMotions.ContainsKey(CharState.Freefalling))
                _charMotions.Add(CharState.Freefalling, FreefallMotion.Create(this.gameObject, _charBody, _charCollider));

            if (!_charMotions.ContainsKey(CharState.Walking))
                _charMotions.Add(CharState.Walking, WalkMotion.Create(this.gameObject, _charBody, _charCollider));
            
            if (!_charMotions.ContainsKey(CharState.Grappling))
            _charMotions.Add(CharState.Grappling, GrappleMotion.Create(this.gameObject, _charBody, _charCollider));
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


            lookRotation = Quaternion.Euler(_inputs.mousePositionY, _inputs.mousePositionX, 0);
            RaycastHit lookHit;
            if (Physics.Raycast(this.transform.position, lookRotation * Vector3.forward, out lookHit))
                lookPoint = lookHit.point;
            else
                lookPoint = this.transform.position + lookRotation * Vector3.forward * 10;
        }


        private void UpdateState()
        {
            _previousState = _currentState;

            if (Input.GetKeyDown(KeyCode.Mouse0) && Input.GetKey(KeyCode.Mouse0)) 
            {
                RaycastHit grappleHit;
                if ( (_charMotions[CharState.Grappling] as GrappleMotion).CheckDistance(out grappleHit, lookRotation))
                {
                    (_charMotions[CharState.Grappling] as GrappleMotion).TryGrapple(grappleHit);
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
    }
}