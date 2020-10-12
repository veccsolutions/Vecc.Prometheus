using System;

namespace Vecc.Prometheus.Internal
{
    public class CallbackDisposable : IDisposable
    {
        private readonly Action _callback;
        private bool _disposed = false;

        public CallbackDisposable(Action callback)
        {
            this._callback = callback;
        }

        public void Dispose()
        {
            if (!this._disposed)
            {
                this._callback();
                this._disposed = true;
            }
        }
    }
}