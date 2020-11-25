using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerControl
{
    interface IMoveController
    {
        void TransformVelocity( Vector3 newVelocity );

        Vector3 UpdateVelocity();
    }


    public class WalkingController : IMoveController
    {
        private Vector3 velocity;

        public void TransformVelocity( Vector3 newVelocity )
        {

        }

        public Vector3 UpdateVelocity()
        {
            Vector3 newVelocity = Vector3.zero;
            return newVelocity;
        }

    }

}