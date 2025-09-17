namespace Restaurant.Abstracts
{
    public class RabbitMQConfig
    {
        public const string Uri = "amqps://user:pass@jackal.rmq.cloudamqp.com/user";

        public const string OrderExchange = "restaurant.orders";
        public const string OrderQueue = "orders.queue";
        public const string KitchenQueue = "kitchen.queue";
        public const string DeliveryQueue = "delivery.queue";


        public const string OrderRoutingKey = "order.placed";
        public const string KitchenRoutingKey = "order.cooking";
        public const string ReadyRoutingKey = "order.ready";
        public const string DeliveryRoutingKey = "order.delivered";
    }
}
