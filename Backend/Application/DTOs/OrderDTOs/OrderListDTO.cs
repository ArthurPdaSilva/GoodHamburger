using Application.DTOs.OrderItemDTOs;

namespace Application.DTOs.OrderDTOs
{
    public class OrderListDTO
    {
        public Guid Id { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
        public int ItemsCount { get; set; }
        public IList<OrderItemListDTO> Items { get; set; } = new List<OrderItemListDTO>();
    }
}
