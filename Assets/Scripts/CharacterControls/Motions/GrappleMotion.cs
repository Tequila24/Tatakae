using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CharControl
{
    public class GrappleMotion : Motion
    {
        Vector3 grapplePoint = Vector3.zero;


        public GrappleMotion(Rigidbody charBody)
        {
            _charBody = charBody;
        }

        public override void UpdateInputs(InputState newInputs)
        {
            _inputs = newInputs;
        }

        public override void BeginMotion(Vector3 oldVelocity)
        {
            _charBody.useGravity = false;
            _velocity = oldVelocity;
        }

        public override void ProcessMotion()
        {
            if (grapplePoint.sqrMagnitude != 0)
                Debug.DrawLine(_charBody.transform.position, grapplePoint, Color.red, Time.deltaTime);

            _charBody.velocity = Vector3.zero;

        }

        public override Vector3 GetVelocity()
        {
            return _velocity;
        }


        public bool TryGrapple(Quaternion lookDirection)
        {
            bool isGrappled = false;


            RaycastHit hit;
            Debug.DrawLine( _charBody.transform.position,
                            lookDirection * Vector3.forward * 1000.0f,
                            Color.blue, 
                            0.5f);
            if (Physics.Raycast(    _charBody.transform.position, 
                                    lookDirection * Vector3.forward,
                                    out hit,
                                    1000.0f ) )
            {
                grapplePoint = hit.point;
                isGrappled = true;
            } else {
                grapplePoint = Vector3.zero;
                isGrappled = false;
            }



            return isGrappled;
        }

        public void FreeGrapple()
        {
            grapplePoint = Vector3.zero;
        }
    }
}