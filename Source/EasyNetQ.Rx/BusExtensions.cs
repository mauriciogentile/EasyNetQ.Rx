using System;
using EasyNetQ.FluentConfiguration;
using System.Reactive.Subjects;

namespace EasyNetQ.Rx
{
    public static class BusExtensions
    {
        public static IConnectableObservable<T> ToObservable<T>(this IBus bus, string topicId = null, Action<ISubscriptionConfiguration> configure = null) where T : class
        {
            return new ObservableBus<T>(bus, topicId, configure);
        }
    }
}
