using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using RestifyServer.Dto;
using RestifyServer.Interfaces.Repositories;
using RestifyServer.Interfaces.Services;
using RestifyServer.TypeContracts;
using RestifyServer.Models.Enums;
using RestifyServer.Services;

namespace RestifyServer.Tests.Services;

public class AdminServiceTests
{
    private readonly Mock<IRepository<Models.Admin>> _adminRepository = new();
    private readonly Mock<IPasswordHasher<Models.Admin>> _passwordHasher = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly Mock<IEntityService<Models.Admin>> _entityService = new();

    private readonly AdminService _sut;

    public AdminServiceTests()
    {
        _sut = new AdminService(_adminRepository.Object, _entityService.Object, _passwordHasher.Object, _mapper.Object);
    }

    [Fact]
    public async Task CreateAdmin_Valid_SuccessfulSaving()
    {
        // Arrange
        var ct = new CancellationTokenSource().Token;

        var input = new CreateAdmin(Username: "endor", Password: "plain_pw", WriteMode: true);

        var hashed = "HASHED_PASSWORD";

        _passwordHasher
            .Setup(h => h.HashPassword(It.IsAny<RestifyServer.Models.Admin>(), input.Password))
            .Returns(hashed);

        var mapped = new Admin
        {
            // fill minimally needed fields
            Username = input.Username
        };

        _mapper
            .Setup(m => m.Map<Admin>(It.Is<object>(o => o is Models.Admin)))
            .Returns(mapped);

        Models.Admin? addedEntity = null;

        _adminRepository
            .Setup(r => r.Add(It.IsAny<RestifyServer.Models.Admin>()))
            .Callback<RestifyServer.Models.Admin>(a => addedEntity = a);

        // Act
        var result = await _sut.Create(input, ct);

        // Assert: returned mapped admin
        result.Should().BeSameAs(mapped);

        // Assert: repo.Add called once
        _adminRepository.Verify(r => r.Add(It.IsAny<RestifyServer.Models.Admin>()), Times.Once);

        // Assert: the entity passed to Add has correct fields set
        addedEntity.Should().NotBeNull();
        addedEntity!.Username.Should().Be(input.Username);
        addedEntity.AccessLevel.Should().Be(Permission.Write); // because WriteMode = true
        addedEntity.Password.Should().Be(hashed);

        // Assert: mapper called with the same entity we added
        _mapper.Verify(m => m.Map<Admin>(It.Is<RestifyServer.Models.Admin>(a => ReferenceEquals(a, addedEntity))), Times.Once);

        // Assert: hasher called with the same entity + password
        _passwordHasher.Verify(h =>
            h.HashPassword(It.Is<RestifyServer.Models.Admin>(a => ReferenceEquals(a, addedEntity)), input.Password),
            Times.Once);
    }

    [Fact]
    public async Task FindById_Existing_ReturnsMappedAdmin()
    {
        // Arrange
        var ct = CancellationToken.None;
        var id = Guid.NewGuid();

        var dbAdmin = new Models.Admin
        {
            Id = id,
            Username = "endor",
            AccessLevel = Permission.Read
        };

        var mapped = new Admin { Username = "endor" };

        _mapper.Setup(m => m.Map<Admin>(It.IsAny<object>()))
               .Returns(mapped);

        _entityService.Setup(r => r.LoadEntity(id, ct))
                  .ReturnsAsync(dbAdmin);

        // Act
        var result = await _sut.FindById(id, ct);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeSameAs(mapped);

        _mapper.Verify(m => m.Map<Admin>(It.Is<object>(o => ReferenceEquals(o, dbAdmin))), Times.Once);
        _entityService.Verify(r => r.LoadEntity(id, ct), Times.Once);
    }

    [Fact]
    public async Task UpdateAdmin_Valid_UpdatesFields_SuccessfulSaving()
    {
        // Arrange
        var ct = CancellationToken.None;
        var id = Guid.NewGuid();

        var dbAdmin = new Models.Admin
        {
            Id = id,
            Username = "old",
            AccessLevel = Permission.Read
        };

        var update = new UpdateAdmin
        (
            Username: "new",
            WriteMode: true
        );

        var mapped = new Admin { Username = "new" };

        _mapper.Setup(m => m.Map<Admin>(It.IsAny<object>()))
               .Returns(mapped);

        _entityService.Setup(r => r.LoadEntityAsync(id, ct))
            .ReturnsAsync(dbAdmin);

        // Act
        var result = await _sut.Update(id, update, ct);

        // Assert
        dbAdmin.Username.Should().Be("new");
        dbAdmin.AccessLevel.Should().Be(Permission.Write);

        _mapper.Verify(m => m.Map<Admin>(It.Is<object>(o => ReferenceEquals(o, dbAdmin))), Times.Once);

        result.Should().NotBeNull();
        result.Should().BeSameAs(mapped);
    }

