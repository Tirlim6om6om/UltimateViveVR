using System;
using System.Collections.Generic;
using Physical_Ghost.Scripts.trackingSystem.sources.interfaces;
using Physical_Ghost.trackingSystem.sources.interfaces;
using Physical_Ghost.trackingSystem.targets.concrete;
using Physical_Ghost.trackingSystem.targets.interfaces;
using Physical_Ghost.trackingSystem.tools;
using UnityEngine;

namespace Physical_Ghost.trackingSystem.sources.concrete
{
    public class RigWatchMono : MonoBehaviour, IPositionsSourceObserver
    {
        /// <summary> Этот класс служит общим местом регистрации и понимает какой источник позишенов перенаправить к какому из получателей позишенов </summary>
        [SerializeField] private RigReceiverMonoBase[] receivers;

        private IRigTarget[] _receivers;
        private IRigTarget[] receiverAggregators => _receivers ??= GetReceivers();

        private IRigTarget[] GetReceivers() {
            IRigTarget[] outp = new IRigTarget[receivers.Length];
            for (int i = 0; i < receivers.Length; i++) {
                outp[i] = receivers[i];
            }

            return outp;
        }

        private Dictionary<IKeyPositionSource, IDisposable> _disposables = new();

        private List<IKeyPositionSource> _sources = new();
        


        private void AddSource(IKeyPositionSource source)
        {
            _sources.Add(source);
            for (int i = 0; i < receiverAggregators.Length; i++)
            {
                receiverAggregators[i]?.AddSource(source);
            }
        }

        private void RemoveSource(IKeyPositionSource source)
        {
            _sources.Remove(source);
            for (int i = 0; i < receiverAggregators.Length; i++)
            {
                receiverAggregators[i]?.RemoveSource(source);
            }

            if (_disposables.ContainsKey(source))
                _disposables[source].Dispose();
        }

        private void OnChangeSources(IKeyPositionSource removed, IKeyPositionSource added)
        {
            if (removed != null) RemoveSource(removed);
            if (added != null) AddSource(added);
        }

        public IDisposable RegisterSource(IPositionsDataSources source)
        {
            foreach (var s in source.positionSources)
                AddSource(s);
            source.onPositionSourceChanged += OnChangeSources;
            return new Disposer(() => source.onPositionSourceChanged -= OnChangeSources);
        }
    }
}