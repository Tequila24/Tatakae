using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharControl;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class CharController : MonoBehaviour
{
    private CharMoveController _moveControl;
    private SurfaceController _surface;
    private Rigidbody _charBody;
    private Collider _charCollider;
    private Vector3 step;
    private Vector2 stepDeltaMouse;
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
        _moveControl = new CharMoveController(_surface, _charBody, _charCollider);
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
        _moveControl.Process();
    }
}
