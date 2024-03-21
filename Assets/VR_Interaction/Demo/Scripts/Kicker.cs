using Mirror;
using UnityEngine;

namespace BCS.CORE.VR.Network.Example
{
    public class Kicker : NetworkBehaviour
    {
        private Vector3 _lastpos;
        private Quaternion _lastrot;
        private float _power;

        void FixedUpdate()
        {
            _power = Mathf.Lerp(
                Vector3.Distance(_lastpos, transform.position) * 0.5f +
                Quaternion.Angle(_lastrot, transform.rotation) * .125f, _power, Time.fixedDeltaTime);
            _power -= .125f;
            _power *= 3;
            _lastrot = transform.rotation;
            _lastpos = transform.position;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.CompareTag("ball"))
            {
                Kick(other.gameObject, _power, transform.position);
            }
        }

        [Command (requiresAuthority = false)]
        public void Kick(GameObject other, float powerInput, Vector3 pos)
        {
            KickClients(other, powerInput, pos);
        }

        [ClientRpc]
        private void KickClients(GameObject other, float powerInput, Vector3 pos)
        {
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