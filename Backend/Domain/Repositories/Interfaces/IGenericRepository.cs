namespace Domain.Repositories.Interfaces
{

    /// <summary>
    /// Repository generíco para todos os repositories
    /// </summary>
    public interface IGenericRepository<T>
    {
        public Task CreateAsync(T entity);

        public Task UpdateAsync(T entity);

        public Task<T?> GetByIdAsync(Guid id);

        public Task<IList<T>> GetAllAsync();

        public Task DeleteAsync(T entity);
    }
}
