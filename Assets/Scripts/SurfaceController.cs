using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CharControl
{  

    public class SurfaceController
    {
        Collider objectCollider;
        
	    public GameObject surfaceObject;
	    public Vector3 point;
	    public Vector3 normal;
        public Quaternion rotationToNormal;
        public float contactSeparation;
	    public Vector3 pointVelocity;
	    public Vector3 angularVelocity;

        public SurfaceController(Collider newCollider)
        {
            objectCollider = newCollider;
        }

        public bool isGrounded() 
        {
            return ( Mathf.Abs(contactSeparation) < Mathf.Abs(objectCollider.bounds.size.y*0.3f) ) ? true : false;
        }

        public void Check()
        {
            RaycastHit surfaceRay;

            
            Debug.DrawRay(  objectCollider.bounds.center - new Vector3(0, objectCollider.bounds.extents.y, 0),
                            -Vector3.up * objectCollider.bounds.size.y, 
                            Color.black, 
                            Time.deltaTime);


            if (Physics.Raycast(    objectCollider.bounds.center - new Vector3(0, objectCollider.bounds.extents.y, 0),
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
            point = rayHit.point;
            normal = rayHit.normal;
            rotationToNormal = Quaternion.FromToRotation(Vector3.up, normal);
            contactSeparation = (objectCollider.bounds.center - new Vector3(0, objectCollider.bounds.extents.y, 0) - point).y;
            pointVelocity = Vector3.zero;
            angularVelocity = Vector3.zero;


            Debug.DrawRay(  point,
                            new Vector3(0, contactSeparation, 0),
                            Color.green, 
                            Time.deltaTime);
        }

        public void Reset()
        {
            surfaceObject = null;
            point = Vector3.zero;
            normal = Vector3.zero;
            contactSeparation = Mathf.Infinity;
            pointVelocity = Vector3.zero;
            angularVelocity = Vector3.zero;
        }
    }
}