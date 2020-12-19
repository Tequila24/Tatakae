using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
        Quaternion rotation = _player.lookRotation;

        this.transform.position = _player.transform.position + rotation * offset;
        this.transform.rotation = rotation;
    }
}
