namespace Restaurant.Contracts
{
    public sealed class Order
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string CustomerName { get; set; } = string.Empty;
        public List<OrderItem> Items { get; set; } = new();
        public OrderStatus Status { get; set; } = OrderStatus.Placed;
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
    }
}
