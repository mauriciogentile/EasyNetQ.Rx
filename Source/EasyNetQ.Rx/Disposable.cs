using System;

namespace EasyNetQ.Rx
{
    public abstract class Disposable : IDisposable
    {
        bool _disposed;

        protected Action ManagedDisposal;
        protected Action UnmanagedDisposal;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing && ManagedDisposal != null)
            {
                ManagedDisposal();
            }

            if (UnmanagedDisposal != null)
            {
                UnmanagedDisposal();
            }

            _disposed = true;
        }

        ~Disposable()
        {
            Dispose(false);
        }
    }
}
