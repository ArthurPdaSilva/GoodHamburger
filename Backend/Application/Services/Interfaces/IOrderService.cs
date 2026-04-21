using Application.DTOs.OrderDTOs;

namespace Application.Services.Interfaces
{
    public interface IOrderService 
    {
        Task CreateAsync(OrderDTO dto);

        Task UpdateAsync(Guid id, OrderDTO dto);

        Task DeleteAsync(Guid id);

        Task<IList<OrderListDTO>> GetAllAsync();
        Task<OrderDTO> GetByIdAsync(Guid id);
    }
}
