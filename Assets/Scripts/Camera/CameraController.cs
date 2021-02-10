using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharControl;


public class CameraController : MonoBehaviour
{
    Vector3 offset = new Vector3( 1.5f, 1.5f, -5.0f );

    private CharController _player;
    private CrosshairController cs;
    Camera thisCamera;

    void OnValidate()
    {
        Start();
    }
    void Start()
    {
        cs = new CrosshairController();
        thisCamera = this.gameObject.GetComponent<Camera>();
        _player = GameObject.Find("Player").GetComponent<CharController>();
    }

    void FixedUpdate()
    {
        this.transform.position = Vector3.Lerp(this.transform.position, _player.transform.position + _player.lookRotation * offset, 0.5f);
        this.transform.rotation = Quaternion.Slerp( this.transform.rotation, _player.lookRotation, 0.5f);
    }

    void OnGUI()
    {
        Vector3 viewportPoint = thisCamera.WorldToViewportPoint( _player.lookPoint );
        /*GUI.DrawTexture(    new Rect(   Screen.width * 0.5f - cs.cs_blue.width*0.5f,
                                        Screen.height * 0.5f - cs.cs_blue.height*0.5f,
                                        64, 
                                        64),
                            cs.cs_blue);*/
        
        Debug.Log(viewportPoint);

        GUI.DrawTexture(    new Rect(   Screen.width * viewportPoint.x - cs.cs_blue.width * 0.125f,
                                        Screen.height * viewportPoint.y - cs.cs_blue.height * 0.125f,
                                        32, 
                                        32),
                            cs.cs_blue);
    }
}
