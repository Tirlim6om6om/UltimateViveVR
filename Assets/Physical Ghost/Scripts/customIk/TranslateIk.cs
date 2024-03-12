using UnityEngine;

namespace Physical_Ghost.Scripts.customIk
{
    public class TranslateIk : CustomIK
    {
        [Space]
        [Header("Elbow collider's root")]
        [SerializeField] private Transform translateElbow;

        protected override void UpdateSequence()
        {
            base.UpdateSequence();
            if (translateElbow && elbowBone)
                translateElbow.eulerAngles = elbowBone.eulerAngles;
        }
    }
}