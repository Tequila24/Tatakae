using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CharControl
{
    abstract public class Motion : MonoBehaviour
    {
        protected InputState _inputs;
        protected Vector3 _velocity;

        protected Rigidbody _charBody;
        protected Collider _charCollider;

        protected Vector3 _contactNormal;

        public void UpdateInputs(InputState newInputs)
        {
            _inputs = newInputs;
        }

        abstract public void BeginMotion(Vector3 oldVelocity);

        abstract public void ProcessMotion();

        abstract public Vector3 GetVelocity();

        protected Vector3 CheckCollision()
        {
            Vector3 summDepenetrationVector = Vector3.zero;

            Collider[] hits = new Collider[10];
            int hitsAmount = Physics.OverlapBoxNonAlloc(  _charCollider.transform.position, _charCollider.bounds.extents, 
                                                            hits, _charCollider.transform.rotation);

            if ( hitsAmount > 0 )
            {
                for (int i = 0; i < hitsAmount; i++)
                {
                    Collider hit = hits[i];
                    if (hit.gameObject == _charCollider.gameObject)
                        continue;

                    Vector3 depenetrationDirection;
                    float depenetrationDistance;
                    Physics.ComputePenetration( _charCollider, _charCollider.transform.position, _charCollider.transform.rotation,
                                                hit, hit.transform.position, hit.transform.rotation,
                                                out depenetrationDirection, out depenetrationDistance);
                    summDepenetrationVector += depenetrationDirection * depenetrationDistance;
                }
            }

            return summDepenetrationVector;
        }


        void FixedUpdate()
        {
            Vector3 depenetrationVector = CheckCollision();
            if (depenetrationVector.sqrMagnitude > 0 ) {
                Debug.Log(depenetrationVector.magnitude);
                _charBody.transform.position += depenetrationVector;
                Debug.Log("Inside another object");
            }
        }


        void OnCollisionEnter(Collision hit)
        {
            for (int i = 0; i < hit.contactCount; i++)
            {
                _contactNormal += hit.contacts[i].normal;    
            }
            _contactNormal.Normalize();

            if (_contactNormal.sqrMagnitude != 0) 
                        if (Vector3.Angle(_velocity, _contactNormal) > 90)
                            _velocity = Vector3.ProjectOnPlane(_velocity, _contactNormal); 
        }

        void OnCollisionStay(Collision hit)
        {
            for (int i = 0; i < hit.contactCount; i++)
            {
                _contactNormal += hit.contacts[i].normal;    
            }
            _contactNormal.Normalize();

            if (_contactNormal.sqrMagnitude != 0) 
                if (Vector3.Angle(_velocity, _contactNormal) > 90)
                    _velocity = Vector3.ProjectOnPlane(_velocity, _contactNormal); 
        }

        void OnCollisionExit(Collision hit)
        {
            _contactNormal = Vector3.zero;
        }
    }

}