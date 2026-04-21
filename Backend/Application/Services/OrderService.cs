using Application.DTOs.OrderDTOs;
using Application.DTOs.OrderItemDTOs;
using Application.Services.Interfaces;
using Application.Utils;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public OrderService(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
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

        private float CalculateTotalWithDiscount(IList<OrderItemDTO> items)
        {
            var hasSandwich = items.Any(i => i.Type == MenuItemType.Main);
            var hasFries = items.Any(i => i.Type == MenuItemType.Side);
            var hasSoda = items.Any(i => i.Type == MenuItemType.Drink);
            var subTotal = items.Sum(i => i.Price);
            var discount = 1.0f;
            if (hasSandwich && hasFries && hasSoda)
                discount = 0.80f;
            else if (hasSandwich && hasSoda)
                discount = 0.85f; 
            else if (hasSandwich && hasFries)
                discount = 0.90f; 
            return subTotal * discount;
        }

        public async Task CreateAsync(OrderDTO dto)
        {
            ValidateOrderItems(dto.Items);
            CalculateTotalWithDiscount(dto.Items);

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
            ValidateOrderItems(dto.Items);
            CalculateTotalWithDiscount(dto.Items);

            var existingEntity = await _orderRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Pedido não encontrado.");

            var createdAt = existingEntity.CreatedAt;

            existingEntity.SubTotal = dto.SubTotal;
            existingEntity.Total = dto.Total;
            existingEntity.Items.Clear();

            var updatedItems = _mapper.Map<IList<OrderItem>>(dto.Items);
            foreach (var item in updatedItems)
            {
                item.OrderId = existingEntity.Id;
                item.Order = existingEntity;
                existingEntity.Items.Add(item);
            }

            existingEntity.CreatedAt = createdAt;

            await _orderRepository.UpdateAsync(existingEntity);
        }
    }
}