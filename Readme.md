![EasyNetQ.Rx Logo](https://raw.github.com/wiki/mikehadlow/EasyNetQ/images/logo_design_150.png)

[![Build Status](https://travis-ci.org/mauriciogentile/EasyNetQ.Rx.svg?branch=master)](https://travis-ci.org/mauriciogentile/EasyNetQ.Rx)

[![NuGet status](https://img.shields.io/nuget/v/EasyNetQ.Rx.png?maxAge=2592000)](https://www.nuget.org/packages/EasyNetQ.Rx)

#EasyNetQ.Rx 

EasyNetQ.Rx is an extension for enabling reactive subscriptions on EasyNetQ

##Usage

###To connect to a RabbitMQ broker...

```csharp
var bus = RabbitHutch
    .CreateBus("host=localhost");
```

###To subscribe to a message...

```csharp
var topic = bus
    .ToObservable<MyTestMessage>("my_topic_id");

topic
    .Subscribe((x) => { max = x.Value; });

topic
    .Connect();
```

###To stop a topic
```csharp
var topic = bus
    .ToObservable<MyTestMessage>("my_topic_id")
    .Subscribe(x => Console.Write(x.Value));

topic
    .Connect();

topic
    .Dispose();
```

###Or with aggregations

```csharp
var topic = bus
    .ToObservable<MyTestMessage>("my_topic_id");

var filtered = topic
    .Where(x => x.Value < 5);

filtered
   .Max(x => x.Value)
   .Subscribe(x => Console.Write("Max value is: " + x));

filtered
   .Min(x => x.Value)
   .Subscribe(x => Console.Write("Min value is: " + x));

filtered
   .Average(x => x.Value)
   .Subscribe(x => Console.Write("Avg value is: " + x));

topic
    .Connect();
```

###Buffering

```csharp
var topic = bus
    .ToObservable<Order>("new-orders-topic");

topic
    .Where(order => order.Total > 100)
    .Buffer(10)
    .SelectMany(x => x)
    .Subscribe(x => Console.WriteLine("Order of total ${0} has arrived", x.Total));

topic
    .Connect();
```

## Install

    PM> Install-Package EasyNetQ.Rx