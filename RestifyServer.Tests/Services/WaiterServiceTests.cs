using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using RestifyServer.Dto;
using RestifyServer.Exceptions;
using RestifyServer.Interfaces.Repositories;
using RestifyServer.TypeContracts;
using RestifyServer.Services;

namespace RestifyServer.Tests.Services;

public class WaiterServiceTests
{
    private readonly Mock<IRepository<Models.Waiter>> _waiterRepository = new();
    private readonly Mock<IPasswordHasher<Models.Waiter>> _passwordHasher = new();
    private readonly Mock<IMapper> _mapper = new();

    private readonly WaiterService _sut;

    public WaiterServiceTests()
    {
        _sut = new WaiterService(_waiterRepository.Object, _passwordHasher.Object, _mapper.Object);
    }

    [Fact]
    public async Task CreateWaiter_Valid_SuccessfulSaving()
    {
        // Arrange
        var ct = new CancellationTokenSource().Token;
        var input = new CreateWaiter(Username: "waiter1", Name: "John Doe", Password: "plain_password");
        var hashed = "HASHED_PASSWORD";

        _passwordHasher
            .Setup(h => h.HashPassword(It.IsAny<Models.Waiter>(), input.Password))
            .Returns(hashed);

        var mappedResult = new Waiter { Username = input.Username, Name = input.Name };

        _mapper
            .Setup(m => m.Map<Waiter>(It.IsAny<Models.Waiter>()))
            .Returns(mappedResult);

        Models.Waiter? addedEntity = null;
        _waiterRepository
            .Setup(r => r.Add(It.IsAny<Models.Waiter>()))
            .Callback<Models.Waiter>(w => addedEntity = w);

        // Act
        var result = await _sut.Create(input, ct);

        // Assert
        result.Should().BeSameAs(mappedResult);
        _waiterRepository.Verify(r => r.Add(It.IsAny<Models.Waiter>()), Times.Once);

        addedEntity.Should().NotBeNull();
        addedEntity!.Username.Should().Be(input.Username);
        addedEntity.Name.Should().Be(input.Name);
        addedEntity.Password.Should().Be(hashed);
    }

    [Fact]
    public async Task FindById_Existing_ReturnsMappedWaiter()
    {
        // Arrange
        var ct = CancellationToken.None;
        var id = Guid.NewGuid();
        var dbWaiter = new Models.Waiter { Id = id, Username = "waiter1" };
        var mapped = new Waiter { Username = "waiter1" };

        _waiterRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), ct, true))
                  .ReturnsAsync(dbWaiter);

        _mapper.Setup(m => m.Map<Waiter>(dbWaiter)).Returns(mapped);

        // Act
        var result = await _sut.FindById(id, ct);

        // Assert
        result.Should().BeSameAs(mapped);
        _waiterRepository.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), ct, true), Times.Once);
    }

    [Fact]
    public async Task FindById_NotFound_ThrowsNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();
        _waiterRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), false))
                  .ReturnsAsync((Models.Waiter?)null);

        // Act
        Func<Task> act = async () => await _sut.FindById(id);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task UpdateWaiter_Valid_UpdatesFields()
    {
        // Arrange
        var ct = new CancellationTokenSource().Token;
        var id = Guid.NewGuid();
        var dbWaiter = new Models.Waiter { Id = id, Username = "old", Name = "Old Name" };
        var update = new UpdateWaiter(Username: "new", Name: "New Name");
        var mapped = new Waiter { Username = "new", Name = "New Name" };

        _waiterRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), false))
                  .ReturnsAsync(dbWaiter);
        _mapper.Setup(m => m.Map<Waiter>(dbWaiter)).Returns(mapped);

        // Act
        var result = await _sut.Update(id, update, ct);

        // Assert
        dbWaiter.Username.Should().Be("new");
        dbWaiter.Name.Should().Be("New Name");
        result.Should().BeSameAs(mapped);
    }

    [Fact]
    public async Task DeleteWaiter_Valid_RemovesFromRepo()
    {
        // Arrange
        var ct = new CancellationTokenSource().Token;
        var id = Guid.NewGuid();
        var dbWaiter = new Models.Waiter { Id = id };
        _waiterRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), false))
                  .ReturnsAsync(dbWaiter);

        // Act
        var result = await _sut.Delete(id, ct);

        // Assert
        result.Should().BeTrue();
        _waiterRepository.Verify(r => r.Remove(dbWaiter), Times.Once);
    }

    [Fact]
    public async Task UpdatePassword_Valid_HashesNewPassword()
    {
        // Arrange
        var ct = new CancellationTokenSource().Token;
        var id = Guid.NewGuid();
        var dbWaiter = new Models.Waiter { Id = id, Password = "OLD_HASH" };
        var dto = new UpdatePassword("old_plain", "new_plain");

        _waiterRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), false))
                  .ReturnsAsync(dbWaiter);

        _passwordHasher.Setup(h => h.VerifyHashedPassword(dbWaiter, "OLD_HASH", "old_plain"))
                       .Returns(PasswordVerificationResult.Success);
        _passwordHasher.Setup(h => h.HashPassword(dbWaiter, "new_plain"))
                       .Returns("NEW_HASH");

        // Act
        var result = await _sut.UpdatePassword(id, dto, ct);

        // Assert
        result.Should().BeTrue();
        dbWaiter.Password.Should().Be("NEW_HASH");
    }

    [Fact]
    public async Task UpdatePassword_WrongPassword_ThrowsUnauthorized()
    {
        // Arrange
        var dbWaiter = new Models.Waiter { Password = "OLD_HASH" };
        var dto = new UpdatePassword("wrong", "new");

        _waiterRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), false))
                  .ReturnsAsync(dbWaiter);
        _passwordHasher.Setup(h => h.VerifyHashedPassword(dbWaiter, "OLD_HASH", "wrong"))
                       .Returns(PasswordVerificationResult.Failed);

        // Act
        Func<Task> act = async () => await _sut.UpdatePassword(Guid.NewGuid(), dto);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task List_WithFilters_ReturnsFilteredResults()
    {
        // Arrange
        var ct = new CancellationTokenSource().Token;
        var query = new FindWaiter(Username: "john", Name: "John", id: null);
        var dbList = new List<Models.Waiter> { new Models.Waiter { Username = "john" } };
        var mappedList = new List<Waiter> { new Waiter { Username = "john" } };

        _waiterRepository.Setup(r => r.ListAsync(It.IsAny<Expression<Func<Models.Waiter, bool>>>(), It.IsAny<CancellationToken>(), true))
                   .ReturnsAsync(dbList);
        _mapper.Setup(m => m.Map<List<Waiter>>(dbList)).Returns(mappedList);

        // Act
        var result = await _sut.List(query, ct);

        // Assert
        result.Should().BeSameAs(mappedList);
        _waiterRepository.Verify(r => r.ListAsync(It.IsAny<Expression<Func<Models.Waiter, bool>>>(), It.IsAny<CancellationToken>(), true), Times.Once);
    }
}
