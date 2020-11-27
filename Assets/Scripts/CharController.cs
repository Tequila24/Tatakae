using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CharControl
{   
    enum CharState 
    {
        Freefall,
        Slidefall,
        Walking,
        Grappling,
        Flying,
        None = 0
    }

    public class CharController : MonoBehaviour
    {
        SurfaceController surface;
        Rigidbody characterBody;
        Collider characterCollider;

        CharState currentState;

        Vector3 currentVelocity;


        void Awake()
        {
            characterBody = this.gameObject.GetComponent<Rigidbody>();
            characterCollider = this.gameObject.GetComponent<Collider>();

            if (characterBody == null)
            {
                characterBody = this.gameObject.AddComponent<Rigidbody>();
            }
            characterBody.isKinematic = true;

            surface = new SurfaceController(characterCollider);

            currentState = CharState.None;
            currentVelocity = Vector3.zero;
        }


        void FixedUpdate()
        {
            UpdateState();

            UpdatePosition();
        }


        void UpdateState()
        {
            CharState oldState = currentState;

            surface.Check();
            if (surface.isGrounded())  {
                currentState = CharState.Walking;
            } else {
                // CHECK IF FLYING
                // ...
                currentState = CharState.Freefall;
            }

            // CHECK IF GRAPPLING
            // ...



            // TRANSFORM VELOCITY
            if (oldState != currentState)
            {
                Debug.Log("State:" + oldState + "=> State:" + currentState);
                switch (currentState)
                {
                    case CharState.Walking: 
                    {
                        currentVelocity = Vector3.ProjectOnPlane( currentVelocity, surface.contactPointNormal );
                        break;
                    }
                    default:
                        break;
                }
            }
        }

        void UpdatePosition()
        {
            switch (currentState)
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
            Vector3 inputDirection = new Vector3(   (Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0),
                                                    0,
                                                    (Input.GetKey(KeyCode.W) ? 1 : 0)  - (Input.GetKey(KeyCode.S) ? 1 : 0)  );
            Vector2 mouseDelta = new Vector2(   Input.GetAxis("Horizontal"),
                                                Input.GetAxis("Vertical") );            


            // VECTORS
            Quaternion rotationToNormal = Quaternion.FromToRotation(Vector3.up, surface.contactPointNormal);

            Vector3 step = rotationToNormal * inputDirection * 0.1f;

            currentVelocity = Vector3.Lerp(currentVelocity, step, 0.1f);

            Vector3 heightAdjustment = new Vector3(0, surface.contactSeparation - 0.1f, 0) * 0.1f;

            // APPLY

            characterBody.MovePosition(transform.position + (rotationToNormal * currentVelocity) - heightAdjustment);
        }

        void Fall()
        {
            currentVelocity = Vector3.Lerp(currentVelocity, Physics.gravity * Time.deltaTime, 0.3f);

            characterBody.MovePosition(transform.position + currentVelocity);
        }
    }
}
