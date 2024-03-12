using Physical_Ghost.trackingSystem.data;
using Physical_Ghost.trackingSystem.targets.interfaces;
using UnityEngine;

namespace Physical_Ghost.trackingSystem.targets.concrete
{
    public class RigReceiverMono : RigReceiverMonoBase
    {
        [SerializeField] private MonoIKTgt[] monoReceivers;
        protected override void FindTargetReceivers()
        {
            monoReceivers = GetComponentsInChildren<MonoIKTgt>();
        }

        protected override IIkPositionTarget[] Receivers()
        {
            IIkPositionTarget[] outp = new IIkPositionTarget[monoReceivers.Length];
            for (int i = 0; i < outp.Length; i++)
            {
                outp[i] = monoReceivers[i];
            }

            return outp;
        }
    }
}