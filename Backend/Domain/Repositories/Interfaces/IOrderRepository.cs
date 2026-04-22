using Domain.Entities;

namespace Domain.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        public Task CreateAsync(Order entity);

        public Task UpdateAsync(Order entity);

        public Task ReplaceItemsAsync(Guid orderId, IList<OrderItem> newItems);

        public Task<Order?> GetByIdAsync(Guid id);
        public Task<IList<Order>> GetAllAsync();

        public Task DeleteAsync(Order entity);
    }
}
