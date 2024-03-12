using System;
using Physical_Ghost.trackingSystem.data;
using Physical_Ghost.trackingSystem.sources.interfaces;
using UnityEngine;

namespace Physical_Ghost.trackingSystem.sources.concrete {
    /// <summary>
    /// Источник, которые передает позиции и вращение трансформа, к которому привязан
    /// </summary>
    [DisallowMultipleComponent]
    [Tooltip("Источник, которые передает позиции и вращение трансформа, к которому привязан")]
    public class MonoIkSource : MonoBehaviour, IKeyPositionSource {
        [SerializeField] private RigIkType ikType = RigIkType.Spine;
        [SerializeField] private Transform transformOffset;
        [SerializeField] private Vector3 positionOffset;
        [SerializeField] private Vector3 rotationOffset;

        private Transform _tf;
        private Transform _tfRoot;
        private IPositionsDataSources _aggregator;
        private RigIkType _oldType;

        public RigIkType IkType => ikType; 

        public Vector3 PositionOffset => positionOffset;
        public Quaternion RotationOffset => Quaternion.Euler(rotationOffset);
        public event Action<Vector3> OnPositionChanged;
        public event Action<Quaternion> OnRotationChanged;


        private void Awake() {
            _tf = transform;
            _tfRoot = _tf.root;

            _aggregator = _tfRoot.GetComponent<IPositionsDataSources>();
            if (ikType == RigIkType.HeadViewpoint)
                Debug.Log($"Head Source, try to attach to {_aggregator}");
            _aggregator?.Register(this);
            _oldType = ikType;
        }

        private void FixedUpdate() {
            if (_oldType != ikType) {
                _oldType = ikType;
                _aggregator.Replace(this, this);
            }


            Vector3 tfPosition = _tf.position - _tfRoot.position + PositionOffset;
            Quaternion tfRotation = _tf.rotation * RotationOffset;

            if (transformOffset) {
                tfPosition += transformOffset.position - _tf.position;
                tfRotation *= _tf.rotation;
            }

            OnPositionChanged?.Invoke(tfPosition);
            OnRotationChanged?.Invoke(tfRotation);
        }

        private void OnDestroy() {
            _aggregator.Unregister(this);
        }
    }
}