using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleArmController : MonoBehaviour
{
    private Camera _cam;
    private Transform _tf;

    void Awake()
    {
        _cam = Camera.main;
        _tf = transform;
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = _cam.ScreenPointToRay(Input.mousePosition);

        _tf.LookAt(ray.origin + ray.direction * 100);
    }
}