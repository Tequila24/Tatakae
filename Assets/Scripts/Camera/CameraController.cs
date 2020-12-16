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
        Quaternion rotation = Quaternion.Euler(_player.lookAngles.x, _player.lookAngles.y, 0);

        this.transform.position = _player.transform.position + rotation * offset;
        this.transform.rotation = rotation;
    }
}
