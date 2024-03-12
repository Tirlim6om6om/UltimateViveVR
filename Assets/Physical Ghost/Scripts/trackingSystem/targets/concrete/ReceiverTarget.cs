using Physical_Ghost.trackingSystem.data;
using Physical_Ghost.trackingSystem.targets.interfaces;
using UnityEngine;

namespace Physical_Ghost.trackingSystem.targets.concrete {
    [System.Serializable]
    public struct ReceiverTarget : IIkPositionTarget {
        public RigIkType ikType;
        public Transform transform;

        public ReceiverTarget(RigIkType ikType, Transform transform) {
            this.ikType = ikType;
            this.transform = transform;
        }

        public void ApplyPosition(Vector3 position) {
            transform.localPosition = position;
        }

        public void ApplyRotation(Quaternion rotation) {
            transform.localRotation = rotation;
        }

        public bool SameType(RigIkType targetType) {
            return targetType == ikType;
        }
    }
}