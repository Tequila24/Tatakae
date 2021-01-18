using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharControl;


public class CameraController : MonoBehaviour
{
    private CharController _player;

    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<CharController>();
    }

    void FixedUpdate()
    {
        Vector3 offset = new Vector3( 1.5f, 1.5f, -5.0f );
        Quaternion rotation = _player.GetLookRotation();

        this.transform.position = Vector3.Lerp(this.transform.position, _player.transform.position + rotation * offset, 0.5f);
        this.transform.rotation = rotation;
    }
}
