using AutoMapper;
using Moq;
using FluentAssertions;
using RestifyServer.Dto;
using RestifyServer.Exceptions;
using RestifyServer.Interfaces.Repositories;
using RestifyServer.Services;
using RestifyServer.TypeContracts;
using System.Linq.Expressions;

namespace RestifyServer.Tests.Services;

public class TableServiceTests
{
    private readonly Mock<IRepository<Models.Table>> _tableRepoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly TableService _sut;

    public TableServiceTests()
    {
        _tableRepoMock = new Mock<IRepository<Models.Table>>();
        _mapperMock = new Mock<IMapper>();

        _sut = new TableService(_tableRepoMock.Object, _mapperMock.Object);
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

        _tableRepoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>(), true))
            .ReturnsAsync(dbTable);
        _mapperMock.Setup(m => m.Map<Table>(dbTable)).Returns(dtoTable);

        // Act
        var result = await _sut.FindById(id, ct);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(id);
    }

    [Fact]
    public async Task FindById_WhenNotExists_ShouldThrowNotFoundException()
    {
        // Arrange
        var ct = new CancellationTokenSource().Token;
        var id = Guid.NewGuid();
        _tableRepoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>(), false))
            .ReturnsAsync((Models.Table?)null);

        // Act
        var act = () => _sut.FindById(id, ct);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
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

        _tableRepoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>(), false))
            .ReturnsAsync(existing);

        // Act
        await _sut.Update(id, updateDto, ct);

        // Assert
        existing.Number.Should().Be(10);
    }

    [Fact]
    public async Task Update_WhenTableNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        _tableRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), false))
            .ReturnsAsync((Models.Table?)null);

        // Act
        var act = () => _sut.Update(Guid.NewGuid(), new UpdateTable(99));

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Delete_WhenExists_ShouldRemoveAndSave()
    {
        // Arrange
        var ct = new CancellationTokenSource().Token;
        var id = Guid.NewGuid();
        var existing = new Models.Table { Id = id };
        _tableRepoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>(), false))
            .ReturnsAsync(existing);

        // Act
        var result = await _sut.Delete(id, ct);

        // Assert
        result.Should().BeTrue();
        _tableRepoMock.Verify(r => r.Remove(existing), Times.Once);
    }

    [Fact]
    public async Task Delete_WhenNotExists_ShouldThrowNotFoundException()
    {
        // Arrange
        _tableRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), false))
            .ReturnsAsync((Models.Table?)null);

        // Act
        var act = () => _sut.Delete(Guid.NewGuid());

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

}
