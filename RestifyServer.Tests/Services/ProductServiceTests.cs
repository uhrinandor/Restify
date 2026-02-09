using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Moq;
using RestifyServer.Dto;
using RestifyServer.Exceptions;
using RestifyServer.Interfaces.Repositories;
using RestifyServer.Interfaces.Services;
using RestifyServer.TypeContracts;
using RestifyServer.Services;

namespace RestifyServer.Tests.Services;

public class ProductServiceTests
{
    private readonly Mock<IRepository<Models.Product>> _productRepository = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly Mock<IEntityService<Models.Product>> _entityService = new();
    private readonly Mock<IEntityService<Models.Category>> _categoryEntityService = new();
    private readonly ProductService _sut;

    public ProductServiceTests()
    {
        _sut = new ProductService(
            _productRepository.Object,
            _entityService.Object,
            _categoryEntityService.Object,
            _mapper.Object);
    }


    [Fact]
    public async Task Create_ValidProduct_ReturnsMappedProduct()
    {
        // Arrange
        var ct = CancellationToken.None;
        var categoryId = Guid.NewGuid();

        var input = new CreateProduct("Burger", "Juicy burger", 10.50m, new Category { Id = categoryId });

        var dbCategory = new Models.Category { Id = categoryId, Name = "Food" };
        var mappedResult = new Product { Id = Guid.NewGuid(), Name = "Burger", Description = "Juicy burger", Price = 10.50m, Category = new NestedCategory { Id = categoryId, Name = "Food" } };

        _categoryEntityService.Setup(r => r.LoadEntityAsync(categoryId, ct))
            .ReturnsAsync(dbCategory);

        _mapper.Setup(m => m.Map<Product>(It.IsAny<Models.Product>()))
            .Returns(mappedResult);

        // Act
        var result = await _sut.Create(input, ct);

        // Assert
        result.Should().BeSameAs(mappedResult);
        result.Name.Should().Be("Burger");

        _productRepository.Verify(r => r.Add(It.Is<Models.Product>(p => p.Category == dbCategory)), Times.Once);
    }

    [Fact]
    public async Task Update_AllFields_UpdatesAndSaves()
    {
        // Arrange
        var ct = new CancellationTokenSource().Token;
        var id = Guid.NewGuid();
        var newCatId = Guid.NewGuid();
        var dbProduct = new Models.Product { Id = id, Name = "Old", Description = "Old", Price = 1 };
        var newCat = new Models.Category { Id = newCatId, Name = "New Cat" };

        // Primary constructor for UpdateProduct record
        var update = new UpdateProduct("New Name", "New Desc", 99.99m, new Category { Id = newCatId });

        _entityService.Setup(r => r.LoadEntityAsync(id, ct))
            .ReturnsAsync(dbProduct);
        _categoryEntityService.Setup(r => r.LoadEntityAsync(newCatId, ct))
            .ReturnsAsync(newCat);

        // Act
        await _sut.Update(id, update, ct);

        // Assert
        dbProduct.Name.Should().Be("New Name");
        dbProduct.Description.Should().Be("New Desc");
        dbProduct.Price.Should().Be(99.99m);
        dbProduct.Category.Should().Be(newCat);
    }

    [Fact]
    public async Task Update_PartialData_OnlyUpdatesProvidedFields()
    {
        // Arrange
        var ct = new CancellationTokenSource().Token;
        var id = Guid.NewGuid();
        var dbProduct = new Models.Product { Id = id, Name = "Old", Price = 10 };
        var update = new UpdateProduct(Name: "Just Name Change", null, null, null); // Price and Category are null

        _entityService.Setup(r => r.LoadEntityAsync(id, ct))
            .ReturnsAsync(dbProduct);

        // Act
        await _sut.Update(id, update, ct);

        // Assert
        dbProduct.Name.Should().Be("Just Name Change");
        dbProduct.Price.Should().Be(10); // Remains unchanged
    }

    [Fact]
    public async Task List_FiltersApplied_ReturnsMappedList()
    {
        // Arrange
        var ct = new CancellationTokenSource().Token;
        var query = new FindProduct(Name: "Filtered", null, null, null);
        var dbList = new List<Models.Product> { new() { Name = "Filtered" } };
        var mappedList = new List<Product> { new Product { Id = Guid.NewGuid(), Name = "Filtered", Description = "", Price = 0, Category = new NestedCategory { Id = Guid.NewGuid(), Name = "C" } } };

        _productRepository.Setup(r => r.ListAsync(It.IsAny<Expression<Func<Models.Product, bool>>>(), It.IsAny<CancellationToken>(), true))
            .ReturnsAsync(dbList);

        _mapper.Setup(m => m.Map<List<Product>>(dbList)).Returns(mappedList);

        // Act
        var result = await _sut.List(query, ct);

        // Assert
        result.Should().HaveCount(1);
        result.First().Name.Should().Be("Filtered");
    }

    [Fact]
    public async Task FindById_ValidId_ReturnsMappedProduct()
    {
        // Arrange
        var ct = new CancellationTokenSource().Token;
        var id = Guid.NewGuid();
        var dbProduct = new Models.Product { Id = id, Name = "Found" };
        var mapped = new Product { Id = id, Name = "Found", Description = "", Price = 0, Category = new NestedCategory { Id = Guid.NewGuid(), Name = "C" } };

        _entityService.Setup(r => r.LoadEntity(id, ct))
            .ReturnsAsync(dbProduct);
        _mapper.Setup(m => m.Map<Product>(dbProduct)).Returns(mapped);

        // Act
        var result = await _sut.FindById(id, ct);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(id);
    }

    [Fact]
    public async Task Delete_Existing_RemovesAndSaves()
    {
        // Arrange
        var ct = new CancellationTokenSource().Token;
        var id = Guid.NewGuid();
        var dbProduct = new Models.Product { Id = id };

        _entityService.Setup(r => r.LoadEntityAsync(id, ct))
            .ReturnsAsync(dbProduct);

        // Act
        var result = await _sut.Delete(id, ct);

        // Assert
        result.Should().BeTrue();
        _productRepository.Verify(r => r.Remove(dbProduct), Times.Once);
    }
}
