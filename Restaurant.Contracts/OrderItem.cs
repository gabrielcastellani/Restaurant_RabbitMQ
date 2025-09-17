namespace Restaurant.Contracts
{
    public sealed class OrderItem
    {
        public string DishName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public double Price { get; set; }
    }
}
