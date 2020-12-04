using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CharControl
{   
    enum CharState 
    {
        None = 0,
        Freefalling,
        Slidefall,
        Walking,
        Grappling,
        Flying
    }

    public class CharController : MonoBehaviour
    {
        SurfaceController _surface;
        Rigidbody _characterBody;
        Collider _characterCollider;

        [SerializeField]
        CharState _currentState;

        Vector3 stepInput;
        Vector2 deltaMouse;
        Vector2 charLookAngles;

        Quaternion charYaw;
        Quaternion charPitch;


        void Awake()
        {
            _characterBody = gameObject.GetComponent<Rigidbody>();
            _characterCollider = gameObject.GetComponent<Collider>();
            _surface = new SurfaceController(_characterCollider);

            _currentState = CharState.None;
        }


        void FixedUpdate()
        {
            UpdateState();
            UpdatePosition();
        }

        void OnGUI()
        {
            // KEYBOARD STEP INPUT
            stepInput = new Vector3(   (Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0),
                                            0,
                                            (Input.GetKey(KeyCode.W) ? 1 : 0)  - (Input.GetKey(KeyCode.S) ? 1 : 0)  );

            // MOUSE ROTATION DELTA
            deltaMouse = new Vector2(  -Input.GetAxis("Mouse Y"),
                                        Input.GetAxis("Mouse X")    ) * 3.0f;
            charLookAngles += deltaMouse;
            
            charYaw = Quaternion.Euler(0, charLookAngles.y, 0);
            charPitch = Quaternion.Euler(charLookAngles.x, 0, 0);
        }

        void UpdateState()
        {
            CharState oldState = _currentState;

            // GET SURFACE STATE
            _surface.Check();
            if (_surface.contactSeparation < 0.20f) {
                _currentState = CharState.Walking;
            } else {
                _currentState = CharState.Freefalling;
            }

            // SWITCH STATE ACCORDINGLY
            if (oldState != _currentState)
            {
                Debug.Log("State:" + oldState + "=> State:" + _currentState);

                switch (_currentState)
                {
                    case CharState.Freefalling: {
                        _characterBody.isKinematic = false;
                        break;
                    }

                    case CharState.Walking: {
                        _characterBody.isKinematic = true;
                        break;
                    }

                    default:
                        break;
                }
            }
        }

        void UpdatePosition()
        {
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
            Quaternion lookRotation = Quaternion.Euler(charLookAngles.x, charLookAngles.y, 0);

            Debug.DrawRay(  transform.position,
                            lookRotation * Vector3.forward * 3,
                            Color.black,
                            Time.deltaTime  );  
          
            Vector3 heightAdjustment = new Vector3(0, _surface.contactSeparation - 0.1f, 0) * 0.5f * -1.0f;

            // 0.1f step speed
            Vector3 step = _surface.rotationToNormal * charYaw * stepInput * 0.1f;

            // APPLY VELOCITIES
            transform.position +=   heightAdjustment
                                    + step;
                                    
            // APPLY ROTATION    
            transform.rotation = charYaw;
        }
    }
}
