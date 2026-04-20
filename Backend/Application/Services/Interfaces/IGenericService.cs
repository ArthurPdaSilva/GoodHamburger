namespace Application.Services.Interfaces
{
    /// <summary>
    /// Interface que delimita as funções genéricas as interfaces de serviço.
    /// Criei essa interface por ver que havia muita repetição desnecessária
    /// </summary>
    public interface IGenericService<T, TList>
    {
        Task CreateAsync(T dto);

        Task UpdateAsync(Guid id, T dto);

        Task DeleteAsync(Guid id);

        Task<IList<TList>> GetAllAsync();

        Task<T> GetByIdAsync(Guid id);
    }
}
