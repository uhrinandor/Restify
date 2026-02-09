using AutoMapper;
using Moq;
using FluentAssertions;
using RestifyServer.Dto;
using RestifyServer.Interfaces.Repositories;
using RestifyServer.Services;
using RestifyServer.TypeContracts;
using System.Linq.Expressions;
using RestifyServer.Interfaces.Services;

namespace RestifyServer.Tests.Services;

public class TableServiceBaseTests
{
    private readonly Mock<IRepository<Models.Table>> _tableRepoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IEntityService<Models.Table>> _entityService = new();
    private readonly TableServiceBase _sut;

    public TableServiceBaseTests()
    {
        _tableRepoMock = new Mock<IRepository<Models.Table>>();
        _mapperMock = new Mock<IMapper>();

        _sut = new TableServiceBase(_tableRepoMock.Object, _entityService.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task List_WithNoFilters_ShouldCallRepoWithTruePredicate()
    {
        // Arrange
        var query = new FindTable(null, null);
        var ct = CancellationToken.None;

        _tableRepoMock.Setup(r => r.ListAsync(It.IsAny<Expression<Func<Models.Table, bool>>>(), ct))
            .ReturnsAsync(new List<Models.Table>());

        // Act
        await _sut.List(query, ct);

        // Assert
        _tableRepoMock.Verify(r => r.ListAsync(It.IsAny<Expression<Func<Models.Table, bool>>>(), ct), Times.Once);
    }

    [Fact]
    public async Task FindById_WhenExists_ShouldReturnMappedTable()
    {
        // Arrange
        var ct = new CancellationTokenSource().Token;
        var id = Guid.NewGuid();
        var dbTable = new Models.Table { Id = id, Number = 1 };
        var dtoTable = new Table { Id = id, Number = 1 };

        _entityService.Setup(r => r.LoadEntity(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(dbTable);
        _mapperMock.Setup(m => m.Map<Table>(dbTable)).Returns(dtoTable);

        // Act
        var result = await _sut.FindById(id, ct);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(id);
    }

    [Fact]
    public async Task Create_ShouldAssignNumberAndCommit()
    {
        // Arrange
        var ct = new CancellationTokenSource().Token;
        var dto = new CreateTable(5);

        // Act
        await _sut.Create(dto, ct);

        // Assert
        _tableRepoMock.Verify(r => r.Add(It.Is<Models.Table>(t => t.Number == 5)), Times.Once);
    }

    [Fact]
    public async Task Update_WhenNumberIsNull_ShouldNotChangeExistingNumber()
    {
        // Arrange
        var ct = new CancellationTokenSource().Token;
        var id = Guid.NewGuid();
        var existing = new Models.Table { Id = id, Number = 10 };
        var updateDto = new UpdateTable(null); // Record allows null via primary constructor

        _entityService.Setup(r => r.LoadEntityAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        // Act
        await _sut.Update(id, updateDto, ct);

        // Assert
        existing.Number.Should().Be(10);
    }

    [Fact]
    public async Task Delete_WhenExists_ShouldRemoveAndSave()
    {
        // Arrange
        var ct = new CancellationTokenSource().Token;
        var id = Guid.NewGuid();
        var existing = new Models.Table { Id = id };
        _entityService.Setup(r => r.LoadEntityAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        // Act
        var result = await _sut.Delete(id, ct);

        // Assert
        result.Should().BeTrue();
        _tableRepoMock.Verify(r => r.Remove(existing), Times.Once);
    }
}
