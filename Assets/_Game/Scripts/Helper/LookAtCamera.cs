using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    Camera mainCam;
    [SerializeField] private Transform _target;
    [SerializeField] private Vector3 _offSet;
    void Awake()
    {
        mainCam = Camera.main;
    }

    void LateUpdate()
    {
        //transform.LookAt(mainCam.transform);
        transform.localRotation = mainCam.transform.rotation;
        transform.position = _target.position + _offSet;


    }
}
