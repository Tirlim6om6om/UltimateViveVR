using System;
using Physical_Ghost.trackingSystem.sources.interfaces;

namespace Physical_Ghost.Scripts.trackingSystem.sources.interfaces
{
    public interface IPositionsSourceObserver
    {
        IDisposable RegisterSource(IPositionsDataSources source);
    }
}