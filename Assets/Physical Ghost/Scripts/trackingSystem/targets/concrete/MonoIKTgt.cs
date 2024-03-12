using Physical_Ghost.trackingSystem.data;
using Physical_Ghost.trackingSystem.targets.interfaces;
using UnityEngine;

namespace Physical_Ghost.trackingSystem.targets.concrete
{
    /// <summary>
    /// Объект, который принимает и изменяет позиции и вращение трансформа, к которому привязан
    /// </summary>
    [DisallowMultipleComponent]
    public class MonoIKTgt : MonoBehaviour, IIkPositionTarget       
    //^--- Почему MonoIKTgt а не MonoIKTarget - юнити не детектит MonoIKTarget, хотя имя файла и название скрипта совпадают
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
            _tf.localRotation = rotation;
        }


        public bool SameType(RigIkType targetType)
        {
            return targetType == ikType;
        }
    }
}