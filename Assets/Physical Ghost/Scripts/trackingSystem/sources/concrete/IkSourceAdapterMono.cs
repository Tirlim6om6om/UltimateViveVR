using System;
using System.Collections.Generic;
using Physical_Ghost.trackingSystem.data;
using Physical_Ghost.trackingSystem.sources.interfaces;
using UnityEngine;

namespace Physical_Ghost.trackingSystem.sources.concrete
{
    public class IkSourceAdapterMono : MonoBehaviour, IPositionsDataSources
    {
        [SerializeField] private RigWatchMono watcher;
        public IEnumerable<IKeyPositionSource> positionSources => _sources;

        public event Action<IKeyPositionSource, IKeyPositionSource> onPositionSourceChanged;
        private List<IKeyPositionSource> _sources = new();
        private List<IDisposable> _disposers = new();

        private void Awake()
        {
            _disposers.Add(watcher.RegisterSource(this)) ;
        }

        public void Register(IKeyPositionSource source)
        {
            
            if (!_sources.Contains(source))
            {
               
                _sources.Add(source);
                if(source.IkType== RigIkType.HeadViewpoint)
                    Debug.Log($"Head Source, has head: {_sources.Contains(source)}");
                onPositionSourceChanged?.Invoke(null, source);
            }
        }

        public void Unregister(IKeyPositionSource source)
        {
            if (_sources.Contains(source))
            {
                _sources.Remove(source);
                onPositionSourceChanged?.Invoke(source, null);
            }
        }

        public void Replace(IKeyPositionSource oldSource, IKeyPositionSource newSource)
        {
            if (_sources.Contains(oldSource))
                _sources.Remove(oldSource);

            if (!_sources.Contains(newSource))
                _sources.Add(newSource);

            onPositionSourceChanged?.Invoke(oldSource, newSource);
        }

        private void OnDestroy()
        {
            for (int i = 0; i < _disposers.Count; i++)
            {
                _disposers[i].Dispose();
            }
        }
    }
}