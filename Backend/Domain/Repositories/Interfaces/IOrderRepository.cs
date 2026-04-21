using Domain.Entities;

namespace Domain.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        public Task CreateAsync(Order entity);

        public Task UpdateAsync(Order entity);

        public Task<Order?> GetByIdAsync(Guid id);
        public Task<IList<Order>> GetAllAsync();

        public Task DeleteAsync(Order entity);
    }
}