    [Fact]
    public async Task UpdateAdmin_Partial_NoChanges_StillSavesAndReturnsMapped()
    {
        // Arrange
        var ct = CancellationToken.None;
        var id = Guid.NewGuid();

        var dbAdmin = new Models.Admin
        {
            Id = id,
            Username = "same",
            AccessLevel = Permission.Read
        };

        var update = new UpdateAdmin
        (
            Username: null,
            WriteMode: null
        );

        var mapped = new Admin { Username = "same" };

        _mapper.Setup(m => m.Map<Admin>(It.IsAny<object>()))
               .Returns(mapped);

        _entityService.Setup(r => r.LoadEntityAsync(id, ct))
            .ReturnsAsync(dbAdmin);

        // Act
        var result = await _sut.Update(id, update, ct);

        // Assert
        dbAdmin.Username.Should().Be("same");
        dbAdmin.AccessLevel.Should().Be(Permission.Read);


        result.Should().NotBeNull();
        result.Should().BeSameAs(mapped);
    }

    [Fact]
    public async Task DeleteAdmin_Valid_SuccessfulSaving()
    {
        // Arrange
        var ct = CancellationToken.None;
        var id = Guid.NewGuid();

        var dbAdmin = new Models.Admin { Id = id, Username = "endor" };

        _entityService.Setup(r => r.LoadEntityAsync(id, ct))
            .ReturnsAsync(dbAdmin);

        // Act
        var result = await _sut.Delete(id, ct);

        // Assert
        result.Should().BeTrue();

        _adminRepository.Verify(r => r.Remove(It.Is<Models.Admin>(a => ReferenceEquals(a, dbAdmin))), Times.Once);
    }

    [Fact]
    public async Task UpdatePassword_Valid_SuccessfulSaving()
    {
        // Arrange
        var ct = CancellationToken.None;
        var id = Guid.NewGuid();

        var dbAdmin = new Models.Admin { Id = id, Username = "endor", Password = "HASH_OLD" };

        var dto = new UpdatePassword
        (
            OldPassword: "old",
            NewPassword: "new"
        );

        _entityService.Setup(r => r.LoadEntityAsync(id, ct))
            .ReturnsAsync(dbAdmin);

        _passwordHasher
            .Setup(h => h.VerifyHashedPassword(dbAdmin, dbAdmin.Password, dto.OldPassword))
            .Returns(PasswordVerificationResult.Success);

        _passwordHasher
            .Setup(h => h.HashPassword(dbAdmin, dto.NewPassword))
            .Returns("HASH_NEW");

        // Act
        var result = await _sut.UpdatePassword(id, dto, ct);

        // Assert
        result.Should().BeTrue();
        dbAdmin.Password.Should().Be("HASH_NEW");

    }

    [Fact]
    public async Task UpdatePassword_InvalidOldPassword_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var ct = CancellationToken.None;
        var id = Guid.NewGuid();

        var dbAdmin = new Models.Admin { Id = id, Password = "HASH_OLD" };

        var dto = new UpdatePassword
        (
            OldPassword: "wrong",
            NewPassword: "new"
        );

        _entityService.Setup(r => r.LoadEntityAsync(id, ct))
            .ReturnsAsync(dbAdmin);

        _passwordHasher
            .Setup(h => h.VerifyHashedPassword(dbAdmin, dbAdmin.Password, dto.OldPassword))
            .Returns(PasswordVerificationResult.Failed);

        // Act
        Func<Task> act = async () => _ = await _sut.UpdatePassword(id, dto, ct);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>();

        _passwordHasher.Verify(h => h.HashPassword(It.IsAny<Models.Admin>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task List_ValidQuery_ReturnsMappedAdmins()
    {
        // Arrange
        var ct = CancellationToken.None;

        var query = new FindAdmin
        (
            Username: "endor",
            AccessLevel: Permission.Write,
            Id: null
        );

        var dbAdmins = new List<Models.Admin>
        {
            new Models.Admin { Id = Guid.NewGuid(), Username = "endor", AccessLevel = Permission.Write }
        };

        var mappedAdmins = new List<Admin>
        {
            new Admin { Username = "endor" }
        };

        _adminRepository
            .Setup(r => r.ListAsync(It.IsAny<Expression<Func<Models.Admin, bool>>>(), ct, true))
            .ReturnsAsync(dbAdmins);

        _mapper
            .Setup(m => m.Map<List<Admin>>(dbAdmins))
            .Returns(mappedAdmins);

        // Act
        var result = await _sut.List(query, ct);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.Should().BeSameAs(mappedAdmins);

        _adminRepository.Verify(r => r.ListAsync(
                It.IsAny<Expression<Func<Models.Admin, bool>>>(),
                ct,
                true),
            Times.Once);

        _mapper.Verify(m => m.Map<List<Admin>>(It.Is<List<Models.Admin>>(l => l.Count == 1)), Times.Once);
    }
}
