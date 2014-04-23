using System;

namespace EasyNetQ.Rx
{
    public abstract class Disposable : IDisposable
    {
        protected bool Disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            Disposed = true;
        }

        ~Disposable()
        {
            Dispose(false);
        }
    }
}
