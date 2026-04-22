namespace Domain.Entities
{
    public class Order : BaseEntity
    {
        public IList<OrderItem> Items { get; set; } = new List<OrderItem>();
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
    }
}
