using Application.DTOs.MenuItemDTOs;

namespace Application.Services.Interfaces
{
    public interface IMenuItemService
    {
        Task<IList<MenuItemDTO>> GetAllAsync();
        Task<MenuItemDTO> GetByIdAsync(Guid id);
    }
}
