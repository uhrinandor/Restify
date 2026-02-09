using FluentAssertions;
using Moq;
using RestifyServer.Exceptions;
using RestifyServer.Interfaces.Repositories;
using RestifyServer.Models;
using RestifyServer.Services;
using Xunit;

namespace RestifyServer.Tests.Services;

public class EntityServiceTests
{
    private readonly Mock<IRepository<Admin>> _repository = new();
    private readonly EntityService<Admin> _sut;

    public EntityServiceTests()
    {
        _sut = new EntityService<Admin>(_repository.Object);
    }

    [Fact]
    public async Task LoadEntityAsync_ExistingEntity_ReturnsEntity()
    {
        // Arrange
        var ct = CancellationToken.None;
        var id = Guid.NewGuid();

        var admin = new Admin
        {
            Id = id,
            Username = "endor"
        };

        _repository
            .Setup(r => r.GetByIdAsync(id, ct, false))
            .ReturnsAsync(admin);

        // Act
        var result = await _sut.LoadEntityAsync(id, ct);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeSameAs(admin);

        _repository.Verify(
            r => r.GetByIdAsync(id, ct, false),
            Times.Once);
    }

    [Fact]
    public async Task LoadEntityAsync_NotFound_ThrowsNotFoundException()
    {
        // Arrange
        var ct = CancellationToken.None;
        var id = Guid.NewGuid();

        _repository
            .Setup(r => r.GetByIdAsync(id, ct, false))
            .ReturnsAsync((Admin?)null);

        // Act
        Func<Task> act = async () => await _sut.LoadEntityAsync(id, ct);

        // Assert
        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage($"Admin was not found, Id: {id}");

        _repository.Verify(
            r => r.GetByIdAsync(id, ct, false),
            Times.Once);
    }

    [Fact]
    public async Task LoadEntity_ExistingEntity_ReturnsEntity()
    {
        // Arrange
        var ct = CancellationToken.None;
        var id = Guid.NewGuid();

        var admin = new Admin
        {
            Id = id,
            Username = "endor"
        };

        _repository
            .Setup(r => r.GetByIdAsync(id, ct))
            .ReturnsAsync(admin);

        // Act
        var result = await _sut.LoadEntity(id, ct);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeSameAs(admin);

        _repository.Verify(
            r => r.GetByIdAsync(id, ct),
            Times.Once);
    }

    [Fact]
    public async Task LoadEntity_NotFound_ThrowsNotFoundException()
    {
        // Arrange
        var ct = CancellationToken.None;
        var id = Guid.NewGuid();

        _repository
            .Setup(r => r.GetByIdAsync(id, ct))
            .ReturnsAsync((Admin?)null);

        // Act
        Func<Task> act = async () => await _sut.LoadEntity(id, ct);

        // Assert
        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage($"Admin was not found, Id: {id}");

        _repository.Verify(
            r => r.GetByIdAsync(id, ct),
            Times.Once);
    }
}
