using System;
using System.Collections.Generic;

namespace Physical_Ghost.trackingSystem.sources.interfaces
{
    /// <summary>
    /// Аггрегация всех источнкиов позиционирования
    /// </summary>
    public interface IPositionsDataSources
    {
        IEnumerable<IKeyPositionSource> positionSources { get; }
        /// <summary>
        /// First member - old source (remove), second member - new source (add)
        /// </summary>
        event Action<IKeyPositionSource, IKeyPositionSource> onPositionSourceChanged;

        public void Register(IKeyPositionSource source);
        public void Unregister(IKeyPositionSource source);
        public void Replace(IKeyPositionSource oldSource, IKeyPositionSource newSource);
    }
}