using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LogToFile;

namespace CharControl
{
    public class WalkMotion : Motion
    {
        public static float stairHeight = 0.25f;
        SurfaceController sControl;
        Surface _currentSurface;
        Surface _nextSurface;

        Vector3 _inertia = Vector3.zero;

        FileLog flog;

        public WalkMotion(Rigidbody charBody, Collider charCollider)
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
            _charBody.useGravity = false;
            _charBody.constraints = RigidbodyConstraints.FreezeRotationX |
                                    RigidbodyConstraints.FreezeRotationZ;

            _charBody.velocity = Vector3.zero;
            _charBody.angularVelocity = Vector3.zero;
            //_charBody.rotation = Quaternion.Euler(0, _inputs.mousePositionX, 0);


            _currentSurface = SurfaceController.GetSurface( _charBody.transform.position, 
                                                            Physics.gravity, 
                                                            _charCollider.bounds.size.y, 
                                                            _charBody.transform);

            _inertia = Vector3.ProjectOnPlane(oldVelocity, _currentSurface.contactPointNormal);
        }

        public override void ProcessMotion()
        {
            _charBody.velocity = Vector3.zero;
            _charBody.angularVelocity = Vector3.zero;
            
            _currentSurface = SurfaceController.GetSurface( _charBody.transform.position, 
                                                            Physics.gravity, 
                                                            _charCollider.bounds.size.y, 
                                                            _charBody.transform );

            _inertia = Vector3.Lerp(    _inertia,
                                        Vector3.zero,
                                        0.05f);                                                            

            float currentSurfaceIncline = Vector3.Angle(Vector3.up, _currentSurface.contactPointNormal);

            if (currentSurfaceIncline < 30) {

                if ( /*not stairs*/ true) {     // how to distinguish between stairs and any other surface?
                    
                    // walking on surface
                    Vector3 step = (_charBody.transform.forward * (_inputs.forward - _inputs.backward) +
                                    _charBody.transform.right * (_inputs.right - _inputs.left)).normalized *
                                    (_inputs.shift != 0 ? 0.15f : 0.05f);

                    _velocity = Vector3.Lerp(_velocity, step, 0.2f);


                    Vector3 heightAdjust = new Vector3(0, (_currentSurface.contactPoint.y + _charCollider.bounds.extents.y * 1.3f) - _charBody.transform.position.y, 0) * 0.2f;


                    _charBody.MovePosition( _charBody.transform.position + 
                                            heightAdjust + 
                                            _currentSurface.rotationToNormal * _velocity +
                                            _inertia);



                    // rotation
                    Quaternion lookDirection = Quaternion.Euler(0, _inputs.mousePositionX, 0);           // rotation to mouse look

                    _charBody.MoveRotation( Quaternion.RotateTowards(   _charBody.transform.rotation,
                                                                        lookDirection,
                                                                        15.0f ) );


                } else {
                    // walking on stairs
                    // Do I Even Need This?!
                    // yes i do
                }

            } else {

                // sliding downhill
                _velocity = Vector3.Lerp(_velocity, _currentSurface.downhillVector * 0.15f , 0.2f);

                _charBody.MovePosition( _charBody.transform.position + 
                                        _currentSurface.rotationToNormal * _velocity +
                                        _inertia);

                _charBody.MoveRotation( Quaternion.RotateTowards(   _charBody.transform.rotation,
                                                                    Quaternion.LookRotation(_currentSurface.downhillVector, Vector3.up),
                                                                    5.0f ) );
            }

        }

        public override Vector3 GetVelocity()
        {
            return (_currentSurface.rotationToNormal * _velocity + _inertia);
        }

    }
}