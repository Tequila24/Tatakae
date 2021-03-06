﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CharControl
{
    public class GrappleMotion : Motion
    {
        float MaxGrappleDistance = 200.0f;

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
            public Vector3 GetFromTo(Vector3 pointFrom)
            {
                return (GetWorldPoint() - pointFrom);
            }
        }
        GrappleInfo Grapple;

        private LineRenderer lineRender = null;


        public static GrappleMotion Create(GameObject parent, Rigidbody charBody, Collider charCollider)
        {
            GrappleMotion motion = parent.GetComponent<GrappleMotion>();
            if (motion == null)
                motion = parent.AddComponent<GrappleMotion>();

            motion._charBody = charBody;
            motion._charCollider = charCollider;

            if (motion._charBody.GetComponent<LineRenderer>() == null)
                motion.lineRender = motion._charBody.gameObject.AddComponent<LineRenderer>();
            else
                motion.lineRender = motion._charBody.GetComponent<LineRenderer>();


            motion.lineRender.SetPosition(0, motion._charBody.transform.position);
            motion.lineRender.SetPosition(1, motion._charBody.transform.position);
		    motion.lineRender.positionCount = 2;
		    motion.lineRender.startWidth = motion.lineRender.endWidth = 0.15f;
		    motion.lineRender.material = new Material(Shader.Find("Sprites/Default"));
		    motion.lineRender.startColor = motion.lineRender.endColor = Color.black;

            return motion;
        }

        public override void BeginMotion(Vector3 oldVelocity)
        {
            lineRender.positionCount = 2;
            lineRender.enabled = true;
            _velocity = oldVelocity;
        }

        public override void ProcessMotion()
        {
            // APPLY VELOCITY
            _charBody.MovePosition( _charBody.transform.position + 
                                    _velocity);



            Quaternion lookRotation = Quaternion.Euler(_inputs.mousePositionY, _inputs.mousePositionX, 0);
            Vector3 lookVector = Vector3.ProjectOnPlane(lookRotation * Vector3.forward, Grapple.GetFromTo(_charBody.transform.position)).normalized;

            Vector3 grappleDirection = Grapple.GetFromTo(_charBody.transform.position).normalized;
            float grappleLength = Grapple.GetFromTo(_charBody.transform.position).magnitude;

            Vector3 grappleAcceleration =   (grappleDirection * 0.005f + lookVector * 0.0008f)
                                            * Mathf.Pow( Mathf.Clamp01(grappleLength * 0.1f), 1.5f)
                                            - (_velocity * 0.005f * _velocity.magnitude)
                                            + Physics.gravity * Time.deltaTime * 0.003f;

            _velocity += grappleAcceleration;
            _velocity = Vector3.ClampMagnitude(_velocity, 0.5f);

            /*if (_contactNormal.sqrMagnitude != 0)
                if (Vector3.Angle(_velocity, _contactNormal) > 90)
                    _velocity = Vector3.ProjectOnPlane(_velocity, _contactNormal);*/



            // APPLY ACCELERATION TO GRAPPLED OBJECT
            if (Grapple.grappledRigidbody != null)
            {
                Grapple.grappledRigidbody.AddForceAtPosition( -grappleAcceleration, Grapple.GetWorldPoint(), ForceMode.Acceleration);
            }



            Quaternion lookDirection = Quaternion.Euler(0, _inputs.mousePositionX, 0);           // rotation to mouse look
            _charBody.MoveRotation( Quaternion.RotateTowards(   _charBody.transform.rotation,
                                                                lookDirection,
                                                                10.0f ) );


            // RENDER GRAPPLE LINE
            lineRender.SetPosition(1, Grapple.GetWorldPoint() );
            lineRender.SetPosition(0, _charBody.transform.position);
        }

        public override Vector3 GetVelocity()
        {
            lineRender.enabled = false;
            lineRender.positionCount = 0;
            return _velocity;
        }


        public void TryGrapple(RaycastHit rayHit)
        {
            Grapple.Set(rayHit);
        }

        public bool CheckDistance(out RaycastHit rayHit, Quaternion lookRotation)
        {
            if (Physics.Raycast(    _charBody.transform.position, 
                                    lookRotation * Vector3.forward,
                                    out rayHit))
            {
                Debug.DrawRay(_charBody.transform.position, lookRotation * Vector3.forward * 10, Color.red, 1);
                if (rayHit.distance < MaxGrappleDistance)
                    return true;
            }

            return false;
        }

    }
}