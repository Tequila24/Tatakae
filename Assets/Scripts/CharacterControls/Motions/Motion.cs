using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CharControl
{
    abstract public class Motion
    {
        abstract public void UpdateInputs(InputState newInputs);

        abstract public void BeginMotion(Vector3 oldVelocity);

        abstract public void ProcessMotion();

        abstract public Vector3 EndMotion();
    }

}