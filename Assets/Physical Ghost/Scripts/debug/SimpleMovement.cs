using UnityEngine;

namespace Physical_Ghost.Scripts.debug
{
    public class SimpleMovement : MonoBehaviour
    {
        [SerializeField] private float speed = 0.01f;
        [SerializeField] private float rotationSpeed = 1;

        [Header("Debug Features")]
        [SerializeField] private bool displayColliders = false;

        [SerializeField] private GameObject[] collidersDisplays;

        private void Start()
        {
            for (int i = 0; i < collidersDisplays.Length; i++)
            {
                if (!collidersDisplays[i])
                    continue;
                collidersDisplays[i].SetActive(displayColliders);
            }
        }

        private void Update()
        {
            Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * speed;

            if (Input.GetKeyDown(KeyCode.LeftShift))
                input *= 2;

            transform.Translate(input);

            float rotV = 0;
            if (Input.GetKey(KeyCode.Q))
                rotV = -1;
            if (Input.GetKey(KeyCode.E))
                rotV = 1;
            transform.Rotate(Vector3.up * rotationSpeed * rotV);
        }
    }
}