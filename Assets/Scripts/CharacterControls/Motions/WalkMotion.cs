using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CharControl
{
    public class WalkMotion : Motion
    {
        InputState _inputs;
        Vector3 _velocity;

        Rigidbody _charBody;
        Collider _charCollider;
        SurfaceController _surface;


        public WalkMotion(Rigidbody charBody, Collider charCollider, SurfaceController surface)
        {
            _charBody = charBody;
            _charCollider = charCollider;
            _surface = surface;
        }

        public override void UpdateInputs(InputState newInputs)
        {
            _inputs = newInputs;
        }

        public override void BeginMotion(Vector3 oldVelocity)
        {
            _velocity = oldVelocity;

            _charBody.useGravity = false;
            _charBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }

        public override void ProcessMotion()
        {
            Vector3 heightAdjust = new Vector3(0, _surface.contactSeparation - (_charCollider.bounds.extents.y + 0.1f), 0) * 0.4f;
            Vector3 step =  _surface.rotationToNormal *                                                                             //rotation to surface
                            Quaternion.Euler(0, _inputs.mousePositionX, 0) *                                                        // rotation to horizontal lookDirection
                            new Vector3(_inputs.right - _inputs.left, 0 , _inputs.forward - _inputs.backward) *                     // step 
                            0.1f ;                                                                                                  // walk speed


            // APPLY VELOCITIES
            _velocity = Vector3.Lerp(_velocity, step, 0.2f);
            _charBody.MovePosition(_charBody.transform.position - heightAdjust + _velocity);
            _charBody.velocity = Vector3.zero;


            // APPLY ROTATION
            _charBody.transform.rotation = Quaternion.Euler(0, _inputs.mousePositionX, 0);
        }

        public override Vector3 EndMotion() 
        {
            return _velocity;
        }
    }
}