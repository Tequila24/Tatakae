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

            _velocity = Vector3.ProjectOnPlane(oldVelocity, _currentSurface.contactPointNormal);
        }

        public override void ProcessMotion()
        {
            _charBody.velocity = Vector3.zero;
            
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

                    _velocity = Vector3.Lerp(_velocity, step, 0.5f);


                    Vector3 heightAdjust = new Vector3(0, (_currentSurface.contactPoint.y + _charCollider.bounds.extents.y * 1.3f) - _charBody.transform.position.y, 0) * 0.2f;


                    _charBody.MovePosition( _charBody.transform.position + 
                                            heightAdjust + 
                                            _currentSurface.rotationToNormal * _velocity);



                    // rotation
                    Quaternion lookDirection = Quaternion.Euler(0, _inputs.mousePositionX, 0);           // rotation to mouse look

                    _charBody.MoveRotation( Quaternion.RotateTowards(   _charBody.transform.rotation,
                                                                        lookDirection,
                                                                        15.0f ) );


                } else {
                    // walking on stairs
                    // Do I Even Need This?!
                }

            } else {

                // sliding downhill
                _velocity = Vector3.Lerp(_velocity, _currentSurface.downhillVector * 0.5f , 0.2f);

                _charBody.MovePosition( _charBody.transform.position + 
                                        _currentSurface.rotationToNormal * _velocity );

                _charBody.MoveRotation( Quaternion.RotateTowards(   _charBody.transform.rotation,
                                                                    Quaternion.LookRotation(_currentSurface.downhillVector, Vector3.up),
                                                                    5.0f ) );
            }



            
                                   //rotationToElevation *           // Rotation to surface normal
                                   
                                   //_currentSurface.contactPointVelocity * Time.deltaTime + 
                                   //heightAdjust
                                   

            /*

            Quaternion lookRotation = Quaternion.Euler(0, _inputs.mousePositionX, 0);           // rotation to mouse look

            Vector3 step = (_charBody.transform.forward * (_inputs.forward - _inputs.backward) +
                             _charBody.transform.right * (_inputs.right - _inputs.left)).normalized *
                            (_inputs.shift != 0 ? 0.15f : 0.05f);                                                               // walk speed

            // VELOCITIES
            _charBody.velocity = Vector3.zero;
            _velocity = Vector3.Lerp(_velocity, step, 0.5f);

            //Rotation to next step elevation
            Vector3 toNextStep = _currentSurface.contactPoint - _nextSurface.contactPoint;
            Quaternion rotationToElevation = Quaternion.FromToRotation( Vector3.ProjectOnPlane(toNextStep, Vector3.up),
                                                                        toNextStep );
            float nextStepHeight = _currentSurface.contactPoint.y - _nextSurface.contactPoint.y;

        
            if (nextStepHeight < 0.4f) {
                canClimb = true;
            } else {
                canClimb = false;
            }
            Debug.Log(nextStepHeight);

            Vector3 heightAdjust = new Vector3(0, (_currentSurface.contactPoint.y + _charCollider.bounds.extents.y * 1.4f) - _charBody.transform.position.y, 0) * 0.2f;

            _charBody.MovePosition(_charBody.transform.position + 
                                   rotationToElevation *           // Rotation to surface normal
                                   _velocity +
                                   _currentSurface.contactPointVelocity * Time.deltaTime + 
                                   heightAdjust
                                   );

            // ROTATIONS
            _charBody.angularVelocity = Vector3.zero;

            if (step.sqrMagnitude > 0)
                _charBody.transform.rotation = Quaternion.Lerp(_charBody.transform.rotation,
                                                                lookRotation,
                                                                0.5f);

            /*
            _charBody.MovePosition(_charBody.transform.position +
                                    rotationToNormal * _velocity +
                                    contactPointRelativeVelocity * Time.deltaTime);

            // ROTATION
            _charBody.angularVelocity = Vector3.zero;

            if (step.sqrMagnitude > 0)
                _charBody.transform.rotation = Quaternion.Lerp(_charBody.transform.rotation,
                                                                lookRotation,
                                                                0.1f);
            else
                _charBody.transform.rotation = Quaternion.Lerp(_charBody.transform.rotation,
                                                                Quaternion.FromToRotation(Vector3.forward,
                                                                                          Vector3.ProjectOnPlane(_charBody.transform.forward, Vector3.up)),
                                                                0.1f);
            //_charBody.transform.rotation = _charBody.transform.rotation * Quaternion.Euler(0, _surface.angularVelocity.y, 0);
            */
        }

        public override Vector3 GetVelocity()
        {
            return _currentSurface.rotationToNormal * _velocity;
        }

        /*
        private void UpdateSurfaces()
        {
            

            _nextSurface = SurfaceController.GetSurface(_charBody.transform.position + (_velocity.normalized * _charCollider.bounds.extents.x + _velocity), 
                                                        Physics.gravity, 
                                                        _charCollider.bounds.size.y, 
                                                        _charBody.transform);
        }
        */

    }
}