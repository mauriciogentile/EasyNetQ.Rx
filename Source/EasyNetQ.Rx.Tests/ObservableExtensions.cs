using EasyNetQ.Tests.Mocking;
using System;
using RabbitMQ.Client.Framing;
using System.Reactive.Linq;

namespace EasyNetQ.Rx.Tests
{
    public static class ObservableExtensions
    {
        private const string TypeName = "EasyNetQ.Rx.Tests.MyTestMessage:EasyNetQ.Rx.Tests";
        const string CorrelationId = "the_correlation_id";
        const string ConsumerTag = "the_consumer_tag";
        const ulong DeliveryTag = 123;

        public static IObservable<int> Send(this IObservable<int> input, MockBuilder mockbuilder, JsonSerializer serializer)
        {
            return input
                .Do(value => mockbuilder
                    .Consumers
                    .ForEach(cons => cons
                        .HandleBasicDeliver(
                            ConsumerTag,
                            DeliveryTag,
                            false, // redelivered
                            TypeName,
                            "#",
                            new BasicProperties
                            {
                                Type = TypeName,
                                CorrelationId = CorrelationId
                            },
                            serializer.MessageToBytes(new MyTestMessage(value)))));
        }
    }
}
