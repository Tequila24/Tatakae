using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CharControl
{
    abstract public class Motion
    {
        protected InputState _inputs;
        protected Vector3 _velocity;

        protected Rigidbody _charBody;
        protected Collider _charCollider;

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
            if ( hitsAmount > 1 )
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
    }

}