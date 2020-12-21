using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//[RequireComponent(typeof(Rigidbody))]
//[RequireComponent(typeof(Collider))]

namespace CharControl
{
    public enum CharState 
    {
        None = 0,
        Freefalling,
        Sliding,
        Walking,
        Jumping,
        Grappling,
        Flying
    }
    
    public struct InputState
    {
            public int forward;
            public int backward;
            public int left;
            public int right;
            public int jump;

            public float mouseDeltaX;
            public float mouseDeltaY;

            public float mousePositionX;
            public float mousePositionY;
    };

        
    public class CharController : MonoBehaviour
    {
        private SurfaceController _surface;
        private Rigidbody _charBody;
        private Collider _charCollider;
    
        InputState _inputs;


    
        void OnValidate()
        {
            Start();
        }
        void Start()
        {
            _charBody = gameObject.GetComponent<Rigidbody>();
            _charCollider = gameObject.GetComponent<Collider>();
            _surface = new SurfaceController(_charCollider);
        }


        public void UpdateInputs()
        {
            _inputs.forward = Input.GetKey(KeyCode.W) ? 1 : 0;
            _inputs.backward = Input.GetKey(KeyCode.S) ? 1 : 0;
            _inputs.left = Input.GetKey(KeyCode.A) ? 1 : 0;
            _inputs.right = Input.GetKey(KeyCode.D) ? 1 : 0;
            _inputs.jump = Input.GetKeyUp(KeyCode.Space) ? 1 : 0;

            _inputs.mouseDeltaX = Input.GetAxis("Mouse X");
            _inputs.mouseDeltaY = -Input.GetAxis("Mouse Y");

            _inputs.mousePositionX += _inputs.mouseDeltaX;
            _inputs.mousePositionY += _inputs.mouseDeltaY;
        }
    
        void Update()
        {
        }
    
        void FixedUpdate()
        {
        }
    
    
        public Quaternion GetLookRotation()
            {
                return Quaternion.Euler(_inputs.mousePositionY, _inputs.mousePositionX, 0);
            }
    }
}