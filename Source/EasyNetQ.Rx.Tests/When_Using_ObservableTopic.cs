using EasyNetQ.Tests;
using EasyNetQ.Tests.Mocking;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Reactive.Linq;

namespace EasyNetQ.Rx.Tests
{
    [TestFixture]
    public class When_Using_ObservableTopic
    {
        const string ConsumerTag = "the_consumer_tag";
        const string SubscriptionId = "the_subscription_id";

        MockBuilder _mockBuilder;
        JsonSerializer _serializer;

        [SetUp]
        public void SetUp()
        {
            _mockBuilder = new MockBuilder(x => x.Register<IConventions>(_ => new Conventions(new TypeNameSerializer())
            {
                ConsumerTagConvention = () => ConsumerTag
            }));

            _serializer = new JsonSerializer(new TypeNameSerializer());
        }

        [Test]
        public void Should_Be_Capable_Of_Using_Where()
        {
            var bus = _mockBuilder
                .Bus
                .ToObservable<MyTestMessage>(SubscriptionId);

            var connection = bus
                .Connect();

            var result = bus
                .Take(10)
                .Where(x => x.Value < 5)
                .Count()
                .TakeUntil(Observable.Timer(TimeSpan.FromSeconds(2)))
                .Merge(Observable
                    .Range(0, 10)
                    .Send(_mockBuilder, _serializer)
                    .IgnoreElements())
                .Wait();

            connection.Dispose();

            result
                .ShouldEqual(5);
        }

        [Test]
        public void Should_Call_OnComplete_When_Bus_Closes()
        {
            var bus = _mockBuilder
               .Bus
               .ToObservable<MyTestMessage>(SubscriptionId);

            var connection = bus
                .Connect();

            var result = bus
                .Count()
                .Merge(Observable
                    .Range(0, 1)
                    .Do(x => connection.Dispose())
                    .Send(_mockBuilder, _serializer)
                    .IgnoreElements())
                .TakeUntil(Observable.Timer(TimeSpan.FromSeconds(2)))
                .Wait();

            result
                .ShouldEqual(0);
        }

        [Test]
        public void Should_Be_Capable_Of_Using_Aggregations()
        {
            var bus = _mockBuilder
               .Bus
               .ToObservable<MyTestMessage>(SubscriptionId);

            var connection = bus
                .Connect();

            var input = bus
                .Take(10)
                .TakeUntil(Observable.Timer(TimeSpan.FromSeconds(2)));

            var result = input
                .Max(x => x.Value)
                .Merge(Observable
                    .Range(0, 10)
                    .Send(_mockBuilder, _serializer)
                    .IgnoreElements())
                .Zip(
                    input.Min(x => x.Value),
                    input.Average(x => x.Value),
                    input.Sum(x => x.Value), (max, min, average, sum) => new { Max = max, Min = min, Average = average, Sum = sum })
                .Wait();

            connection.Dispose();

            result.Max.ShouldEqual(9);
            result.Min.ShouldEqual(0);
            result.Average.ShouldEqual(4.5);
            result.Sum.ShouldEqual(45);
        }
    }
}
