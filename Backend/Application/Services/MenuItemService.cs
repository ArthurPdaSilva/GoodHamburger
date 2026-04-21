using Application.DTOs.MenuItemDTOs;
using Application.Services.Interfaces;
using AutoMapper;
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

    }
}