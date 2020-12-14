using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CharControl
{   
    public class CharController : MonoBehaviour
    {
        CharStateController _stateControl;
        SurfaceController _surface;
        Rigidbody _charBody;
        Collider _charCollider;

        Vector3 stepInput;
        Vector2 deltaMouse;
        Vector2 charLookAngles;

        Quaternion charYaw;
        Quaternion charPitch;


        void Awake()
        {
            _charBody = gameObject.GetComponent<Rigidbody>();
            _charCollider = gameObject.GetComponent<Collider>();
            _surface = new SurfaceController(_charCollider);

            _stateControl = new CharStateController(_surface, _charBody);
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


        void FixedUpdate()
        {
            _stateControl.UpdateState();


        }        

        void UpdateState()
        {
            

            
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
            Vector3 heightAdjustment = new Vector3(0, _surface.contactSeparation - 0.1f, 0) * 0.5f * -1.0f;


            Vector3 step = _surface.rotationToNormal * charYaw * stepInput * 0.05f;

            // APPLY VELOCITIES
            transform.position +=   heightAdjustment
                                    + step;
                                    
            // APPLY ROTATION    
            transform.rotation = charYaw;
        }
    }
}
