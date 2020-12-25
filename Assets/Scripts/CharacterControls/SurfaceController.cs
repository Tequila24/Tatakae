using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CharControl
{
    public struct Surface
    {
        public GameObject surfaceObject;
	    public Vector3 contactPoint;
	    public Vector3 contactPointNormal;
        public float contactSeparation;
	    public Vector3 contactPointVelocity;
        public Vector3 contactPointRelativeVelocity;
	    public Vector3 angularVelocity;
        public Vector3 fullRotation;
        public Vector3 downhillVector;
        public Quaternion rotationToNormal;
        public Quaternion rotationFromNormal;

        public void Set(RaycastHit rayHit)
        {
            surfaceObject = rayHit.transform.gameObject;
            contactPoint = rayHit.point;
            contactPointNormal = rayHit.normal;

            Rigidbody surfaceBody = surfaceObject.GetComponent<Rigidbody>();
            if ( surfaceBody != null) {
                contactPointVelocity = surfaceBody.GetPointVelocity(contactPoint);
                contactPointRelativeVelocity = surfaceBody.GetRelativePointVelocity(contactPoint);
                angularVelocity = surfaceBody.angularVelocity;
                fullRotation += angularVelocity;
            }
            downhillVector = Vector3.Cross(contactPointNormal, Vector3.Cross(contactPointNormal, Vector3.up)).normalized;

            rotationToNormal = Quaternion.FromToRotation(Vector3.up, contactPointNormal);
            rotationFromNormal = Quaternion.Inverse(rotationToNormal);

            Debug.DrawRay(  contactPoint,
                            new Vector3(0, contactSeparation, 0),
                            Color.green, 
                            Time.deltaTime);
        }

        public void Reset()
        {
            surfaceObject = null;
            contactPoint = Vector3.zero;
            contactPointNormal = Vector3.zero;
            contactSeparation = Mathf.Infinity;
            contactPointVelocity = Vector3.zero;
            contactPointRelativeVelocity = Vector3.zero;
            angularVelocity = Vector3.zero;
            fullRotation = Vector3.zero;
            downhillVector = Vector3.zero;

            rotationToNormal = Quaternion.identity;
            rotationFromNormal = Quaternion.identity;
        }
    }
    
    public class SurfaceController
    {
        static public Surface GetSurface(Vector3 rayPosition, Vector3 rayDirection, float distance, Transform parent)
        {
            Surface newSurface = new Surface();

            RaycastHit surfaceRay;
            
            Debug.DrawRay(  rayPosition,
                            rayDirection.normalized * distance, 
                            Color.black, 
                            Time.deltaTime);


            if (Physics.Raycast(    rayPosition, rayDirection, out surfaceRay, distance) ) {
                if ( !(surfaceRay.transform.IsChildOf(parent)) )
            	    newSurface.Set(surfaceRay);
            } else {
            	newSurface.Reset();
            }

        return newSurface;
        }
    }
}