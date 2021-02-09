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

        Vector3 _heightAdjust;

        FileLog flog;

        public static WalkMotion Create(GameObject parent, Rigidbody charBody, Collider charCollider)
        {
            WalkMotion motion = parent.GetComponent<WalkMotion>();
            if (motion == null)
                motion = parent.AddComponent<WalkMotion>();

            motion._charBody = charBody;
            motion._charCollider = charCollider;

            return motion;
        }

        public override void BeginMotion(Vector3 oldVelocity)
        {
            _currentSurface = SurfaceController.GetSurface( _charBody.transform.position, 
                                                            Physics.gravity, 
                                                            _charCollider.bounds.size.y, 
                                                            _charBody.transform);

            _velocity = Vector3.ProjectOnPlane(oldVelocity, _currentSurface.contactPointNormal);
        }

        public override void ProcessMotion()
        {
            // APPLY VELOCITY
            _charBody.MovePosition( _charBody.transform.position + 
                                    _heightAdjust + 
                                    _velocity);



            _currentSurface = SurfaceController.GetSurface( _charBody.transform.position, 
                                                            Physics.gravity, 
                                                            _charCollider.bounds.size.y, 
                                                            _charBody.transform );
            float currentSurfaceIncline = Vector3.Angle(Vector3.up, _currentSurface.contactPointNormal);

            if (currentSurfaceIncline < 30) {

                if ( /*not stairs*/ true) {     // how to distinguish between stairs and any other surface?
                    
                    // walking on surface
                    Vector3 step = (_charBody.transform.forward * (_inputs.forward - _inputs.backward) +
                                    _charBody.transform.right * (_inputs.right - _inputs.left)).normalized *
                                    (_inputs.shift != 0 ? 0.15f : 0.05f);

                    _velocity = Vector3.MoveTowards(_velocity, _currentSurface.rotationToNormal * step, 0.005f);

                    _heightAdjust = new Vector3(0, (_currentSurface.contactPoint.y + _charCollider.bounds.extents.y * 1.3f) - _charBody.transform.position.y, 0) * 0.2f;

                    /*if (_contactNormal.sqrMagnitude != 0) 
                        if (Vector3.Angle(_velocity, _contactNormal) > 90)
                            _velocity = Vector3.ProjectOnPlane(_velocity, _contactNormal);*/

                    


                    
                    Quaternion lookDirection = Quaternion.Euler(0, _inputs.mousePositionX, 0);           // rotation to mouse look

                    // APPLY ROTATION
                    _charBody.MoveRotation( Quaternion.RotateTowards(   _charBody.transform.rotation,
                                                                        lookDirection,
                                                                        15.0f ) );


                } else {
                    // walking on stairs
                    // Do I Even Need This?!
                    // yes i do
                }

            } else {

                // APPLY VELOCITY
                _charBody.MovePosition( _charBody.transform.position + 
                                        _velocity);


                // sliding downhill
                _velocity = Vector3.Lerp(_velocity, _currentSurface.downhillVector * 0.15f , 0.2f);

                _charBody.MoveRotation( Quaternion.RotateTowards(   _charBody.transform.rotation,
                                                                    Quaternion.LookRotation(_currentSurface.downhillVector, Vector3.up),
                                                                    5.0f ) );
            }

        }

        public override Vector3 GetVelocity()
        {
            return (_currentSurface.rotationToNormal * _velocity);
        }

    }
}