using Physical_Ghost.trackingSystem.sources.interfaces;

namespace Physical_Ghost.trackingSystem.targets.interfaces
{
    /// <summary>
    /// Интерфейс распределения получаемых значений по целевым IK
    /// </summary>
    public interface IRigTarget
    {
        void AddSource(IKeyPositionSource source);
        void RemoveSource(IKeyPositionSource source);
        IIkPositionTarget[] receivers { get; }
    }
}