namespace Domain.Entities
{
    public class Order : BaseEntity
    {
        public IList<OrderItem> Items { get; set; } = new List<OrderItem>();
        public float SubTotal { get; set; }
        public float Total { get; set; }
    }
}
