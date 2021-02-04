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

        FileLog flog;

        public WalkMotion(Rigidbody charBody, Collider charCollider)
        {
            _charBody = charBody;
            _charCollider = charCollider;
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
            Debug.DrawRay(_charBody.transform.position, _velocity, Color.red, Time.deltaTime);
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

                    Vector3 heightAdjust = new Vector3(0, (_currentSurface.contactPoint.y + _charCollider.bounds.extents.y * 1.3f) - _charBody.transform.position.y, 0) * 0.2f;


                    // CHECK IF MOVEMENT BLOCKED
                    RaycastHit hit;
                    if (_charBody.SweepTest(_velocity * Time.deltaTime, out hit, _velocity.magnitude))
                    {
                        if (hit.collider.attachedRigidbody != null)
                            hit.collider.attachedRigidbody.AddForceAtPosition( _charBody.velocity, hit.point, ForceMode.Impulse);
                        _velocity = Vector3.ProjectOnPlane(Vector3.ProjectOnPlane(_velocity, hit.normal), _currentSurface.contactPointNormal);
                    }
                    Vector3 depenetrationVector = CheckCollision();
                    if (depenetrationVector.sqrMagnitude > 0 )
                        _charBody.transform.position += Vector3.ProjectOnPlane(depenetrationVector, _currentSurface.contactPointNormal);

                    // APPLY VELOCITY
                    _charBody.MovePosition( _charBody.transform.position + 
                                            heightAdjust + 
                                            _velocity);


                    
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

                // sliding downhill
                _velocity = Vector3.Lerp(_velocity, _currentSurface.downhillVector * 0.15f , 0.2f);

                // CHECK IF MOVEMENT BLOCKED
                Vector3 _sumVelocity =  _velocity;

                RaycastHit hit;
                if (_charBody.SweepTest(_sumVelocity * Time.deltaTime, out hit, _sumVelocity.magnitude))
                {

                    _sumVelocity = Vector3.ProjectOnPlane(_sumVelocity, hit.normal);
                }

                // APPLY VELOCITY
                _charBody.MovePosition( _charBody.transform.position + 
                                        _sumVelocity);

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