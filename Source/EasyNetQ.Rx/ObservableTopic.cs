using System;
using System.Collections.Generic;

namespace EasyNetQ.Rx
{
    public class ObservableTopic<T> : IObservable<T>
    {
        readonly List<IObserver<T>> _observers;

        internal Func<T, bool> CompleteWhen { get; set; }

        public ObservableTopic()
        {
            _observers = new List<IObserver<T>>();
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
            };
        }
    }

    class Unsubscriber<T> : IDisposable
    {
        private readonly List<IObserver<T>> _observers;
        private readonly IObserver<T> _observer;

        public Unsubscriber(List<IObserver<T>> observers, IObserver<T> observer)
        {
            _observers = observers;
            _observer = observer;
        }

        public void Dispose()
        {
            if (_observer != null && _observers.Contains(_observer))
                _observers.Remove(_observer);
        }
    }
}
