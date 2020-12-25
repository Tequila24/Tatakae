using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CharControl
{
    public class WalkMotion : Motion
    {
        public static float stairHeight = 0.4f;
        private SurfaceController _surface;

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
            _charBody.useGravity = false;
            _charBody.constraints = RigidbodyConstraints.FreezeRotationX | 
                                    RigidbodyConstraints.FreezeRotationZ;
            

            _velocity = Vector3.ProjectOnPlane(oldVelocity, _surface.contactPointNormal);
        }

        public override void ProcessMotion()
        {
            Quaternion lookRotation = Quaternion.Euler(0, _inputs.mousePositionX, 0);          // rotation to mouse look

            Vector3 step =  (_charBody.transform.forward * (_inputs.forward - _inputs.backward) + 
                             _charBody.transform.right * (_inputs.right - _inputs.left) ) * 
                            0.1f ;                                                                                                  // walk speed
            
            // VELOCITIES
            _charBody.velocity = Vector3.zero;
            _velocity = Vector3.Lerp(_velocity, step, 0.5f);
            
            _charBody.MovePosition( _charBody.transform.position + 
                                    _surface.rotationToNormal * _velocity + 
                                    _surface.contactPointRelativeVelocity * Time.deltaTime );

            // ROTATION
            _charBody.angularVelocity = Vector3.zero;

            if (step.sqrMagnitude > 0) 
                _charBody.transform.rotation = Quaternion.Lerp( _charBody.transform.rotation, 
                                                                lookRotation,
                                                                0.1f);
            else
                _charBody.transform.rotation = Quaternion.Lerp( _charBody.transform.rotation, 
                                                                Quaternion.FromToRotation(Vector3.forward,
                                                                                          Vector3.ProjectOnPlane(_charBody.transform.forward, Vector3.up)),
                                                                0.1f );
            //_charBody.transform.rotation = _charBody.transform.rotation * Quaternion.Euler(0, _surface.angularVelocity.y, 0);
        }

        public override Vector3 EndMotion()
        {
            return _velocity;
        }
    }
}