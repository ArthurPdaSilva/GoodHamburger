using Application.DTOs.OrderItemDTOs;

namespace Application.DTOs.OrderDTOs
{
    public class OrderListDTO
    {
        public Guid Id { get; set; }
        public float SubTotal { get; set; }
        public float Total { get; set; }
        public int ItemsCount { get; set; }
        public IList<OrderItemListDTO> Items { get; set; } = new List<OrderItemListDTO>();
    }
}
