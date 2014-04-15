using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyNetQ.Rx
{
    class Unsubscriber<T> : Disposable
    {
        private readonly IEnumerable<IObserver<T>> _observers;
        private readonly IObserver<T> _observer;

        public Unsubscriber(IEnumerable<IObserver<T>> observers, IObserver<T> observer)
        {
            _observers = observers;
            _observer = observer;
            ManagedDisposal = DisposeCallback;
        }

        void DisposeCallback()
        {
            if (_observer != null && _observers.Contains(_observer))
            {
                _observers.ToList().Remove(_observer);
            }
        }
    }
}