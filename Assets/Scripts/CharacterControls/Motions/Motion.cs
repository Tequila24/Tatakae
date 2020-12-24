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

        abstract public void UpdateInputs(InputState newInputs);

        abstract public void BeginMotion(Vector3 oldVelocity);

        abstract public void ProcessMotion();

        abstract public Vector3 EndMotion();
    }

}