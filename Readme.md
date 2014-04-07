![EasyNetQ.Rx Logo](https://raw.github.com/wiki/mikehadlow/EasyNetQ/images/logo_design_150.png)

EasyNetQ.Rx is an extension for enabling reactive subscriptions on EasyNetQ

### Usage

To connect to a RabbitMQ broker...

```csharp
var bus = RabbitHutch.CreateBus("host=localhost");
```

To subscribe to a message...

```csharp
bus.ObservableTopic<MyTestMessage>("my_subscription_id")
   .CompleteWhen(m => m.Value == 999)
   .Subscribe((x) => { max = x.Value; }, () => resetEvent.Set());
```

Or

```csharp
bus.ObservableTopic<MyTestMessage>("my_subscription_id")
   .Where(x => x.Value < 5)
   .CompleteWhen(m => m.Value == 0)
   .Subscribe(x => Console.Write(x.Value));
```

Or with aggregations (System.Reactive.Linq)

```csharp
var topic = bus.ObservableTopic<MyTestMessage>("my_subscription_id")
   .Where(x => x.Value < 5)
   .CompleteWhen(m => m.Value == 0);

topic
   .Max(x => x.Value)
   .Subscribe(x => Console.Write("Max value is: " + x));

topic
   .Min(x => x.Value)
   .Subscribe(x => Console.Write("Min value is: " + x));

topic
   .Average(x => x.Value)
   .Subscribe(x => Console.Write("Avg value is: " + x));
```

### Install

    PM> Install-Package EasyNetQ.Rx