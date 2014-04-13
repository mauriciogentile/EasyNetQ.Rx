using System;
using System.Collections.Generic;

namespace EasyNetQ.Rx
{
    public class ObservableTopic<T> : Disposable, IObservable<T>
    {
        readonly List<IObserver<T>> _observers;

        internal Func<T, bool> CompleteWhen { get; set; }
        internal IDisposable InternalSubscription { get; set; }

        public ObservableTopic()
        {
            _observers = new List<IObserver<T>>();
            OnDispose = DisposeCallback;
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            _observers.Add(observer);
            return new Unsubscriber<T>(_observers, observer);
        }

        public void Next(T message)
        {
            _observers.ForEach(x => x.OnNext(message));
            if (CompleteWhen != null && CompleteWhen.Invoke(message))
            {
                Complete();
            }
        }

        public void Complete()
        {
            foreach (var obs in _observers.ToArray())
            {
                if (_observers.Contains(obs))
                {
                    obs.OnCompleted();
                }
            }
        }

        void DisposeCallback()
        {
            _observers.Clear();

            if (InternalSubscription != null)
            {
                InternalSubscription.Dispose();
            }
        }
    }
}
