using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CharControl
{
    public class GrappleMotion : Motion
    {

        public struct GrappleInfo
        {
            public GameObject grappledObject;
            public Rigidbody grappledRigidbody;
            public Vector3 localGrapplePoint;
            public Vector3 toGrapplePoint;

            public void Set(RaycastHit rayHit)
            {
                grappledObject = rayHit.collider.gameObject;
                localGrapplePoint = Quaternion.Inverse(grappledObject.transform.rotation) * (rayHit.point - grappledObject.transform.position);
                grappledRigidbody = rayHit.collider.attachedRigidbody;
                Debug.DrawLine(Vector3.zero, localGrapplePoint, Color.red, 10);
            }

            public Vector3 GetWorldPoint()
            {
                return grappledObject.transform.position + grappledObject.transform.rotation * localGrapplePoint;
            }
            public Vector3 GetDirection(Vector3 pointFrom)
            {
                return (GetWorldPoint() - pointFrom);
            }
        }
        GrappleInfo Grapple;

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
            _charBody.useGravity = true;
            _charBody.constraints = RigidbodyConstraints.FreezeRotationX |
                                    RigidbodyConstraints.FreezeRotationZ;

            lineRender.enabled = true;
            _velocity = oldVelocity;
        }

        public override void ProcessMotion()
        {
            Quaternion lookRotation = Quaternion.Euler(_inputs.mousePositionY, _inputs.mousePositionX, 0);
            Vector3 lookVector = Vector3.ProjectOnPlane(lookRotation * Vector3.forward, Grapple.GetDirection(_charBody.transform.position));

            _velocity = Vector3.MoveTowards(    _velocity, 
                                                (Grapple.GetDirection(_charBody.transform.position) * 0.3f + lookVector * 0.2f),
                                                0.01f);

            // CHECK IF MOVEMENT BLOCKED
            RaycastHit hit;
            if (_charBody.SweepTest(_velocity * Time.deltaTime, out hit, _velocity.magnitude))
            {
                if (hit.collider.attachedRigidbody != null)
                    hit.collider.attachedRigidbody.AddForceAtPosition( _charBody.velocity * _charBody.mass, hit.point, ForceMode.Impulse);
                    
                _velocity = Vector3.ProjectOnPlane(_velocity, hit.normal);
            }

            // APPLY VELOCITY
            _charBody.MovePosition( _charBody.transform.position + 
                                    _velocity);

            // APPLY FORCE TO GRAPPLED OBJECT
            if (Grapple.grappledRigidbody != null)
                Grapple.grappledRigidbody.AddForceAtPosition( _velocity, Grapple.GetWorldPoint(), ForceMode.Force);


            Quaternion lookDirection = Quaternion.Euler(0, _inputs.mousePositionX, 0);           // rotation to mouse look

            _charBody.MoveRotation( Quaternion.RotateTowards(   _charBody.transform.rotation,
                                                                lookDirection,
                                                                10.0f ) );


            // RENDER GRAPPLE LINE
            lineRender.SetPosition(0, Grapple.GetWorldPoint() );
            lineRender.SetPosition(1, _charBody.transform.position);
        }

        public override Vector3 GetVelocity()
        {
            lineRender.enabled = false;
            return _velocity;
        }


        public bool TryGrapple(Quaternion lookDirection)
        {
            bool isGrappled = false;


            RaycastHit rayHit;
            Debug.DrawLine( _charBody.transform.position,
                            lookDirection * Vector3.forward * 100.0f,
                            Color.blue, 
                            0.5f);
            if (Physics.Raycast(    _charBody.transform.position, 
                                    lookDirection * Vector3.forward,
                                    out rayHit,
                                    100.0f ) )
            {
                Grapple.Set(rayHit);
                isGrappled = true;
            } else {
                isGrappled = false;
            }



            return isGrappled;
        }
    }
}