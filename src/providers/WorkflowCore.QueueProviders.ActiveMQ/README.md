# ActiveMQ provider for Workflow Core

Provides distributed worker support  on [Workflow Core](../../README.md) using [ActiveMQ](http://activemq.apache.org/).

This makes it possible to have a cluster of nodes processing your workflows, along with a distributed lock manager.

* ActiveMQ provider using Apache.NMS library and this library requires at least .Net Standart 2.0

## Installing

Install the NuGet package "WorkflowCore.QueueProviders.ActiveMQ"

```
PM> Install-Package WorkflowCore.QueueProviders.ActiveMQ -Pre
```

## Usage

Use the .ActiveMQ extension method when building your service provider.

```C#
services.AddWorkflow(x => x.UseActiveMQ(new ConnectionFactory() { HostName = "localhost" });

If you want to change Queue names you can set optional paramaters as you need (WorkflowQueueName,EventQueueName).

```