using Physical_Ghost.trackingSystem.data;
using UnityEngine;

namespace Physical_Ghost.trackingSystem.targets.interfaces
{
    /// <summary>
    /// Интерфейс применения значений к целевым IK
    /// </summary>
    public interface IIkPositionTarget
    {
        public void ApplyPosition(UnityEngine.Vector3 position);
        public void ApplyRotation(UnityEngine.Quaternion rotation);
        public bool SameType(RigIkType targetType);
    }
}