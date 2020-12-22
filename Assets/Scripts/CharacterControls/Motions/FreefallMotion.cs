using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CharControl
{
    public class FreefallMotion : Motion
    {
        InputState _inputs;
        Vector3 _velocity;

        Rigidbody _charBody;
        Collider _charCollider;
        SurfaceController _surface;


        public FreefallMotion(Rigidbody charBody, Collider charCollider, SurfaceController surface)
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
            _velocity = oldVelocity;

            _charBody.useGravity = true;
            _charBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }

        public override void ProcessMotion()
        {
            _velocity = Vector3.Lerp(_velocity, Vector3.zero, 0.001f);
            _charBody.MovePosition(_charBody.transform.position + _velocity);
        }

        public override Vector3 EndMotion() 
        {
            return _velocity;
        }
    }
}