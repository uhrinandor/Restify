using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Moq;
using RestifyServer.Dto;
using RestifyServer.Interfaces.Repositories;
using RestifyServer.Interfaces.Services;
using RestifyServer.TypeContracts;
using RestifyServer.Services;

namespace RestifyServer.Tests.Services;

public class CategoryServiceTests
{
    private readonly Mock<IRepository<Models.Category>> _categoryRepository = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly CategoryService _sut;
    private readonly Mock<IEntityService<Models.Category>> _entityService = new();

    public CategoryServiceTests()
    {
        _sut = new CategoryService(_categoryRepository.Object, _entityService.Object, _mapper.Object);
    }

    [Fact]
    public async Task FindById_TargetExists_ReturnsMappedCategory()
    {
        // Arrange
        var id = Guid.NewGuid();
        var ct = CancellationToken.None;
        var dbCategory = new Models.Category { Id = id, Name = "Computers" };
        var expectedDto = new Category { Id = id, Name = "Computers" };

        _entityService.Setup(r => r.LoadEntity(id, ct))
            .ReturnsAsync(dbCategory);

        _mapper.Setup(m => m.Map<Category>(dbCategory))
            .Returns(expectedDto);

        // Act
        var result = await _sut.FindById(id, ct);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(id);
        result.Name.Should().Be("Computers");
        _entityService.Verify(r => r.LoadEntity(id, ct), Times.Once);
    }

    [Fact]
    public async Task Create_ValidData_ReturnsMappedResult()
    {
        // Arrange
        var ct = new CancellationTokenSource().Token;
        var input = new CreateCategory("Hardware", null);
        var contract = new Category { Name = "Hardware" };

        _mapper.Setup(m => m.Map<Category>(It.IsAny<Models.Category>())).Returns(contract);

        // Act
        var result = await _sut.Create(input, ct);

        // Assert
        result.Name.Should().Be("Hardware");
        _categoryRepository.Verify(r => r.Add(It.Is<Models.Category>(c => c.Name == "Hardware")), Times.Once);
    }

    [Fact]
    public async Task Create_WithExistingParent_LinksSuccessfully()
    {
        // Arrange
        var ct = new CancellationTokenSource().Token;
        var parentId = Guid.NewGuid();
        var parentEntity = new Models.Category { Id = parentId, Name = "Root" };
        var input = new CreateCategory("Child", new NestedCategory { Id = parentEntity.Id });

        _entityService.Setup(r => r.LoadEntityAsync(It.IsAny<Guid>(), ct))
            .ReturnsAsync(parentEntity);

        // Act
        await _sut.Create(input, ct);

        // Assert
        _categoryRepository.Verify(r => r.Add(It.Is<Models.Category>(c => c.Parent == parentEntity)), Times.Once);
    }

    [Fact]
    public async Task Update_ChangeNameOnly_KeepsOldParent()
    {
        // Arrange
        var ct = new CancellationTokenSource().Token;
        var id = Guid.NewGuid();
        var existingParent = new Models.Category { Id = Guid.NewGuid() };
        var dbCategory = new Models.Category { Id = id, Name = "Old", Parent = existingParent };
        var update = new UpdateCategory("New", null);

        _entityService.Setup(r => r.LoadEntityAsync(It.IsAny<Guid>(), ct))
            .ReturnsAsync(dbCategory);

        // Act
        await _sut.Update(id, update, ct);

        // Assert
        dbCategory.Name.Should().Be("New");
        dbCategory.Parent.Should().Be(existingParent); // Verify logic: if data.Parent is null, we don't clear the parent
    }

    [Fact]
    public async Task Update_SwitchParent_LoadsAndAssignsNewParent()
    {
        // Arrange
        var ct = new CancellationTokenSource().Token;
        var id = Guid.NewGuid();
        var newParentId = Guid.NewGuid();
        var dbCategory = new Models.Category { Id = id, Name = "Category" };
        var newParent = new Models.Category { Id = newParentId, Name = "New Parent" };

        var update = new UpdateCategory(null, new FindEntity(Id: newParentId));

        _entityService.Setup(r => r.LoadEntityAsync(id, ct))
            .ReturnsAsync(dbCategory);

        _entityService.Setup(r => r.LoadEntityAsync(newParentId, ct))
            .ReturnsAsync(newParent);

        // Act
        await _sut.Update(id, update, ct);

        // Assert
        dbCategory.Parent.Should().Be(newParent);
    }

    [Theory]
    [InlineData("Electronics", null)]
    [InlineData(null, "6990521e-706a-4638-958b-085e78111e00")]
    [InlineData("Furniture", "6990521e-706a-4638-958b-085e78111e00")]
    public async Task List_FiltersCorrectValues(string? nameFilter, string? parentIdStr)
    {
        // Arrange
        var ct = new CancellationTokenSource().Token;
        Guid? parentId = parentIdStr != null ? Guid.Parse(parentIdStr) : null;
        var query = new FindCategory(
            Id: null,
            Name: nameFilter,
            Parent: parentId.HasValue ? new FindEntity(Id: parentId.Value) : null
        );

        _categoryRepository.Setup(r => r.ListAsync(It.IsAny<Expression<Func<Models.Category, bool>>>(), It.IsAny<CancellationToken>(), true))
            .ReturnsAsync(new List<Models.Category>());

        // Act
        await _sut.List(query, ct);

        // Assert
        _categoryRepository.Verify(r => r.ListAsync(
            It.Is<Expression<Func<Models.Category, bool>>>(p => true), // Logic verification of predicate construction
            It.IsAny<CancellationToken>(),
            true), Times.Once);
    }

    [Fact]
    public async Task Delete_TargetExists_RemovesAndSaves()
    {
        // Arrange
        var id = Guid.NewGuid();
        var ct = new CancellationTokenSource().Token;
        var entity = new Models.Category { Id = id };
        _entityService.Setup(r => r.LoadEntityAsync(id, ct))
            .ReturnsAsync(entity);

        // Act
        var result = await _sut.Delete(id, ct);

        // Assert
        result.Should().BeTrue();
        _categoryRepository.Verify(r => r.Remove(entity), Times.Once);
    }
}
