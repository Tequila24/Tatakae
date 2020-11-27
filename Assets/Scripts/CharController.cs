using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CharControl
{   
    enum CharState 
    {
        None = 0,
        Freefall,
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

        CharState _currentState;

        Vector3 _inertia;
        Vector3 _currentVelocity;


        void Awake()
        {
            _characterBody = this.gameObject.GetComponent<Rigidbody>();
            _characterCollider = this.gameObject.GetComponent<Collider>();

            if (_characterBody == null)
            {
                _characterBody = this.gameObject.AddComponent<Rigidbody>();
            }
            _characterBody.isKinematic = true;

            _surface = new SurfaceController(_characterCollider);

            _currentState = CharState.None;
            _inertia = Vector3.zero;

        }


        void FixedUpdate()
        {
            UpdateState();

            UpdatePosition();
        }


        void UpdateState()
        {
            CharState oldState = _currentState;

            _surface.Check();
            if (_surface.isGrounded())  {
                _currentState = CharState.Walking;
            } else {
                // CHECK IF FLYING
                // ...
                _currentState = CharState.Freefall;
            }

            // CHECK IF GRAPPLING
            // ...


            // CHANGE CHARACTER PROPERTIES
            if (oldState != _currentState)
            {
                switch (_currentState)
                {
                    case CharState.Walking: 
                    {
                        _characterBody.isKinematic = true;
                        _inertia = Quaternion.Inverse(_surface.rotationToNormal) * Vector3.ProjectOnPlane( _currentVelocity, _surface.normal );
                        _currentVelocity = Vector3.zero;
                        Debug.DrawRay(_surface.point, _surface.rotationToNormal * _inertia * 10, Color.blue, 10);
                        break;
                    }
                    case CharState.Freefall:
                    {
                        _characterBody.isKinematic = false;
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
                case CharState.Walking: 
                    Walk();
                    break;

                case CharState.Freefall:
                    Fall();
                    break;

                default:
                    break;
            }
        }

        void Walk()
        {
            // READ INPUT
            Vector3 step = new Vector3( (Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0),
                                        0,
                                        (Input.GetKey(KeyCode.W) ? 1 : 0)  - (Input.GetKey(KeyCode.S) ? 1 : 0)  );
            Vector2 mouseDelta = new Vector2(   Input.GetAxis("Horizontal"),
                                                Input.GetAxis("Vertical") );            


            // VECTORS
            Vector3 heightAdjustment = new Vector3(0, _surface.contactSeparation - 0.1f, 0) * 0.1f;

            _currentVelocity = Vector3.Lerp(_currentVelocity, step * 0.1f, 0.1f);

            _inertia = Vector3.Lerp(_inertia, Vector3.zero, 0.1f);

            // APPLY
            _characterBody.MovePosition(    transform.position
                                            + (_surface.rotationToNormal * _currentVelocity)
                                            + (_surface.rotationToNormal * _inertia)
                                            - heightAdjustment  );
        }

        void Fall()
        {
            _currentVelocity = _characterBody.velocity * Time.deltaTime;
        }
    }
}
