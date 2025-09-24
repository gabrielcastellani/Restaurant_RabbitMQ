using RabbitMQ.Client;
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

await channel.ExchangeDeclareAsync(RabbitMQConfig.Exchange, ExchangeType.Direct);
await channel.QueueDeclareAsync(RabbitMQConfig.OrderQueue, durable: true, exclusive: false, autoDelete: false);
await channel.QueueBindAsync(RabbitMQConfig.OrderQueue, RabbitMQConfig.Exchange, RabbitMQConfig.OrderRoutingKey);

Console.WriteLine("[Order Service] Service started. Press Enter to create orders...");

while (true)
{
    Console.ReadLine();

    var order = CreateSampleOrder();
    var message = JsonSerializer.Serialize(order);
    var body = Encoding.UTF8.GetBytes(message);

    await channel.BasicPublishAsync(
        exchange: RabbitMQConfig.Exchange,
        routingKey: RabbitMQConfig.OrderRoutingKey,
        body: body);

    Console.WriteLine($"[Order Service] Order placed: {order.Id} for {order.CustomerName}");
}

static Order CreateSampleOrder()
{
    var random = new Random();
    var dishes = new[] { "Pizza", "Burger", "Pasta", "Salad", "Steak" };

    return new Order
    {
        CustomerName = $"Customer_{random.Next(100)}",
        Items = new List<OrderItem>
        {
            new()
            {
                DishName = dishes[random.Next(dishes.Length)],
                Quantity = 1,
                Price = random.Next(10, 30),
            }
        }
    };
}