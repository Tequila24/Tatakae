using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CharControl
{
    public class FreefallMotion : Motion
    {
        public static FreefallMotion Create(GameObject parent, Rigidbody charBody, Collider charCollider)
        {
            FreefallMotion motion = parent.GetComponent<FreefallMotion>();
            if (motion == null)
                motion = parent.AddComponent<FreefallMotion>();

            motion._charBody = charBody;
            motion._charCollider = charCollider;
                
            return motion;
        }

        public override void BeginMotion(Vector3 oldVelocity)
        {
            _velocity = oldVelocity;
        }

        public override void ProcessMotion()
        {
            Vector3 step = (_charBody.transform.forward * (_inputs.forward - _inputs.backward) +
                                    _charBody.transform.right * (_inputs.right - _inputs.left)).normalized * 0.001f;
            

            // PROPERLY SMOOTH MOVEMENT
            _velocity.x = Mathf.MoveTowards(_velocity.x + step.x, 0, 0.00035f );
            _velocity.y = Mathf.MoveTowards(_velocity.y, Physics.gravity.y * 3, 0.0030f);
            _velocity.z = Mathf.MoveTowards(_velocity.z + step.z, 0, 0.00035f );


            // CHECK IF MOVEMENT BLOCKED
            RaycastHit hit;
            if (_charBody.SweepTest(_velocity * Time.deltaTime, out hit, _velocity.magnitude))
            {
                if (hit.collider.attachedRigidbody != null)
                    hit.collider.attachedRigidbody.AddForceAtPosition( _charBody.velocity, hit.point, ForceMode.Impulse);
                _velocity = Vector3.ProjectOnPlane(_velocity, hit.normal);
            }
            Vector3 depenetrationVector = CheckCollision();
            if (depenetrationVector.sqrMagnitude > 0 )
                _charBody.transform.position += depenetrationVector;
        

            // APPLY VELOCITY
            _charBody.MovePosition( _charBody.transform.position + 
                                    _velocity);



            Quaternion lookDirection = Quaternion.Euler(0, _inputs.mousePositionX, 0);           // rotation to mouse look

            _charBody.MoveRotation( Quaternion.RotateTowards(   _charBody.transform.rotation,
                                                                lookDirection,
                                                                10.0f ) );
        }

        public override Vector3 GetVelocity() 
        {
            return _velocity;
        }
    }
}