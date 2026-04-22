using Domain.Entities;

namespace Domain.Repositories.Interfaces
{
    public interface IMenuItemRepository
    {
        Task<IList<MenuItem>> GetAllAsync();
        Task<MenuItem?> GetByIdAsync(Guid id);
    }
}
