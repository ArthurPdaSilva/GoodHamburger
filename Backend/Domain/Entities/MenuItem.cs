using Domain.Enums;

namespace Domain.Entities
{
    public class MenuItem : BaseEntity
    {
        public required string Name { get; set; }
        public float Price { get; set; }
        public required MenuItemType Type { get; set; }
    }
}
