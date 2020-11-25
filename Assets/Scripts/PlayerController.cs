using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PlayerControl
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

    public class PlayerController : MonoBehaviour
    {
        SurfaceController surface;
        GameObject player;
        Rigidbody playerBody;

        CharState currentState;

        Vector3 currentVelocity;


        void Awake()
        {
            player = this.gameObject;

            playerBody = player.GetComponent<Rigidbody>();
            if (playerBody == null)
            {
                playerBody = player.AddComponent<Rigidbody>();
            }
            playerBody.isKinematic = true;

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

            surface.Check( transform.position );
            if (surface.isGrounded())  {
                currentState = CharState.Walking;
            } else {
                // CHECK IF FLYING
                // ...
                currentState = CharState.Freefall;
            }

            // CHECK IF GRAPPLING
            // ...



            // MODIFY VELOCITY
            if (oldState != currentState)
            {
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
            // modify speed


            // apply speed
            playerBody.MovePosition ( transform.position + currentSpeed );
        }
    }


}
