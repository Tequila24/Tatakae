using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CharControl
{  

    public class SurfaceController
    {
        private Collider objectCollider;
        
	    public GameObject surfaceObject;
	    public Vector3 contactPoint;
	    public Vector3 contactPointNormal;
        public float contactSeparation;
	    public Vector3 contactPointVelocity;
	    public Vector3 angularVelocity;
        public Vector3 downhillVector;
        public Quaternion rotationToNormal;
        public Quaternion rotationFromNormal;

        public SurfaceController(Collider newCollider)
        {
            objectCollider = newCollider;
        }

        public void Check()
        {
            RaycastHit surfaceRay;

            
            Debug.DrawRay(  objectCollider.bounds.center,
                            -Vector3.up * objectCollider.bounds.size.y, 
                            Color.black, 
                            Time.deltaTime);


            if (Physics.Raycast(    objectCollider.bounds.center,
                                    -Vector3.up * objectCollider.bounds.size.y,
                                    out surfaceRay,
                                    objectCollider.bounds.size.y) ) {
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

            contactSeparation = (objectCollider.bounds.center - new Vector3(0, objectCollider.bounds.extents.y, 0) - contactPoint).y;

            contactPointVelocity = Vector3.zero;
            angularVelocity = Vector3.zero;
            downhillVector = Vector3.Cross(contactPointNormal, Vector3.Cross(contactPointNormal, Vector3.up));

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
            angularVelocity = Vector3.zero;
            downhillVector = Vector3.zero;

            rotationToNormal = Quaternion.identity;
            rotationFromNormal = Quaternion.identity;
        }
    }
}