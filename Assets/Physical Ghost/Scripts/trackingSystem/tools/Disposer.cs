using System;

namespace Physical_Ghost.trackingSystem.tools
{
    public class Disposer : IDisposable
    {
        private Action onDispose;

        public Disposer(Action onDispose) => this.onDispose = onDispose;

        public void Dispose()
        {
            var dispose = this.onDispose;
            onDispose = null;
            dispose?.Invoke();
        }
    }
}