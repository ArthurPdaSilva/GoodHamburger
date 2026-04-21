using Application.DTOs.MenuItemDTOs;
using Application.DTOs.OrderDTOs;
using Application.Mapping;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories.Interfaces;
using Moq;

namespace Tests.Services;

[TestFixture]
public class MenuItemServiceTests
{
    private Mock<IMenuItemRepository> _menuItemRepositoryMock = null!;
    private Mock<IMapper> _mapperMock = null!;
    private MenuItemService _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _menuItemRepositoryMock = new Mock<IMenuItemRepository>();

        _mapperMock = new Mock<IMapper>();
        _sut = new MenuItemService(_menuItemRepositoryMock.Object, _mapperMock.Object);
    }

    [Test]
    public async Task GetAllAsync_ShouldReturnAllSeedItemsWithCorrectData()
    {
        var seededEntities = new List<MenuItem>
        {
            new()
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "X Burger",
                Price = 5.00f,
                Type = MenuItemType.Main
            },
            new()
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Name = "X Egg",
                Price = 4.50f,
                Type = MenuItemType.Main
            },
            new()
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Name = "X Bacon",
                Price = 7.00f,
                Type = MenuItemType.Main
            },
            new()
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                Name = "Batata frita",
                Price = 2.00f,
                Type = MenuItemType.Side
            },
            new()
            {
                Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                Name = "Refrigerante",
                Price = 2.50f,
                Type = MenuItemType.Drink
            }
        };

        var mapped = new List<MenuItemDTO>
        {
            new()
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "X Burger",
                Price = 5.00f,
                Type = MenuItemType.Main
            },
            new()
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Name = "X Egg",
                Price = 4.50f,
                Type = MenuItemType.Main
            },
            new()
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Name = "X Bacon",
                Price = 7.00f,
                Type = MenuItemType.Main
            },
            new()
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                Name = "Batata frita",
                Price = 2.00f,
                Type = MenuItemType.Side
            },
            new()
            {
                Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                Name = "Refrigerante",
                Price = 2.50f,
                Type = MenuItemType.Drink
            }
        };

        _menuItemRepositoryMock
            .Setup(repository => repository.GetAllAsync())
            .ReturnsAsync(seededEntities);
        _mapperMock.Setup(m => m.Map<IList<MenuItemDTO>>(seededEntities)).Returns(mapped);

        var result = await _sut.GetAllAsync();

        _menuItemRepositoryMock.Verify(repository => repository.GetAllAsync(), Times.Once);
        Assert.That(result, Has.Count.EqualTo(5));

        Assert.Multiple(() =>
        {
            AssertMenuItem(result[0], Guid.Parse("11111111-1111-1111-1111-111111111111"), "X Burger", 5.00f, MenuItemType.Main);
            AssertMenuItem(result[1], Guid.Parse("22222222-2222-2222-2222-222222222222"), "X Egg", 4.50f, MenuItemType.Main);
            AssertMenuItem(result[2], Guid.Parse("33333333-3333-3333-3333-333333333333"), "X Bacon", 7.00f, MenuItemType.Main);
            AssertMenuItem(result[3], Guid.Parse("44444444-4444-4444-4444-444444444444"), "Batata frita", 2.00f, MenuItemType.Side);
            AssertMenuItem(result[4], Guid.Parse("55555555-5555-5555-5555-555555555555"), "Refrigerante", 2.50f, MenuItemType.Drink);
        });
    }

    [Test]
    public async Task GetAllAsync_ShouldReturnEmptyList_WhenRepositoryReturnsEmptyList()
    {
        var emptyEntities = new List<MenuItem>();
        var mapped = new List<MenuItemDTO>();

        _menuItemRepositoryMock
            .Setup(repository => repository.GetAllAsync())
            .ReturnsAsync(emptyEntities);
        _mapperMock.Setup(m => m.Map<IList<MenuItemDTO>>(emptyEntities)).Returns(mapped);

        var result = await _sut.GetAllAsync();

        Assert.That(result, Is.Empty);
    }

    private static void AssertMenuItem(MenuItemDTO actual, Guid id, string name, float price, MenuItemType type)
    {
        Assert.That(actual.Id, Is.EqualTo(id));
        Assert.That(actual.Name, Is.EqualTo(name));
        Assert.That(actual.Price, Is.EqualTo(price));
        Assert.That(actual.Type, Is.EqualTo(type));
    }
}
