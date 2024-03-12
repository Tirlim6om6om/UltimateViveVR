using Physical_Ghost.trackingSystem.data;
using UnityEngine;

namespace Physical_Ghost.trackingSystem.sources.interfaces
{
    /// <summary>
    /// Источник позиционирования
    /// </summary>
    public interface IPositionSource
    {
        public Vector3 PositionOffset { get; }
        public Quaternion RotationOffset { get; }
        public event System.Action<Vector3> OnPositionChanged;
        public event System.Action<Quaternion> OnRotationChanged;
    }

    public interface IKeyPositionSource : IPositionSource
    {
        public RigIkType IkType { get; }
    }
}