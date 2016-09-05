using EasyNetQ.FluentConfiguration;
using System;
using System.Reactive.Subjects;

namespace EasyNetQ.Rx
{
    public class ObservableBus<T> : IDisposable, IConnectableObservable<T> where T : class
    {
        IBus _bus;
        string _topicId;
        Action<ISubscriptionConfiguration> _configure;

        IDisposable _subsription;

        ISubject<T> _internal;

        public ObservableBus(IBus bus, string topicId, Action<ISubscriptionConfiguration> configure)
        {
            _bus = bus;
            _topicId = topicId;
            _configure = configure ?? new Action<ISubscriptionConfiguration>(c => { });

            _internal = new Subject<T>();

            _bus
                .Advanced
                .Disconnected += (error, t) => _internal.OnCompleted();
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return _internal
                .Subscribe(observer);
        }

        public IDisposable Connect()
        {
            _subsription = _bus
                .Subscribe<T>(_topicId, m => _internal.OnNext(m), _configure)
                .ConsumerCancellation;

            return this;
        }

        public void Dispose()
        {
            _internal.OnCompleted();
            _subsription.Dispose();
        }
    }
}
