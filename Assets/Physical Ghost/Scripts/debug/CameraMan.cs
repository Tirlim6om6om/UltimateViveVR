using UnityEngine;

namespace Physical_Ghost.debug {
    public class CameraMan : MonoBehaviour {
        [SerializeField] private float speed = 1;
        [SerializeField] private float camSpeed = 0.5f;
        private Transform _tf;
        private Camera _cam;
        private Transform _camTf;

        // Start is called before the first frame update
        void Start() {
            _tf = transform;
            _cam = GetComponentInChildren<Camera>();
            _camTf = _cam.transform;
        }

        // Update is called once per frame
        void Update() {
            _camTf.localEulerAngles += new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0) * camSpeed;
            Vector3 moveVector = Vector3.up * Input.GetAxis("Hover") + _camTf.right * Input.GetAxis("Horizontal") +
                                 _camTf.forward * Input.GetAxis("Vertical");
            float sp1 = speed;
            if (Input.GetKey(KeyCode.LeftShift))
                sp1 *= 2;
            _tf.Translate(moveVector * sp1);
        }
    }
}