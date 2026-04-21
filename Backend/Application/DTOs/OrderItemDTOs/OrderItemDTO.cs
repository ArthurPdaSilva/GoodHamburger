using Domain.Enums;

namespace Application.DTOs.OrderItemDTOs
{
    public class OrderItemDTO
    {
        public Guid Id { get; set; }
        public Guid MenuItemId { get; set; }
        public required MenuItemType Type { get; set; }
        public required string Name { get; set; }
        public float Price { get; set; }
    }
}
