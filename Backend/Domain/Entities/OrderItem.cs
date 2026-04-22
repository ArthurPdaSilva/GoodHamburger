using Domain.Enums;

namespace Domain.Entities
{
    public class OrderItem : BaseEntity
    {
        public Guid OrderId { get; set; }
        public required Order Order { get; set; }
        public Guid MenuItemId { get; set; }
        public required MenuItemType Type { get; set; }
        public required string Name { get; set; }
        public decimal Price { get; set; }
    }
}
