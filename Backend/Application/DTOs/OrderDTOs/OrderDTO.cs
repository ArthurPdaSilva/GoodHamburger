using Application.DTOs.OrderItemDTOs;

namespace Application.DTOs.OrderDTOs
{
    public class OrderDTO
    {
        public Guid Id { get; set; }
        public float SubTotal { get; set; }
        public float Total { get; set; }
        public IList<OrderItemDTO> Items { get; set; } = new List<OrderItemDTO>();
    }
}
