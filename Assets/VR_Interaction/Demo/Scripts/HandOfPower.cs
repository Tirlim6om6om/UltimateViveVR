using Mirror;
using UnityEngine;

namespace BCS.CORE.VR.Network.Example
{
    public class HandOfPower : NetworkBehaviour
    {
        private Vector3 lastpos;
        private Quaternion lastrot;
        private float power;

        // Update is called once per frame
        void Update()
        {
            power = Mathf.Lerp(
                Vector3.Distance(lastpos, transform.position) * 1 +
                Quaternion.Angle(lastrot, transform.rotation) * .25f, power, Time.deltaTime);
            power -= .25f;
            power *= 3;
            lastrot = transform.rotation;
            lastpos = transform.position;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.CompareTag("ball"))
            {
                Kick(other.gameObject, power, transform.position);
            }
        }

        [Command]
        public void Kick(GameObject other, float powerInput, Vector3 pos)
        {
            DebugVR.Log("Kick -> " + powerInput + " from " + pos);
            if (powerInput > 0)
            {
                other.transform.GetComponent<Rigidbody>()
                    .AddExplosionForce(powerInput, pos, 1, .175f, ForceMode.Impulse);
                Vector3 vel = Quaternion.EulerAngles(pos - other.transform.position) * new Vector3(0, 90, 0) *
                              powerInput * 2;
                other.transform.GetComponent<Rigidbody>().angularVelocity = vel;
            }
            else
            {
                other.transform.GetComponent<Rigidbody>().velocity *= -.1f;
            }
        }
    }
}