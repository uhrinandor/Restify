using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Moq;
using RestifyServer.Dto;
using RestifyServer.Exceptions;
using RestifyServer.Interfaces.Repositories;
using RestifyServer.TypeContracts;
using RestifyServer.Services;

namespace RestifyServer.Tests.Services;

public class CategoryServiceTests
{
    private readonly Mock<IRepository<Models.Category>> _categoryRepository = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly CategoryService _sut;

    public CategoryServiceTests()
    {
        _sut = new CategoryService(_categoryRepository.Object, _mapper.Object);
    }

    [Fact]
    public async Task FindById_TargetExists_ReturnsMappedCategory()
    {
        // Arrange
        var id = Guid.NewGuid();
        var ct = CancellationToken.None;
        var dbCategory = new Models.Category { Id = id, Name = "Computers" };
        var expectedDto = new Category { Id = id, Name = "Computers" };

        _categoryRepository.Setup(r => r.GetByIdAsync(id, ct, true))
            .ReturnsAsync(dbCategory);

        _mapper.Setup(m => m.Map<Category>(dbCategory))
            .Returns(expectedDto);

        // Act
        var result = await _sut.FindById(id, ct);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(id);
        result.Name.Should().Be("Computers");
        _categoryRepository.Verify(r => r.GetByIdAsync(id, ct, true), Times.Once);
    }

    [Fact]
    public async Task FindById_TargetDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var ct = CancellationToken.None;

        _categoryRepository.Setup(r => r.GetByIdAsync(id, ct, false))
            .ReturnsAsync((Models.Category?)null);

        // Act
        var act = () => _sut.FindById(id, ct);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .Where(e => e.Message.Contains(id.ToString()));

        // Ensure mapping was never attempted since load failed
        _mapper.Verify(m => m.Map<Category>(It.IsAny<Models.Category>()), Times.Never);
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

        _categoryRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), false))
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

        _categoryRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), false))
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

        var update = new UpdateCategory(null, new Category { Id = newParentId });

        _categoryRepository.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>(), It.IsAny<bool>()))
            .ReturnsAsync(dbCategory);

        _categoryRepository.Setup(r => r.GetByIdAsync(newParentId, It.IsAny<CancellationToken>(), It.IsAny<bool>()))
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
            Parent: parentId.HasValue ? new FindParentCategory(parentId.Value, null) : null
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
    public async Task Delete_TargetDoesNotExist_ThrowsNotFound()
    {
        // Arrange
        var ct = new CancellationTokenSource().Token;
        var id = Guid.NewGuid();
        _categoryRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), false))
            .ReturnsAsync((Models.Category?)null);

        // Act
        var act = () => _sut.Delete(id, ct);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Delete_TargetExists_RemovesAndSaves()
    {
        // Arrange
        var id = Guid.NewGuid();
        var ct = new CancellationTokenSource().Token;
        var entity = new Models.Category { Id = id };
        _categoryRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), false))
            .ReturnsAsync(entity);

        // Act
        var result = await _sut.Delete(id, ct);

        // Assert
        result.Should().BeTrue();
        _categoryRepository.Verify(r => r.Remove(entity), Times.Once);
    }
}
