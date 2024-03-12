using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Physical_Ghost.Scripts.debug
{
    public class MouseIkController : MonoBehaviour
    {
        [Range(0, 10)]
        [SerializeField] private float minDist = 3;

        [Range(0, 10)]
        [SerializeField] private float maxDist = 5;

        [SerializeField] private float scrollPower = 0.1f;
        [SerializeField] private float rayLength = 4f;

        [SerializeField] private int id;

        private static int _currentControllerId;
        private static int _controllerCount;

        private Camera _cam;
        private Transform _tf;

        private Transform _camTf;

        void Start()
        {
            id = _controllerCount++;

            _cam = Camera.main;
            _tf = transform;
            _camTf = _cam.transform;
        }


        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.E))
            {
                if (id == 0)
                {
                    _currentControllerId = (_currentControllerId + 1) % _controllerCount;
                    Debug.Log($"Current Controller id: {_currentControllerId}, count: {_controllerCount}");
                }
            }
        }

        void FixedUpdate()
        {
            if (_currentControllerId == id)
            {
                Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
                rayLength = Mathf.Clamp(rayLength + Input.mouseScrollDelta.y * scrollPower, minDist, maxDist);
                _tf.position = _camTf.position + ray.direction * rayLength;
            }
        }

        private void OnDestroy()
        {
        }
    }
}