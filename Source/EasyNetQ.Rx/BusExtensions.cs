using System;
using EasyNetQ.FluentConfiguration;
using EasyNetQ.Rx;

namespace EasyNetQ
{
    public static class BusExtensions
    {
        public static ObservableTopic<T> ObservableTopic<T>(this IBus bus, string topicId) where T : class
        {
            return bus.ObservableTopic<T>(topicId, null);
        }

        public static ObservableTopic<T> ObservableTopic<T>(this IBus bus, string topicId, Action<ISubscriptionConfiguration> configure) where T : class
        {
            var topic = new ObservableTopic<T>();
            topic.InternalSubscription = configure != null ? bus.Subscribe<T>(topicId, topic.Next, configure) : bus.Subscribe<T>(topicId, topic.Next);
            return topic;
        }

        public static ObservableTopic<T> CompleteWhen<T>(this ObservableTopic<T> topic, Func<T, bool> completeWhen) where T : class
        {
            topic.CompleteWhen = completeWhen;
            return topic;
        }
    }
}
