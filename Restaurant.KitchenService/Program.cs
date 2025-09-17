using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Restaurant.Abstracts;
using Restaurant.Contracts;
using System.Text;
using System.Text.Json;

var factory = new ConnectionFactory
{
    Uri = new Uri(RabbitMQConfig.Uri)
};

using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

await channel.ExchangeDeclareAsync(RabbitMQConfig.OrderExchange, ExchangeType.Direct);
await channel.QueueDeclareAsync(RabbitMQConfig.KitchenQueue, durable: true, exclusive: false, autoDelete: false);
await channel.QueueBindAsync(RabbitMQConfig.KitchenQueue, RabbitMQConfig.OrderExchange, RabbitMQConfig.OrderRoutingKey);

var consumer = new AsyncEventingBasicConsumer(channel);
consumer.ReceivedAsync += async (s, e) =>
{
    var body = e.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    var order = JsonSerializer.Deserialize<Order>(message);

    if (order is not null)
    {
        Console.WriteLine($"[Kitchen Service] Order received: {order.Id}");
        order.Status = OrderStatus.Cooking;

        Thread.Sleep(3000);

        order.Status = OrderStatus.Ready;
        Console.WriteLine($"[Kitchen Service] Order ready: {order.Id}");

        // Send to delivery queue
        var readyMessage = JsonSerializer.Serialize(order);
        var readyBody = Encoding.UTF8.GetBytes(readyMessage);

        await channel.BasicPublishAsync(
            exchange: RabbitMQConfig.OrderExchange,
            routingKey: RabbitMQConfig.ReadyRoutingKey,
            body: readyBody);
    }
};

await channel.BasicConsumeAsync(queue: RabbitMQConfig.KitchenQueue, autoAck: true, consumer: consumer);

Console.WriteLine("[Kitchen Service] Service started. Waiting for orders...");
Console.ReadLine();