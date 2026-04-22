using Application.DTOs.OrderDTOs;
using Application.DTOs.OrderItemDTOs;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories.Interfaces;
using Moq;

namespace Tests.Services;

[TestFixture]
public class OrderServiceTests
{
    private Mock<IOrderRepository> _orderRepositoryMock = null!;
    private Mock<IMenuItemRepository> _menuItemRepositoryMock = null!;
    private Mock<IMapper> _mapperMock = null!;
    private OrderService _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _menuItemRepositoryMock = new Mock<IMenuItemRepository>();
        _mapperMock = new Mock<IMapper>();
        _sut = new OrderService(_orderRepositoryMock.Object, _mapperMock.Object, _menuItemRepositoryMock.Object);
    }

    [Test]
    public void CreateAsync_ShouldThrowArgumentException_WhenMenuItemDoesNotExist()
    {
        var missingMenuItemId = Guid.NewGuid();
        var dto = new OrderDTO
        {
            Items = new List<OrderItemDTO>
            {
                CreateItemDto(MenuItemType.Main, 10f, missingMenuItemId)
            }
        };

        _menuItemRepositoryMock
            .Setup(repository => repository.GetByIdAsync(missingMenuItemId))
            .ReturnsAsync((MenuItem?)null);

        var action = async () => await _sut.CreateAsync(dto);

        Assert.That(action, Throws.InstanceOf<ArgumentException>()
            .With.Message.Contains("não encontrado"));
    }

    [Test]
    public void CreateAsync_ShouldThrowArgumentException_WhenOrderHasNoItems()
    {
        var dto = new OrderDTO
        {
            Items = new List<OrderItemDTO>()
        };

        var action = async () => await _sut.CreateAsync(dto);

        Assert.That(action, Throws.InstanceOf<ArgumentException>()
            .With.Message.Contains("pelo menos um item"));
    }

    [Test]
    public void CreateAsync_ShouldThrowArgumentException_WhenOrderHasDuplicateItemTypes()
    {
        var menuItemId = Guid.NewGuid();

        var dto = new OrderDTO
        {
            Items = new List<OrderItemDTO>
            {
                CreateItemDto(MenuItemType.Main, 10f, menuItemId),
                CreateItemDto(MenuItemType.Main, 12f, menuItemId)
            }
        };

        var menuItem = CreateMenuItem(menuItemId, "X-Burger", 22f, MenuItemType.Main);

        _menuItemRepositoryMock
           .Setup(repository => repository.GetByIdAsync(It.IsAny<Guid>()))
           .ReturnsAsync(menuItem);

        var action = async () => await _sut.CreateAsync(dto);

        Assert.That(action, Throws.InstanceOf<ArgumentException>()
            .With.Message.Contains("itens duplicados"));
    }

    [Test]
    public async Task CreateAsync_ShouldMapPersistAndSetDtoId_WhenOrderIsValid()
    {
        var dto = new OrderDTO
        {
            Items = new List<OrderItemDTO>
            {
                CreateItemDto(MenuItemType.Main, 22f),
                CreateItemDto(MenuItemType.Side, 8f),
                CreateItemDto(MenuItemType.Drink, 5f)
            }
        };

        var entity = new Order
        {
            Id = Guid.NewGuid(),
            Items = new List<OrderItem>
            {
                new() { Type = MenuItemType.Main, Name = "X-Burger", Price = 22f, OrderId = Guid.Empty, Order = new Order() },
                new() { Type = MenuItemType.Side, Name = "Batata", Price = 8f, OrderId = Guid.Empty, Order = new Order() },
                new() { Type = MenuItemType.Drink, Name = "Refrigerante", Price = 5f, OrderId = Guid.Empty, Order = new Order() }
            }
        };

        var menuItem = CreateMenuItem(Guid.NewGuid(), "X-Burger", 22f, MenuItemType.Main);

        _mapperMock
            .Setup(m => m.Map<Order>(dto))
            .Returns(entity);

        _menuItemRepositoryMock
            .Setup(repository => repository.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(menuItem);

        await _sut.CreateAsync(dto);

        _orderRepositoryMock.Verify(r => r.CreateAsync(entity), Times.Once);
        Assert.That(dto.Id, Is.EqualTo(entity.Id));
        Assert.That(entity.Items.All(i => ReferenceEquals(i.Order, entity)), Is.True);
    }

    [Test]
    public async Task DeleteAsync_ShouldThrowKeyNotFoundException_WhenOrderDoesNotExist()
    {
        var id = Guid.NewGuid();
        _orderRepositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Order?)null);

        var menuItem = CreateMenuItem(Guid.NewGuid(), "X-Burger", 22f, MenuItemType.Main);

        _menuItemRepositoryMock
            .Setup(repository => repository.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(menuItem);

        var action = async () => await _sut.DeleteAsync(id);

        Assert.That(action, Throws.InstanceOf<KeyNotFoundException>()
            .With.Message.Contains("Pedido não encontrado"));
    }

    [Test]
    public async Task DeleteAsync_ShouldDeleteOrder_WhenOrderExists()
    {
        var entity = new Order { Id = Guid.NewGuid() };
        _orderRepositoryMock.Setup(r => r.GetByIdAsync(entity.Id)).ReturnsAsync(entity);

        await _sut.DeleteAsync(entity.Id);

        _orderRepositoryMock.Verify(r => r.DeleteAsync(entity), Times.Once);
    }

    [Test]
    public async Task GetAllAsync_ShouldReturnMappedDtos()
    {
        var entities = new List<Order>
        {
            new() { Id = Guid.NewGuid(), SubTotal = 30f, Total = 27f },
            new() { Id = Guid.NewGuid(), SubTotal = 15f, Total = 15f }
        };

        var mapped = new List<OrderListDTO>
        {
            new() { Id = entities[0].Id, SubTotal = 30f, Total = 27f, ItemsCount = 2 },
            new() { Id = entities[1].Id, SubTotal = 15f, Total = 15f, ItemsCount = 1 }
        };

        _orderRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(entities);
        _mapperMock.Setup(m => m.Map<IList<OrderListDTO>>(entities)).Returns(mapped);

        var result = await _sut.GetAllAsync();

        Assert.That(result, Is.SameAs(mapped));
    }

    [Test]
    public async Task GetByIdAsync_ShouldThrowKeyNotFoundException_WhenOrderDoesNotExist()
    {
        var id = Guid.NewGuid();
        _orderRepositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Order?)null);

        var action = async () => await _sut.GetByIdAsync(id);

        Assert.That(action, Throws.InstanceOf<KeyNotFoundException>()
            .With.Message.Contains("Pedido não encontrado"));
    }

    [Test]
    public async Task GetByIdAsync_ShouldReturnMappedDto_WhenOrderExists()
    {
        var entity = new Order { Id = Guid.NewGuid(), SubTotal = 20f, Total = 18f };
        var mapped = new OrderDTO { Id = entity.Id, SubTotal = 20f, Total = 18f };

        _orderRepositoryMock.Setup(r => r.GetByIdAsync(entity.Id)).ReturnsAsync(entity);
        _mapperMock.Setup(m => m.Map<OrderDTO>(entity)).Returns(mapped);

        var result = await _sut.GetByIdAsync(entity.Id);

        Assert.That(result, Is.SameAs(mapped));
    }

    [Test]
    public async Task UpdateAsync_ShouldThrowArgumentException_WhenOrderHasDuplicateItemTypes()
    {
        var menuItemId = Guid.NewGuid();

        var dto = new OrderDTO
        {
            Items = new List<OrderItemDTO>
            {
                CreateItemDto(MenuItemType.Drink, 3f, menuItemId),
                CreateItemDto(MenuItemType.Drink, 4f, menuItemId)
            }
        };

        var menuItem = CreateMenuItem(menuItemId, "X-Burger", 22f, MenuItemType.Main);

        _menuItemRepositoryMock
           .Setup(repository => repository.GetByIdAsync(It.IsAny<Guid>()))
           .ReturnsAsync(menuItem);

        var action = async () => await _sut.UpdateAsync(Guid.NewGuid(), dto);

        Assert.That(action, Throws.InstanceOf<ArgumentException>()
            .With.Message.Contains("itens duplicados"));
    }

    [Test]
    public async Task UpdateAsync_ShouldThrowKeyNotFoundException_WhenOrderDoesNotExist()
    {
        var menuItemId = Guid.NewGuid();
        var id = Guid.NewGuid();
        var dto = new OrderDTO
        {
            Items = new List<OrderItemDTO>
            {
                CreateItemDto(MenuItemType.Main, 22f, menuItemId)
            }
        };

        var menuItem = CreateMenuItem(menuItemId, "X-Burger", 22f, MenuItemType.Main);

        _menuItemRepositoryMock
           .Setup(repository => repository.GetByIdAsync(It.IsAny<Guid>()))
           .ReturnsAsync(menuItem);

        _orderRepositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Order?)null);

        var action = async () => await _sut.UpdateAsync(id, dto);

        Assert.That(action, Throws.InstanceOf<KeyNotFoundException>()
            .With.Message.Contains("Pedido não encontrado"));
    }

    [Test]
    public async Task UpdateAsync_ShouldUpdateEntityPreserveCreatedAtAndRelinkItems_WhenOrderExists()
    {
        var id = Guid.NewGuid();
        var createdAt = new DateTime(2026, 4, 20, 10, 0, 0, DateTimeKind.Utc);
        var existingEntity = new Order
        {
            Id = id,
            CreatedAt = createdAt,
            SubTotal = 50f,
            Total = 50f,
            Items = new List<OrderItem>
            {
                new() { Type = MenuItemType.Main, Name = "Item antigo", Price = 50f, OrderId = id, Order = new Order() }
            }
        };

        var dto = new OrderDTO
        {
            SubTotal = 35f,
            Total = 31.5f,
            Items = new List<OrderItemDTO>
            {
                CreateItemDto(MenuItemType.Main, 25f),
                CreateItemDto(MenuItemType.Side, 10f)
            }
        };

        var mappedItems = new List<OrderItem>
        {
            new() { Type = MenuItemType.Main, Name = "X-Burger", Price = 25f, OrderId = Guid.Empty, Order = new Order() },
            new() { Type = MenuItemType.Side, Name = "Batata", Price = 10f, OrderId = Guid.Empty, Order = new Order() }
        };

        var menuItem = CreateMenuItem(Guid.NewGuid(), "X-Burger", 25f, MenuItemType.Main);

        _orderRepositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existingEntity);
        _mapperMock.Setup(m => m.Map<IList<OrderItem>>(dto.Items)).Returns(mappedItems);
        _menuItemRepositoryMock
            .Setup(repository => repository.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(menuItem);

        await _sut.UpdateAsync(id, dto);

        _orderRepositoryMock.Verify(r => r.UpdateAsync(existingEntity), Times.Once);
        Assert.That(existingEntity.SubTotal, Is.EqualTo(dto.SubTotal));
        Assert.That(existingEntity.Total, Is.EqualTo(dto.Total));
        Assert.That(existingEntity.CreatedAt, Is.EqualTo(createdAt));
        Assert.That(existingEntity.Items.Count, Is.EqualTo(mappedItems.Count));
        Assert.That(existingEntity.Items.All(i => i.OrderId == existingEntity.Id), Is.True);
        Assert.That(existingEntity.Items.All(i => ReferenceEquals(i.Order, existingEntity)), Is.True);
    }

    private static OrderItemDTO CreateItemDto(MenuItemType type, float price, Guid? menuItemId = null)
    {
        return new OrderItemDTO
        {
            MenuItemId = menuItemId ?? Guid.NewGuid(),
            Type = type,
            Name = $"Item {type}",
            Price = price
        };
    }

    private static MenuItem CreateMenuItem(Guid id, string name, float price, MenuItemType type)
    {
        return new MenuItem
        {
            Id = id,
            Name = name,
            Price = price,
            Type = type
        };
    }
}
