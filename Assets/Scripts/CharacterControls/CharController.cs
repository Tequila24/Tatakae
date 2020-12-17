using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharControl;


public class CharController : MonoBehaviour
{
    private CharStateController _stateControl;
    private SurfaceController _surface;
    private Rigidbody _charBody;
    private Collider _charCollider;
    private Vector3 stepInput;
    private Vector2 deltaMouse;
    public Vector2 lookAngles;


    private Quaternion charYaw;
    private Quaternion charPitch;

    void OnValidate()
    {
        Start();
    }
    void Start()
    {
        _charBody = gameObject.GetComponent<Rigidbody>();
        _charCollider = gameObject.GetComponent<Collider>();
        _surface = new SurfaceController(_charCollider);
        _stateControl = new CharStateController(_surface, _charBody);
    }


    void OnGUI()
    {
        // KEYBOARD STEP INPUT
        stepInput = new Vector3(   (Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0),
                                    0,
                                    (Input.GetKey(KeyCode.W) ? 1 : 0)  - (Input.GetKey(KeyCode.S) ? 1 : 0)  );
        stepInput = Vector3.ClampMagnitude(stepInput, 1);


        // MOUSE ROTATION DELTA
        deltaMouse = new Vector2(  -Input.GetAxis("Mouse Y"),
                                    Input.GetAxis("Mouse X")    ) * 1.0f;
        lookAngles += deltaMouse;
        
        // CALCULATE ROTATIONS
        charYaw = Quaternion.Euler(0, lookAngles.y, 0);
        charPitch = Quaternion.Euler(lookAngles.x, 0, 0);
    }


    void FixedUpdate()
    {
        _stateControl.UpdateState();
        UpdatePosition();
    }


    void UpdatePosition()
    {
        switch (_stateControl.GetCurrentState())
        {
            case CharState.Freefalling:
                Fall();
                break;
            case CharState.Walking:
                Walk();
                break;
            default:
                break;
        }
    }


    void Fall()
    {
    }


    void Walk()
    {
        Quaternion lookRotation = Quaternion.Euler(lookAngles.x, lookAngles.y, 0);
        Vector3 heightAdjust = new Vector3(0, _surface.contactSeparation - 0.3f, 0) * 0.1f;
        Vector3 step = _surface.rotationToNormal * charYaw * stepInput * 0.075f;

        // APPLY VELOCITIES
        _charBody.AddForce(-Physics.gravity, ForceMode.Acceleration);
        _charBody.MovePosition( this.transform.position - heightAdjust + step );
        _charBody.velocity = Vector3.zero;

        // APPLY ROTATION
        Quaternion rotationAdjust = Quaternion.Lerp(transform.rotation, charYaw * Quaternion.identity, 0.2f);
        transform.rotation = rotationAdjust;
    }
}
