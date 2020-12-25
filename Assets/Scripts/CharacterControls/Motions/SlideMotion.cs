using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CharControl
{
    public class SlideMotion : Motion
    {
        public SlideMotion(Rigidbody charBody, Collider charCollider)
        {
            _charBody = charBody;
            _charCollider = charCollider;
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

                                    
            //_velocity = Vector3.Project(oldVelocity, _surface.downhillVector);
        }

        public override void ProcessMotion()
        {
            //Vector3 heightAdjust = new Vector3(0, _surface.contactSeparation - (_charCollider.bounds.extents.y + 0.2f), 0) * 0.4f;
            //_velocity = Vector3.Lerp(_velocity, Vector3.Project(Physics.gravity * Time.deltaTime, _surface.downhillVector), 0.2f); 

            //_charBody.transform.position +=/* - heightAdjust*/ + _velocity);
            _charBody.velocity = Vector3.zero;
        }

        public override Vector3 EndMotion() 
        {
            return _velocity;
        }
    }
}
