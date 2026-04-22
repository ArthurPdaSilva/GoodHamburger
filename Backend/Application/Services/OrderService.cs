using Application.DTOs.OrderDTOs;
using Application.DTOs.OrderItemDTOs;
using Application.Services.Interfaces;
using Application.Utils;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories.Interfaces;

namespace Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMenuItemRepository _menuItemRepository;
        private readonly IMapper _mapper;

        public OrderService(IOrderRepository orderRepository, IMapper mapper, IMenuItemRepository menuItemRepository)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _menuItemRepository = menuItemRepository;
        }

        private async Task EnsureMenuItemsExistAsync(IList<OrderItemDTO> items)
        {
            foreach (var item in items)
            {
                var menuItem = await _menuItemRepository.GetByIdAsync(item.MenuItemId);
                if (menuItem == null)
                {
                    throw new ArgumentException($"Item do menu com ID {item.MenuItemId} não encontrado.");
                }
            }
        }

        private void ValidateOrderItems(IList<OrderItemDTO> items)
        {
            if (items == null || !items.Any())
            {
                throw new ArgumentException("O pedido deve conter pelo menos um item.");
            }

            var duplicateTypes = items
                                .GroupBy(item => item.Type)
                                .Where(g => g.Count() > 1)
                                .Select(g => g.Key)
                                .ToList();

            if (duplicateTypes.Any())
            {
                var duplicateTypesString = string.Join(", ", duplicateTypes.Select(MenuItemTypeTranslator.ToFriendlyString));
                throw new ArgumentException($"O pedido não pode conter itens duplicados do mesmo tipo: {duplicateTypesString}");
            }
        }

        private decimal CalculateSubTotal(IList<OrderItemDTO> items)
        {
            return items.Sum(i => i.Price);
        }

        private decimal CalculateTotalWithDiscount(IList<OrderItemDTO> items, decimal subTotal)
        {
            var hasSandwich = items.Any(i => i.Type == MenuItemType.Main);
            var hasFries = items.Any(i => i.Type == MenuItemType.Side);
            var hasSoda = items.Any(i => i.Type == MenuItemType.Drink);
            var discount = 1.0m;
            if (hasSandwich && hasFries && hasSoda)
                discount = 0.80m;
            else if (hasSandwich && hasSoda)
                discount = 0.85m; 
            else if (hasSandwich && hasFries)
                discount = 0.90m; 
            return subTotal * discount;
        }

        public async Task CreateAsync(OrderDTO dto)
        {
            await EnsureMenuItemsExistAsync(dto.Items);
            ValidateOrderItems(dto.Items);

            dto.SubTotal = CalculateSubTotal(dto.Items);
            dto.Total = CalculateTotalWithDiscount(dto.Items, dto.SubTotal);

            var entity = _mapper.Map<Order>(dto);
            foreach (var item in entity.Items)
            {
                item.Order = entity;
            }

            await _orderRepository.CreateAsync(entity);
            dto.Id = entity.Id;
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _orderRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Pedido não encontrado.");

            await _orderRepository.DeleteAsync(entity);
        }

        public async Task<IList<OrderListDTO>> GetAllAsync()
        {
            var entities = await _orderRepository.GetAllAsync();
            return _mapper.Map<IList<OrderListDTO>>(entities);
        }

        public async Task<OrderDTO> GetByIdAsync(Guid id)
        {
            var entity = await _orderRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Pedido não encontrado.");

            return _mapper.Map<OrderDTO>(entity);
        }

        public async Task UpdateAsync(Guid id, OrderDTO dto)
        {
            await EnsureMenuItemsExistAsync(dto.Items);
            ValidateOrderItems(dto.Items);
            dto.SubTotal = CalculateSubTotal(dto.Items);
            dto.Total = CalculateTotalWithDiscount(dto.Items, dto.SubTotal);

            var existingEntity = await _orderRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Pedido não encontrado.");

            var createdAt = existingEntity.CreatedAt;

            existingEntity.SubTotal = dto.SubTotal;
            existingEntity.Total = dto.Total;

            var updatedItems = _mapper.Map<IList<OrderItem>>(dto.Items);
            foreach (var item in updatedItems)
            {
                item.OrderId = existingEntity.Id;
            }

            existingEntity.CreatedAt = createdAt;

            await _orderRepository.ReplaceItemsAsync(existingEntity.Id, updatedItems);
        }
    }
}