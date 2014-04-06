using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using EasyNetQ.FluentConfiguration;

namespace EasyNetQ.Rx
{
    public static class BusExtensions
    {
        public static ObservableTopic<T> ObservableTopic<T>(this IBus bus, string topicId) where T : class
        {
            var topic = new ObservableTopic<T>();
            bus.Subscribe<T>(topicId, (message) =>
            {
                topic.Next(message);
            });
            return topic;
        }

        public static ObservableTopic<T> ObservableTopic<T>(this IBus bus, string topicId, Action<ISubscriptionConfiguration> configure) where T : class
        {
            var topic = new ObservableTopic<T>();
            bus.Subscribe<T>(topicId, (message) =>
            {
                topic.Next(message);
            }, configure);
            return topic;
        }

        public static ObservableTopic<T> CompleteWhen<T>(this ObservableTopic<T> topic, Func<T, bool> completeWhen) where T : class
        {
            topic.CompleteWhen = completeWhen;
            return topic;
        }
    }
}
