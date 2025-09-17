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
await channel.QueueDeclareAsync(RabbitMQConfig.DeliveryQueue, durable: true, exclusive: false, autoDelete: false);
await channel.QueueBindAsync(RabbitMQConfig.DeliveryQueue, RabbitMQConfig.OrderExchange, RabbitMQConfig.ReadyRoutingKey);

var consumer = new AsyncEventingBasicConsumer(channel);
consumer.ReceivedAsync += async (s, e) =>
{
    var body = e.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    var order = JsonSerializer.Deserialize<Order>(message);

    if (order is not null)
    {
        Console.WriteLine($"[Delivery Serivce] Delivery received order: {order.Id}");

        // Simulate delivery time
        Thread.Sleep(2000);

        order.Status = OrderStatus.Delivered;
        Console.WriteLine($"[Delivery Serivce] Order delivered: {order.Id} to {order.CustomerName}");
    }
};

await channel.BasicConsumeAsync(
    queue: RabbitMQConfig.DeliveryQueue,
    autoAck: true,
    consumer: consumer);

Console.WriteLine("[Delivery Serivce] Service started. Waiting for ready orders...");
Console.ReadLine();