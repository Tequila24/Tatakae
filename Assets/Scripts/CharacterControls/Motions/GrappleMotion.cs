using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LogToFile;

namespace CharControl
{
    public class GrappleMotion : Motion
    {
        Vector3 grapplePoint = Vector3.zero;
        Vector3 toGrapplePoint = Vector3.zero;

        private LineRenderer lineRender = null;

        private Vector3 _inertia = Vector3.zero;

        FileLog flog;

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
            _charBody.useGravity = true;
            _charBody.constraints = RigidbodyConstraints.FreezeRotationX |
                                    RigidbodyConstraints.FreezeRotationZ;

            lineRender.enabled = true;

            _inertia = oldVelocity;
            Debug.Log(_inertia.ToString("F6"));
        }

        public override void ProcessMotion()
        {
            toGrapplePoint = (grapplePoint - _charBody.transform.position).normalized;


            Quaternion lookRotation = Quaternion.Euler(_inputs.mousePositionY, _inputs.mousePositionX, 0);
            Vector3 lookVector = Vector3.ProjectOnPlane(lookRotation * Vector3.forward, toGrapplePoint);

            _velocity = Vector3.Lerp(   _velocity, 
                                        (toGrapplePoint + lookVector) * (20),
                                        0.1f);

            _inertia = Vector3.Slerp(    _inertia,
                                        Vector3.zero,
                                        0.00001f);


            _charBody.velocity = _inertia + _velocity;



            Quaternion lookDirection = Quaternion.Euler(0, _inputs.mousePositionX, 0);           // rotation to mouse look
            _charBody.MoveRotation( Quaternion.RotateTowards(   _charBody.transform.rotation,
                                                                lookDirection,
                                                                10.0f ) );


            // RENDER GRAPPLE LINE
            lineRender.SetPosition(1, _charBody.transform.position);
        }

        public override Vector3 GetVelocity()
        {
            //_inertia = Vector3.zero;
            //_velocity = Vector3.zero;

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