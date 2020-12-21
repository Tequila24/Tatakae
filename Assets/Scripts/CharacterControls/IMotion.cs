using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CharControl
{
    public interface IMotion
    {
        void UpdateInputs(InputState newInputs);

        void BeginMotion(Vector3 oldVelocity);

        void ProcessMotion();

        Vector3 EndMotion();
    }

}