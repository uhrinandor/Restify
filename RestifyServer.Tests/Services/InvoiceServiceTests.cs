using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Moq;
using RestifyServer.Dto;
using RestifyServer.Interfaces.Repositories;
using RestifyServer.Interfaces.Services;
using RestifyServer.Models.Enums;
using RestifyServer.Services;
using RestifyServer.TypeContracts;

namespace RestifyServer.Tests.Services;

public class InvoiceServiceTests
{
    private readonly Mock<IRepository<Models.Invoice>> _invoiceRepository = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly Mock<IEntityService<Models.Invoice>> _entityService = new();
    private readonly Mock<IEntityService<Models.Waiter>> _waiterEntityService = new();
    private readonly Mock<IEntityService<Models.Table>> _tableEntityService = new();
    private readonly InvoiceService _sut;

    public InvoiceServiceTests()
    {
        _sut = new InvoiceService(
            _invoiceRepository.Object,
            _entityService.Object,
            _waiterEntityService.Object,
            _tableEntityService.Object,
            _mapper.Object);
    }

    [Fact]
    public async Task Create_ValidInvoice_ReturnsMappedInvoice()
    {
        // Arrange
        var ct = CancellationToken.None;
        var waiterId = Guid.NewGuid();
        var tableId = Guid.NewGuid();

        var input = new CreateInvoice(
            new Waiter { Id = waiterId },
            new Table { Id = tableId }
        );

        var dbWaiter = new Models.Waiter { Id = waiterId };
        var dbTable = new Models.Table { Id = tableId };

        var mappedResult = new Invoice
        {
            Id = Guid.NewGuid(),
            Waiter = new Waiter { Id = waiterId },
            Table = new Table { Id = tableId }
        };

        _waiterEntityService.Setup(s => s.LoadEntityAsync(waiterId, ct))
            .ReturnsAsync(dbWaiter);

        _tableEntityService.Setup(s => s.LoadEntityAsync(tableId, ct))
            .ReturnsAsync(dbTable);

        _mapper.Setup(m => m.Map<Invoice>(It.IsAny<Models.Invoice>()))
            .Returns(mappedResult);

        // Act
        var result = await _sut.Create(input, ct);

        // Assert
        result.Should().BeSameAs(mappedResult);

        _invoiceRepository.Verify(r => r.Add(It.Is<Models.Invoice>(inv =>
            inv.Waiter == dbWaiter &&
            inv.Table == dbTable
        )), Times.Once);
    }

    [Fact]
    public async Task Update_AllFields_UpdatesReferences()
    {
        // Arrange
        var ct = new CancellationTokenSource().Token;
        var id = Guid.NewGuid();

        var newWaiterId = Guid.NewGuid();
        var newTableId = Guid.NewGuid();

        var dbInvoice = new Models.Invoice
        {
            Id = id,
            Waiter = new Models.Waiter { Id = Guid.NewGuid() },
            Table = new Models.Table { Id = Guid.NewGuid() }
        };

        var newWaiter = new Models.Waiter { Id = newWaiterId };
        var newTable = new Models.Table { Id = newTableId };

        var update = new UpdateInvoice(
            Waiter: new Waiter { Id = newWaiterId },
            Table: new Table { Id = newTableId }
        );

        _entityService.Setup(s => s.LoadEntityAsync(id, ct))
            .ReturnsAsync(dbInvoice);

        _waiterEntityService.Setup(s => s.LoadEntityAsync(newWaiterId, ct))
            .ReturnsAsync(newWaiter);

        _tableEntityService.Setup(s => s.LoadEntityAsync(newTableId, ct))
            .ReturnsAsync(newTable);

        // Act
        await _sut.Update(id, update, ct);

        // Assert
        dbInvoice.Waiter.Should().BeSameAs(newWaiter);
        dbInvoice.Table.Should().BeSameAs(newTable);
    }

