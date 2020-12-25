using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CharControl
{
    public class FreefallMotion : Motion
    {
        public FreefallMotion(Rigidbody charBody)
        {
            _charBody = charBody;
        }

        public override void UpdateInputs(InputState newInputs)
        {
            _inputs = newInputs;
        }

        public override void BeginMotion(Vector3 oldVelocity)
        {
            _charBody.useGravity = true;
            _charBody.constraints = RigidbodyConstraints.None;

            _velocity = oldVelocity;
            _charBody.velocity = oldVelocity;
        }

        public override void ProcessMotion()
        {
            _velocity = Vector3.Lerp(_velocity, Vector3.zero, 0.1f);
            _charBody.MovePosition(_charBody.transform.position + _velocity );
        }

        public override Vector3 EndMotion() 
        {
            Vector3 vOut = _charBody.velocity * Time.deltaTime + _velocity;
            return (vOut);
        }
    }
}