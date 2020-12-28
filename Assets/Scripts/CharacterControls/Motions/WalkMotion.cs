using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CharControl
{
    public class WalkMotion : Motion
    {
        public static float stairHeight = 0.25f;
        SurfaceController sControl;
        Surface _currentSurface;
        Surface _nextSurface;

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
            _charBody.rotation = Quaternion.Euler(0, _inputs.mousePositionX, 0);

            UpdateSurfaces();
            _velocity = Vector3.ProjectOnPlane(oldVelocity, _currentSurface.contactPointNormal);

        }

        public override void ProcessMotion()
        {
            UpdateSurfaces();

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

            Vector3 heightAdjust = new Vector3(0, (_currentSurface.contactPoint.y + _charCollider.bounds.extents.y * 1.2f) - _charBody.transform.position.y, 0) * 0.2f;

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
            return _velocity;
        }

        private void UpdateSurfaces()
        {
            _currentSurface = SurfaceController.GetSurface( _charBody.transform.position, 
                                                            Physics.gravity, 
                                                            _charCollider.bounds.size.y, 
                                                            _charBody.transform);

            _nextSurface = SurfaceController.GetSurface(_charBody.transform.position + (_velocity.normalized * _charCollider.bounds.extents.x), 
                                                        Physics.gravity, 
                                                        _charCollider.bounds.size.y * 2, 
                                                        _charBody.transform);
        }

    }
}