    [Fact]
    public async Task Update_PartialData_OnlyUpdatesProvidedReferences()
    {
        // Arrange
        var ct = new CancellationTokenSource().Token;
        var id = Guid.NewGuid();

        var originalWaiter = new Models.Waiter { Id = Guid.NewGuid() };
        var originalTable = new Models.Table { Id = Guid.NewGuid() };

        var dbInvoice = new Models.Invoice
        {
            Id = id,
            Waiter = originalWaiter,
            Table = originalTable
        };

        var newWaiterId = Guid.NewGuid();
        var newWaiter = new Models.Waiter { Id = newWaiterId };

        var update = new UpdateInvoice(
            Waiter: new Waiter { Id = newWaiterId },
            Table: null
        );

        _entityService.Setup(s => s.LoadEntityAsync(id, ct))
            .ReturnsAsync(dbInvoice);

        _waiterEntityService.Setup(s => s.LoadEntityAsync(newWaiterId, ct))
            .ReturnsAsync(newWaiter);

        // Act
        await _sut.Update(id, update, ct);

        // Assert
        dbInvoice.Waiter.Should().BeSameAs(newWaiter);
        dbInvoice.Table.Should().BeSameAs(originalTable); // unchanged
    }

    [Fact]
    public async Task List_FiltersApplied_ReturnsMappedList()
    {
        // Arrange
        var ct = new CancellationTokenSource().Token;

        var invoiceId = Guid.NewGuid();
        var waiterId = Guid.NewGuid();
        var tableId = Guid.NewGuid();

        var query = new FindInvoice(
            Id: invoiceId,
            Waiter: new FindWaiter(Id: waiterId, null, null),
            Table: new FindTable(Id: tableId, null),
            Payment: PaymentType.Cash,
            IsClosed: true
        );

        var dbList = new List<Models.Invoice>
        {
            new() { Id = invoiceId }
        };

        var mappedList = new List<Invoice>
        {
            new()
            {
                Id = invoiceId,
                Waiter = new Waiter { Id = waiterId },
                Table = new Table { Id = tableId }
            }
        };

        _invoiceRepository.Setup(r => r.ListAsync(It.IsAny<Expression<Func<Models.Invoice, bool>>>(), It.IsAny<CancellationToken>(), true))
            .ReturnsAsync(dbList);

        _mapper.Setup(m => m.Map<List<Invoice>>(dbList))
            .Returns(mappedList);

        // Act
        var result = await _sut.List(query, ct);

        // Assert
        result.Should().HaveCount(1);
        result.First().Id.Should().Be(invoiceId);
    }

    [Fact]
    public async Task FindById_ValidId_ReturnsMappedInvoice()
    {
        // Arrange
        var ct = new CancellationTokenSource().Token;
        var id = Guid.NewGuid();

        var dbInvoice = new Models.Invoice { Id = id };

        var mapped = new Invoice
        {
            Id = id,
            Waiter = new Waiter { Id = Guid.NewGuid() },
            Table = new Table { Id = Guid.NewGuid() }
        };

        _entityService.Setup(s => s.LoadEntity(id, ct))
            .ReturnsAsync(dbInvoice);

        _mapper.Setup(m => m.Map<Invoice>(dbInvoice))
            .Returns(mapped);

        // Act
        var result = await _sut.FindById(id, ct);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(id);
    }

    [Fact]
    public async Task Delete_Existing_RemovesAndReturnsTrue()
    {
        // Arrange
        var ct = new CancellationTokenSource().Token;
        var id = Guid.NewGuid();

        var dbInvoice = new Models.Invoice { Id = id };

        _entityService.Setup(s => s.LoadEntityAsync(id, ct))
            .ReturnsAsync(dbInvoice);

        // Act
        var result = await _sut.Delete(id, ct);

        // Assert
        result.Should().BeTrue();
        _invoiceRepository.Verify(r => r.Remove(dbInvoice), Times.Once);
    }
}
