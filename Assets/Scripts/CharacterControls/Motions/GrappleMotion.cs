using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CharControl
{
    public class GrappleMotion : Motion
    {
        Vector3 grapplePoint = Vector3.zero;
        Vector3 toGrapplePoint = Vector3.zero;

        private LineRenderer lineRender = null;



        public GrappleMotion(Rigidbody charBody)
        {
            _charBody = charBody;

            if (_charBody.GetComponent<LineRenderer>() == null)
                lineRender = _charBody.gameObject.AddComponent<LineRenderer>();
            else
                lineRender = _charBody.GetComponent<LineRenderer>();

            lineRender.SetPosition(0, _charBody.transform.position);
            lineRender.SetPosition(1, _charBody.transform.position);
		    lineRender.positionCount = 2;
		    lineRender.startWidth = lineRender.endWidth = 0.15f;
		    lineRender.material = new Material(Shader.Find("Sprites/Default"));
		    lineRender.startColor = lineRender.endColor = Color.black;
        }

        public override void UpdateInputs(InputState newInputs)
        {
            _inputs = newInputs;
        }

        public override void BeginMotion(Vector3 oldVelocity)
        {
            _charBody.useGravity = false;
            _charBody.constraints = RigidbodyConstraints.FreezeRotationX |
                                    RigidbodyConstraints.FreezeRotationZ;

            lineRender.enabled = true;

            _velocity = oldVelocity;
        }

        public override void ProcessMotion()
        {
            toGrapplePoint = (grapplePoint - _charBody.transform.position).normalized;

            Quaternion lookRotation = Quaternion.Euler(_inputs.mousePositionY, _inputs.mousePositionX, 0);
            Vector3 lookVector = Vector3.ProjectOnPlane(lookRotation * Vector3.forward, toGrapplePoint);


            _velocity = Vector3.Lerp(   _velocity, 
                                        (toGrapplePoint + lookVector) * (20 + Mathf.Clamp(Vector3.Distance(_charBody.transform.position, grapplePoint), 0, 5 )),
                                        0.01f);

            _charBody.velocity = _velocity;


            lineRender.SetPosition(1, _charBody.transform.position);
        }

        public override Vector3 GetVelocity()
        {
            lineRender.enabled = false;
            return _charBody.velocity * Time.deltaTime;
        }


        public bool TryGrapple(Quaternion lookDirection)
        {
            bool isGrappled = false;


            RaycastHit hit;
            Debug.DrawLine( _charBody.transform.position,
                            lookDirection * Vector3.forward * 100.0f,
                            Color.blue, 
                            0.5f);
            if (Physics.Raycast(    _charBody.transform.position, 
                                    lookDirection * Vector3.forward,
                                    out hit,
                                    100.0f ) )
            {
                grapplePoint = hit.point;
                lineRender.SetPosition(0, grapplePoint);
                isGrappled = true;
            } else {
                grapplePoint = Vector3.zero;
                isGrappled = false;
            }



            return isGrappled;
        }
    }
}