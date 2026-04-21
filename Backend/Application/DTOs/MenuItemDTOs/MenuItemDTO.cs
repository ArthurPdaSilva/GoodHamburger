using Domain.Enums;

namespace Application.DTOs.MenuItemDTOs
{
    public class MenuItemDTO
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public float Price { get; set; }
        public required MenuItemType Type { get; set; }
    }
}
