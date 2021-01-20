using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CharControl
{
    public class FreefallMotion : Motion
    {

        Vector3 _inertia = Vector3.zero;


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

            _inertia = oldVelocity;
        }

        public override void ProcessMotion()
        {
            _inertia = Vector3.Lerp(_inertia, Vector3.zero, 0.04f);
            _charBody.MovePosition(_charBody.transform.position + _inertia );


            Quaternion lookDirection = Quaternion.Euler(0, _inputs.mousePositionX, 0);           // rotation to mouse look
            _charBody.MoveRotation( Quaternion.RotateTowards(   _charBody.transform.rotation,
                                                                lookDirection,
                                                                10.0f ) );
        }

        public override Vector3 GetVelocity() 
        {
            return (_charBody.velocity * Time.deltaTime);
        }
    }
}