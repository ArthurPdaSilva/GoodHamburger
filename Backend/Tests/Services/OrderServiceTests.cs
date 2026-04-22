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
    public async Task CreateAsync_ShouldCalculate20PercentDiscount_WhenOrderHasMainSideAndDrink()
    {
        var dto = new OrderDTO
        {
            SubTotal = 999m,
            Total = 999m,
            Items = new List<OrderItemDTO>
        {
            CreateItemDto(MenuItemType.Main, 5.0m),
            CreateItemDto(MenuItemType.Side, 2.0m),
            CreateItemDto(MenuItemType.Drink, 2.5m)
        }
        };

        var entity = new Order
        {
            Id = Guid.NewGuid(),
            Items = new List<OrderItem>
        {
            new() { Type = MenuItemType.Main, Name = "X Burger", Price = 5.0m, OrderId = Guid.Empty, Order = new Order(), MenuItemId = Guid.NewGuid() },
            new() { Type = MenuItemType.Side, Name = "Batata frita", Price = 2.0m, OrderId = Guid.Empty, Order = new Order(), MenuItemId = Guid.NewGuid() },
            new() { Type = MenuItemType.Drink, Name = "Refrigerante", Price = 2.5m, OrderId = Guid.Empty, Order = new Order(), MenuItemId = Guid.NewGuid() }
        }
        };

        _menuItemRepositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(CreateMenuItem(Guid.NewGuid(), "Any", 1m, MenuItemType.Main));

        _mapperMock
            .Setup(m => m.Map<Order>(dto))
            .Returns(entity);

        await _sut.CreateAsync(dto);

        Assert.That(dto.SubTotal, Is.EqualTo(9.5m).Within(0.0001m));
        Assert.That(dto.Total, Is.EqualTo(7.6m).Within(0.0001m));
    }

    [Test]
    public async Task CreateAsync_ShouldCalculate15PercentDiscount_WhenOrderHasMainAndDrink()
    {
        var dto = new OrderDTO
        {
            Items = new List<OrderItemDTO>
        {
            CreateItemDto(MenuItemType.Main, 5.0m),
            CreateItemDto(MenuItemType.Drink, 2.5m)
        }
        };

        var entity = new Order
        {
            Id = Guid.NewGuid(),
            Items = new List<OrderItem>
        {
            new() { Type = MenuItemType.Main, Name = "X Burger", Price = 5.0m, OrderId = Guid.Empty, Order = new Order(), MenuItemId = Guid.NewGuid() },
            new() { Type = MenuItemType.Drink, Name = "Refrigerante", Price = 2.5m, OrderId = Guid.Empty, Order = new Order(), MenuItemId = Guid.NewGuid() }
        }
        };

        _menuItemRepositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(CreateMenuItem(Guid.NewGuid(), "Any", 1m, MenuItemType.Main));

        _mapperMock
            .Setup(m => m.Map<Order>(dto))
            .Returns(entity);

        await _sut.CreateAsync(dto);

        Assert.That(dto.SubTotal, Is.EqualTo(7.5m).Within(0.0001m));
        Assert.That(dto.Total, Is.EqualTo(6.375m).Within(0.0001m));
    }

    [Test]
    public async Task CreateAsync_ShouldCalculate10PercentDiscount_WhenOrderHasMainAndSide()
    {
        var dto = new OrderDTO
        {
            Items = new List<OrderItemDTO>
        {
            CreateItemDto(MenuItemType.Main, 5.0m),
            CreateItemDto(MenuItemType.Side, 2.0m)
        }
        };

        var entity = new Order
        {
            Id = Guid.NewGuid(),
            Items = new List<OrderItem>
        {
            new() { Type = MenuItemType.Main, Name = "X Burger", Price = 5.0m, OrderId = Guid.Empty, Order = new Order(), MenuItemId = Guid.NewGuid() },
            new() { Type = MenuItemType.Side, Name = "Batata frita", Price = 2.0m, OrderId = Guid.Empty, Order = new Order(), MenuItemId = Guid.NewGuid() }
        }
        };

        _menuItemRepositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(CreateMenuItem(Guid.NewGuid(), "Any", 1m, MenuItemType.Main));

        _mapperMock
            .Setup(m => m.Map<Order>(dto))
            .Returns(entity);

        await _sut.CreateAsync(dto);

        Assert.That(dto.SubTotal, Is.EqualTo(7.0m).Within(0.0001m));
        Assert.That(dto.Total, Is.EqualTo(6.3m).Within(0.0001m));
    }

    [Test]
    public async Task CreateAsync_ShouldNotApplyDiscount_WhenOrderDoesNotMatchAnyCombo()
    {
        var dto = new OrderDTO
        {
            Items = new List<OrderItemDTO>
        {
            CreateItemDto(MenuItemType.Main, 5.0m)
        }
        };

        var entity = new Order
        {
            Id = Guid.NewGuid(),
            Items = new List<OrderItem>
        {
            new() { Type = MenuItemType.Main, Name = "X Burger", Price = 5.0m, OrderId = Guid.Empty, Order = new Order(), MenuItemId = Guid.NewGuid() }
        }
        };

        _menuItemRepositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(CreateMenuItem(Guid.NewGuid(), "Any", 1m, MenuItemType.Main));

        _mapperMock
            .Setup(m => m.Map<Order>(dto))
            .Returns(entity);

        await _sut.CreateAsync(dto);

        Assert.That(dto.SubTotal, Is.EqualTo(5.0m).Within(0.0001m));
        Assert.That(dto.Total, Is.EqualTo(5.0m).Within(0.0001m));
    }

    [Test]
    public async Task UpdateAsync_ShouldRecalculateTotals_IgnoringIncomingValues()
    {
        var id = Guid.NewGuid();
        var existingEntity = new Order
        {
            Id = id,
            CreatedAt = DateTime.UtcNow,
            SubTotal = 999m,
            Total = 999m,
            Items = new List<OrderItem>
        {
            new() { Type = MenuItemType.Main, Name = "Old", Price = 1m, OrderId = id, Order = new Order(), MenuItemId = Guid.NewGuid() }
        }
        };

        var dto = new OrderDTO
        {
            SubTotal = 12345m,
            Total = 12345m,
            Items = new List<OrderItemDTO>
        {
            CreateItemDto(MenuItemType.Main, 5.0m),
            CreateItemDto(MenuItemType.Drink, 2.5m)
        }
        };

        var mappedItems = new List<OrderItem>
    {
        new() { Type = MenuItemType.Main, Name = "X Burger", Price = 5.0m, OrderId = Guid.Empty, Order = new Order(), MenuItemId = Guid.NewGuid() },
        new() { Type = MenuItemType.Drink, Name = "Refrigerante", Price = 2.5m, OrderId = Guid.Empty, Order = new Order(), MenuItemId = Guid.NewGuid() }
    };

        _orderRepositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existingEntity);
        _menuItemRepositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(CreateMenuItem(Guid.NewGuid(), "Any", 1m, MenuItemType.Main));
        _mapperMock.Setup(m => m.Map<IList<OrderItem>>(dto.Items)).Returns(mappedItems);

        await _sut.UpdateAsync(id, dto);

        Assert.That(dto.SubTotal, Is.EqualTo(7.5m).Within(0.0001m));
        Assert.That(dto.Total, Is.EqualTo(6.375m).Within(0.0001m));
        Assert.That(existingEntity.SubTotal, Is.EqualTo(7.5m).Within(0.0001m));
        Assert.That(existingEntity.Total, Is.EqualTo(6.375m).Within(0.0001m));
    }

    [Test]
    public void CreateAsync_ShouldThrowArgumentException_WhenMenuItemDoesNotExist()
    {
        var missingMenuItemId = Guid.NewGuid();
        var dto = new OrderDTO
        {
            Items = new List<OrderItemDTO>
            {
                CreateItemDto(MenuItemType.Main, 10m, missingMenuItemId)
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
                CreateItemDto(MenuItemType.Main, 10m, menuItemId),
                CreateItemDto(MenuItemType.Main, 12m, menuItemId)
            }
        };

        var menuItem = CreateMenuItem(menuItemId, "X-Burger", 22m, MenuItemType.Main);

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
                CreateItemDto(MenuItemType.Main, 22m),
                CreateItemDto(MenuItemType.Side, 8m),
                CreateItemDto(MenuItemType.Drink, 5m)
            }
        };

        var entity = new Order
        {
            Id = Guid.NewGuid(),
            Items = new List<OrderItem>
            {
                new() { Type = MenuItemType.Main, Name = "X-Burger", Price = 22m, OrderId = Guid.Empty, Order = new Order() },
                new() { Type = MenuItemType.Side, Name = "Batata", Price = 8m, OrderId = Guid.Empty, Order = new Order() },
                new() { Type = MenuItemType.Drink, Name = "Refrigerante", Price = 5m, OrderId = Guid.Empty, Order = new Order() }
            }
        };

        var menuItem = CreateMenuItem(Guid.NewGuid(), "X-Burger", 22m, MenuItemType.Main);

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

        var menuItem = CreateMenuItem(Guid.NewGuid(), "X-Burger", 22m, MenuItemType.Main);

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
            new() { Id = Guid.NewGuid(), SubTotal = 30m, Total = 27m },
            new() { Id = Guid.NewGuid(), SubTotal = 15m, Total = 15m }
        };

        var mapped = new List<OrderListDTO>
        {
            new() { Id = entities[0].Id, SubTotal = 30m, Total = 27m, ItemsCount = 2 },
            new() { Id = entities[1].Id, SubTotal = 15m, Total = 15m, ItemsCount = 1 }
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
        var entity = new Order { Id = Guid.NewGuid(), SubTotal = 20m, Total = 18m };
        var mapped = new OrderDTO { Id = entity.Id, SubTotal = 20m, Total = 18m };

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
                CreateItemDto(MenuItemType.Drink, 3m, menuItemId),
                CreateItemDto(MenuItemType.Drink, 4m, menuItemId)
            }
        };

        var menuItem = CreateMenuItem(menuItemId, "X-Burger", 22m, MenuItemType.Main);

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
                CreateItemDto(MenuItemType.Main, 22m, menuItemId)
            }
        };

        var menuItem = CreateMenuItem(menuItemId, "X-Burger", 22m, MenuItemType.Main);

        _menuItemRepositoryMock
           .Setup(repository => repository.GetByIdAsync(It.IsAny<Guid>()))
           .ReturnsAsync(menuItem);

        _orderRepositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Order?)null);

        var action = async () => await _sut.UpdateAsync(id, dto);

        Assert.That(action, Throws.InstanceOf<KeyNotFoundException>()
            .With.Message.Contains("Pedido não encontrado"));
    }

    [Test]
    public async Task UpdateAsync_ShouldUpdateEntityPreserveCreatedAtAndReplaceItems_WhenOrderExists()
    {
        var id = Guid.NewGuid();
        var createdAt = new DateTime(2026, 4, 20, 10, 0, 0, DateTimeKind.Utc);
        var existingEntity = new Order
        {
            Id = id,
            CreatedAt = createdAt,
            SubTotal = 50m,
            Total = 50m,
            Items = new List<OrderItem>
            {
                new() { Type = MenuItemType.Main, Name = "Item antigo", Price = 50m, OrderId = id, Order = new Order() }
            }
        };

        var dto = new OrderDTO
        {
            SubTotal = 35m,
            Total = 31.5m,
            Items = new List<OrderItemDTO>
            {
                CreateItemDto(MenuItemType.Main, 25m),
                CreateItemDto(MenuItemType.Side, 10m)
            }
        };

        var mappedItems = new List<OrderItem>
        {
            new() { Type = MenuItemType.Main, Name = "X-Burger", Price = 25m, OrderId = Guid.Empty, Order = new Order() },
            new() { Type = MenuItemType.Side, Name = "Batata", Price = 10m, OrderId = Guid.Empty, Order = new Order() }
        };

        var menuItem = CreateMenuItem(Guid.NewGuid(), "X-Burger", 25m, MenuItemType.Main);

        _orderRepositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existingEntity);
        _mapperMock.Setup(m => m.Map<IList<OrderItem>>(dto.Items)).Returns(mappedItems);
        _menuItemRepositoryMock
            .Setup(repository => repository.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(menuItem);

        await _sut.UpdateAsync(id, dto);

        _orderRepositoryMock.Verify(r => r.ReplaceItemsAsync(existingEntity.Id,
            It.Is<IList<OrderItem>>(items => items.Count == mappedItems.Count &&
                                      items.All(i => i.OrderId == existingEntity.Id))), Times.Once);
        Assert.That(existingEntity.SubTotal, Is.EqualTo(dto.SubTotal));
        Assert.That(existingEntity.Total, Is.EqualTo(dto.Total));
        Assert.That(existingEntity.CreatedAt, Is.EqualTo(createdAt));
    }

    private static OrderItemDTO CreateItemDto(MenuItemType type, decimal price, Guid? menuItemId = null)
    {
        return new OrderItemDTO
        {
            MenuItemId = menuItemId ?? Guid.NewGuid(),
            Type = type,
            Name = $"Item {type}",
            Price = price
        };
    }

    private static MenuItem CreateMenuItem(Guid id, string name, decimal price, MenuItemType type)
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

