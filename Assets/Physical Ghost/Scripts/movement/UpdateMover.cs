using UnityEngine;

namespace Physical_Ghost.movement {
    public class UpdateMover : MonoBehaviour {
        [SerializeField] private float moveSpeed = 0.02f;
        [SerializeField] private float returnTimer = 3;
        private Transform _tf;
        private float _returnTs;
        private Vector3 _startPos;

        // Start is called before the first frame update
        void Awake() {
            _tf = transform;
            _startPos = _tf.position;
            _returnTs = returnTimer + Time.time;
        }

        // Update is called once per frame
        void FixedUpdate() {
            if (Time.time > _returnTs) {
                _tf.position = _startPos;
                _returnTs = returnTimer + Time.time;
            }

            _tf.position += _tf.forward * moveSpeed;
        }
    }
}