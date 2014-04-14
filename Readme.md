![EasyNetQ.Rx Logo](https://raw.github.com/wiki/mikehadlow/EasyNetQ/images/logo_design_150.png)

#EasyNetQ.Rx 

EasyNetQ.Rx is an extension for enabling reactive subscriptions on EasyNetQ

##Usage

###To connect to a RabbitMQ broker...

```csharp
var bus = RabbitHutch.CreateBus("host=localhost");
```

###To subscribe to a message...

```csharp
bus.ObservableTopic<MyTestMessage>("my_subscription_id")
   .CompleteWhen(m => m.Value == 999)
   .Subscribe((x) => { max = x.Value; });
```

###Or

```csharp
bus.ObservableTopic<MyTestMessage>("my_subscription_id")
   .Where(x => x.Value < 5)
   .CompleteWhen(m => m.Value == 0)
   .Subscribe(x => Console.Write(x.Value));
```

###Or with aggregations (using System.Reactive.Linq)

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

###Buffering

```csharp
bus
    .ObservableTopic<Order>("new-orders-topic")
    .Where(order => order.Total > 100)
    .Buffer(10)
    .SelectMany(x => x)
    .Subscribe(x => Console.WriteLine("Order of total ${0} has arrived", x.Total));
```

## Install

    PM> Install-Package EasyNetQ.Rx
