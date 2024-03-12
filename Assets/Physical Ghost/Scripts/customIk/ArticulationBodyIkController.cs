using System.Collections.Generic;
using UnityEngine;

namespace Physical_Ghost.Scripts.customIk
{
    public class ArticulationBodyIkController : CustomIK
    {
        [Space]
        [Header("Controlled Joints")]
        [SerializeField] private ArticulationBody rootController;

        //[SerializeField] private ArticulationBody elbowController;


        protected override void UpdateSequence()
        {
            base.UpdateSequence();

            if (elbowBone && rootController && rootBone)
            {
                var shoulder = rootBone.localEulerAngles;
                var elbow = elbowBone.localEulerAngles;
                
                var rootAngle = transform.localEulerAngles;
                Debug.Log($"Root Angle: {rootAngle}");
                rootController.SetDriveTargets(new List<float>()
                {
                    0, UnityAngle(rootAngle.y), UnityAngle(rootAngle.z),
                    UnityAngle(shoulder.x), /*UnityAngle(shoulder.y), UnityAngle(shoulder.z)*/0, 0,
                    UnityAngle(elbow.x), //UnityAngle(elbow.y), UnityAngle(elbow.z), UnityAngle(elbow.x),
                    0, 0,
                });
            }
        }

        private static float UnityAngle(float angle)
        {
            var unityAngle = (angle > 90 ? angle - 360 : angle);
            Debug.Log($"Input Angle: {angle}, unity Angle: {unityAngle}");
            return unityAngle * Mathf.Deg2Rad;
        }
    }
}