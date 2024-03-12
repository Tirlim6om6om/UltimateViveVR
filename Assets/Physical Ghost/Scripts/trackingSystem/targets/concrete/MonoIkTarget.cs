using Physical_Ghost.trackingSystem.data;
using Physical_Ghost.trackingSystem.targets.interfaces;
using UnityEngine;

namespace Physical_Ghost.trackingSystem.targets.concrete
{
    /// <summary>
    /// Объект, который принимает и изменяет позиции и вращение трансформа, к которому привязан
    /// </summary>
    [DisallowMultipleComponent]
    public class MonoIKTarget : MonoBehaviour, IIkPositionTarget
    {
        [SerializeField] private RigIkType ikType;
        private Transform _tf;


        private void Awake()
        {
            _tf = transform;
        }

        public void ApplyPosition(Vector3 position)
        {
            _tf.localPosition = position;
        }

        public void ApplyRotation(Quaternion rotation)
        {
            _tf.rotation = rotation;
        }


        public bool SameType(RigIkType targetType)
        {
            return targetType == ikType;
        }
    }
}