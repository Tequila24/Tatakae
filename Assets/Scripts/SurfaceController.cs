using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PlayerControl
{  

    public class SurfaceController
    {
        public float rayLength = 1.5f;
	    public GameObject surfaceObject;
	    public Vector3 contactPoint;
	    public Vector3 contactPointNormal;
	    public Vector3 contactPointVelocity;
	    public Vector3 angularVelocity;

        public bool isGrounded() 
        {
            return surfaceObject == null ? false : true;
        }

        public void Check(Vector3 position)
        {
            Debug.DrawRay(position, -Vector3.up * rayLength, Color.black, Time.deltaTime);

            RaycastHit surfaceRay;
            if (Physics.Raycast(position, -Vector3.up, out surfaceRay, rayLength)) {
            	Set(surfaceRay);
            } else {
            	Reset();
            }
        }

        public void Set(RaycastHit rayHit)
        {
            surfaceObject = rayHit.transform.gameObject;
            contactPoint = rayHit.point;
            contactPointNormal = rayHit.normal;

            contactPointVelocity = Vector3.zero;
            angularVelocity = Vector3.zero;
        }

        public void Reset()
        {
            surfaceObject = null;
            contactPoint = Vector3.zero;
            contactPointNormal = Vector3.zero;
            contactPointVelocity = Vector3.zero;
            angularVelocity = Vector3.zero;
        }
    }
}