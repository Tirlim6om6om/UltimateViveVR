using System;
using UnityEngine;

namespace Physical_Ghost.Scripts.debug {
    public class SlowMo : MonoBehaviour {
        [Range(0.01f, 1f)]
        [SerializeField] private float timeSpeed = 1;
        private float _oldTimeSpeed = 1;

        private void Update() {
            if (!Mathf.Approximately(timeSpeed, _oldTimeSpeed)) {
                Time.timeScale = timeSpeed;
                _oldTimeSpeed = timeSpeed;
            }
        }
    }
}