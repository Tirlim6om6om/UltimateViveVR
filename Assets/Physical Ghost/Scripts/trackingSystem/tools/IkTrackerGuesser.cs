using System.Collections.Generic;
using Physical_Ghost.trackingSystem.data;
using Physical_Ghost.trackingSystem.targets.concrete;
using UnityEngine;

namespace Physical_Ghost.trackingSystem.tools {
    public class IkTrackerGuesser {
        public void GuessReceiverPoint(Transform[] children, out ReceiverTarget[] points) {
            List<ReceiverTarget> trackers = new();


            for (int i = 0; i < System.Enum.GetNames(typeof(RigIkType)).Length; i++) {
                RigIkType rigIkType = (RigIkType)i;
                AddTrackerOfKeys(rigIkType, trackers, children, IdByKey(rigIkType));
            }

            points = trackers.ToArray();
        }

        private void AddTrackerOfKeys(RigIkType rigIkType, List<ReceiverTarget> trackers, Transform[] tfChildren,
            string[] keys) {
            for (int i = 0; i < tfChildren.Length; i++) {
                var tfChild = tfChildren[i];
                if (EndsWithKey(tfChild, keys)) {
                    trackers.Add(new ReceiverTarget(rigIkType, tfChild));
                    break;
                }
            }
        }
        
        private bool EndsWithKey(Transform tf, string[] keys) {
            string tfName = tf.name;
            for (int i = 0; i < keys.Length; i++) {
                if (tfName.EndsWith(keys[i]) && tfName.Contains("IK"))
                    return true;
            }

            return false;
        }

        private string[] IdByKey(RigIkType ikType) => ikType switch {
            RigIkType.Spine => new[] { " Spine" },
            RigIkType.HandR => new[] { "R Hand", "Hand R" },
            RigIkType.HandL => new[] { "L Hand", "Hand L" },
            RigIkType.FootR => new[] { "R Foot", "Foot R" },
            RigIkType.FootL => new[] { "L Foot", "Foot L" },
            RigIkType.ShoulderR => new[] { "R Forearm", "Shoulder R" },
            RigIkType.ShoulderL => new[] { "L Forearm", "Shoulder L" },
            RigIkType.ThighR => new[] { "R Calvicle", " Knee R" },
            RigIkType.ThighL => new[] { "L Calvicle", " Knee L" },
            RigIkType.HeadViewpoint => new[] { " Head", "Head ViewPoint", "ViewPoint" },
            RigIkType.ElbowR => new[] { " Elbow R", "R Forearm" },
            RigIkType.ElbowL => new[] { " Elbow L", "L Forearm" },
            RigIkType.KneeR => new[] { " Knee R", "R Calf" },
            RigIkType.KneeL => new[] { " knee L", "L Calf" },
            _ => new string[0]
        };
    }
}