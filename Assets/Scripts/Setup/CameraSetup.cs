using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraSetup : MonoBehaviour
{
    CinemachineVirtualCamera _cam;

    void Start()
    {
        _cam = GetComponent<CinemachineVirtualCamera>();
    }
    void Update()
    {
        if (_cam.Follow == null || _cam.Follow.gameObject.tag != "Player")
        {
            GameObject _player = GameObject.FindWithTag("Player");
            _cam.Follow = _player.transform;
        }
    }
}
