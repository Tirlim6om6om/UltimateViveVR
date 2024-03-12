using System;
using System.Collections.Generic;
using Physical_Ghost.trackingSystem.data;
using Physical_Ghost.trackingSystem.sources.interfaces;
using Physical_Ghost.trackingSystem.targets.interfaces;
using Physical_Ghost.trackingSystem.tools;
using UnityEngine;

namespace Physical_Ghost.trackingSystem.targets.concrete {
    public abstract class RigReceiverMonoBase : MonoBehaviour, IRigTarget {
        public IIkPositionTarget[] receivers => _receivers ??= Receivers();

        private Dictionary<IKeyPositionSource, IDisposable> _disposables = new();
        private IIkPositionTarget[] _receivers;
        
        public void AddSource(IKeyPositionSource source) {
            for (int i = 0; i < receivers.Length; i++) {
                IIkPositionTarget receiver = receivers[i];
                if (receiver.SameType(source.IkType)) {
                    source.OnPositionChanged += receiver.ApplyPosition;
                    source.OnRotationChanged += receiver.ApplyRotation;

                    IDisposable disposer = new Disposer(() => {
                        source.OnPositionChanged -= receiver.ApplyPosition;
                        source.OnRotationChanged -= receiver.ApplyRotation;
                    });

                    _disposables[source] = disposer;

                    break;
                }
            }
        }
        
        public void RemoveSource(IKeyPositionSource source) {
            if (_disposables.ContainsKey(source))
                _disposables[source].Dispose();
        }

        [ContextMenu("Try Find IK Targets")]
        public void TryFindTargets() {
            FindTargetReceivers();
        }

        protected abstract void FindTargetReceivers();
        protected abstract IIkPositionTarget[] Receivers();

        private void OnDestroy() {
            foreach (var d in _disposables) {
                d.Value.Dispose();
            }

            _disposables.Clear();
        }
    }
}