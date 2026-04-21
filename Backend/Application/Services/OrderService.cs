using Application.DTOs.OrderDTOs;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories.Interfaces;

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

        public async Task CreateAsync(OrderDTO dto)
        {
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
                ?? throw new KeyNotFoundException("Order not found.");

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
                ?? throw new KeyNotFoundException("Order not found.");

            return _mapper.Map<OrderDTO>(entity);
        }

        public async Task UpdateAsync(Guid id, OrderDTO dto)
        {
            var existingEntity = await _orderRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Order not found.");

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