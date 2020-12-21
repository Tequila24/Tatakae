using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CharControl
{
    public class FreefallMotion : IMotion
    {
        InputState _inputs;
        Vector3 _velocity;

        Rigidbody _charBody;
        Collider _charCollider;
        SurfaceController _surface;


        FreefallMotion(Rigidbody charBody, Collider charCollider, SurfaceController surface)
        {
            _charBody = charBody;
            _charCollider = charCollider;
        }

        public void UpdateInputs(InputState newInputs)
        {
            _inputs = newInputs;
        }

        public void BeginMotion(Vector3 oldVelocity)
        {
            _velocity = oldVelocity;
        }

        public void ProcessMotion()
        {
            _velocity = Vector3.Lerp(_velocity, Vector3.zero, 0.001f);
            _charBody.MovePosition(_charBody.transform.position + _velocity);
        }

        public Vector3 EndMotion() 
        {
            return _velocity;
        }
    }
}