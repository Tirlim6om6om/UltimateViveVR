using Physical_Ghost.trackingSystem.targets.interfaces;
using Physical_Ghost.trackingSystem.tools;
using UnityEngine;

namespace Physical_Ghost.trackingSystem.targets.concrete
{
    public class RigReceiverCollection : RigReceiverMonoBase
    {
        [SerializeField] private ReceiverTarget[] receiverTargets;

        protected override void FindTargetReceivers()
        {
            Transform[] children = GetComponentsInChildren<Transform>();

            var guesser = new IkTrackerGuesser();
            guesser.GuessReceiverPoint(children, out receiverTargets);
        }

        protected override IIkPositionTarget[] Receivers()
        {
            IIkPositionTarget[] outp = new IIkPositionTarget[receiverTargets.Length];
            for (int i = 0; i < outp.Length; i++)
            {
                outp[i] = receiverTargets[i];
            }

            return outp;
        }
    }
}