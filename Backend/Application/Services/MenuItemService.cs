using Application.DTOs.MenuItemDTOs;
using Application.DTOs.OrderDTOs;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Repositories;
using Domain.Repositories.Interfaces;

namespace Application.Services
{
    public class MenuItemService : IMenuItemService
    {
        private readonly IMenuItemRepository _menuItemRepository;
        private readonly IMapper _mapper;

        public MenuItemService(IMenuItemRepository menuItemRepository, IMapper mapper)
        {
            _menuItemRepository = menuItemRepository;
            _mapper = mapper;
        }

        public async Task<IList<MenuItemDTO>> GetAllAsync()
        {
            var entities = await _menuItemRepository.GetAllAsync();
            return _mapper.Map<IList<MenuItemDTO>>(entities);
        }

        public async Task<MenuItemDTO> GetByIdAsync(Guid id)
        {
            var entity = await _menuItemRepository.GetByIdAsync(id)
                 ?? throw new KeyNotFoundException("Item não encontrado.");

            return _mapper.Map<MenuItemDTO>(entity);
        }
    }
}