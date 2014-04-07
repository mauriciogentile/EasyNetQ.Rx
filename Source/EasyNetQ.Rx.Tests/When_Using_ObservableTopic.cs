using System;
using System.Reactive.Linq;
using EasyNetQ.Tests.Mocking;
using NUnit.Framework;
using EasyNetQ.Tests;
using RabbitMQ.Client.Framing.v0_9_1;
using System.Threading;
using System.Threading.Tasks;

namespace EasyNetQ.Rx.Tests
{
    [TestFixture]
    public class When_Using_ObservableTopic
    {
        MockBuilder _mockBuilder;
        private const string TypeName = "EasyNetQ.Rx.Tests.MyTestMessage:EasyNetQ.Rx.Tests";
        const string SubscriptionId = "the_subscription_id";
        const string CorrelationId = "the_correlation_id";
        const string ConsumerTag = "the_consumer_tag";
        const ulong DeliveryTag = 123;

        void SendMessages(string testMessage, int number = 1)
        {
            for (int i = 0; i < number; i++)
            {
                var body = new JsonSerializer(new TypeNameSerializer())
                    .MessageToBytes(new MyTestMessage
                    {
                        Text = testMessage,
                        Value = i
                    });

                // deliver a message
                _mockBuilder.Consumers.ForEach((cons) => cons.HandleBasicDeliver(
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
                    body));
            }
        }

        [SetUp]
        public void SetUp()
        {
            var conventions = new Conventions(new TypeNameSerializer())
            {
                ConsumerTagConvention = () => ConsumerTag
            };

            _mockBuilder = new MockBuilder(x => x.Register<IConventions>(_ => conventions));
        }

        [Test]
        public void Should_Be_Capable_Of_Using_Where()
        {
            const string testMessage = "Hola!";

            int received = 0;

            _mockBuilder.Bus
                .ObservableTopic<MyTestMessage>(SubscriptionId)
                .Where(x => x.Value < 5)
                .Subscribe((x) =>
                {
                    received++;
                });

            var resetEvent = new AutoResetEvent(false);

            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
                resetEvent.Set();
            });

            SendMessages(testMessage, 10);

            resetEvent.WaitOne(TimeSpan.FromSeconds(2));

            received.ShouldEqual(5);
        }

        [Test]
        public void Should_Call_OnComplete_When_Provider_Is_Done()
        {
            const string testMessage = "Hola!";

            int max = 0;

            var resetEvent = new AutoResetEvent(false);

            _mockBuilder.Bus
                .ObservableTopic<MyTestMessage>(SubscriptionId)
                .CompleteWhen(m => m.Value == 999)
                .Subscribe((x) => { max = x.Value; }, () => resetEvent.Set());

            SendMessages(testMessage, 1000);

            resetEvent.WaitOne();

            max.ShouldEqual(999);
        }

        [Test]
        public void Should_Be_Capable_Of_Using_Aggregations()
        {
            const string testMessage = "Hola!";

            int max = 0, sum = 0, min = 0;
            double avg = 0;

            var resetEvent = new AutoResetEvent(false);

            var topicStream = _mockBuilder.Bus
                .ObservableTopic<MyTestMessage>(SubscriptionId)
                .CompleteWhen(m => m.Value == 9);

            topicStream.Max(x => x.Value).Subscribe((x) => { max = x; });
            topicStream.Min(x => x.Value).Subscribe((x) => { min = x; });
            topicStream.Average(x => x.Value).Subscribe((x) => { avg = x; });
            topicStream.Sum(x => x.Value).Subscribe((x) => { sum = x; }, () => resetEvent.Set());

            SendMessages(testMessage, 10);

            resetEvent.WaitOne();

            max.ShouldEqual(9);
            min.ShouldEqual(0);
            avg.ShouldEqual(4.5);
            sum.ShouldEqual(45);
        }
    }
}
