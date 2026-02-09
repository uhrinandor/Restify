using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Moq;
using RestifyServer.Dto;
using RestifyServer.Exceptions;
using RestifyServer.Interfaces.Repositories;
using RestifyServer.Interfaces.Services;
using RestifyServer.Models.Enums;
using RestifyServer.Services;
using RestifyServer.TypeContracts;

namespace RestifyServer.Tests.Services;

public class OrderServiceTests
{
    private readonly Mock<IRepository<Models.Order>> _orderRepo = new();
    private readonly Mock<IEntityService<Models.Order>> _entityService = new();
    private readonly Mock<IEntityService<Models.Product>> _productEntityService = new();
    private readonly Mock<IEntityService<Models.Invoice>> _invoiceEntityService = new();
    private readonly Mock<IMapper> _mapper = new();

    private readonly OrderService _sut;

    public OrderServiceTests()
    {
        _sut = new OrderService(
            _orderRepo.Object,
            _entityService.Object,
            _productEntityService.Object,
            _invoiceEntityService.Object,
            _mapper.Object
        );
    }

    [Fact]
    public async Task Create_ValidInvoice_AddsOrderWithLoadedRefs_AndReturnsMappedContract()
    {
        // Arrange
        var ct = CancellationToken.None;

        var productId = Guid.NewGuid();
        var invoiceId = Guid.NewGuid();

        var input = new CreateOrder(
            Product: new FindEntity(Id: productId),
            Invoice: new FindEntity(Id: invoiceId)
        );

        var dbProduct = new Models.Product { Id = productId };
        var dbInvoice = new Models.Invoice { Id = invoiceId, ClosedAt = null };

        var mapped = new Order
        {
            Id = Guid.NewGuid(),
            Product = new Product { Id = productId },
            Invoice = new NestedInvoice { Id = invoiceId }
        };

        _productEntityService.Setup(s => s.LoadEntityAsync(productId, ct))
            .ReturnsAsync(dbProduct);

        _invoiceEntityService.Setup(s => s.LoadEntityAsync(invoiceId, ct))
            .ReturnsAsync(dbInvoice);

        _mapper.Setup(m => m.Map<Order>(It.IsAny<Models.Order>()))
            .Returns(mapped);

        // Act (calls base Create -> derived CreateEntity)
        var result = await _sut.Create(input, ct);

        // Assert
        result.Should().BeSameAs(mapped);

        _orderRepo.Verify(r => r.Add(It.Is<Models.Order>(o =>
            o.Product == dbProduct &&
            o.Invoice == dbInvoice
        )), Times.Once);
    }

    [Fact]
    public async Task Create_ClosedInvoice_ThrowsBusinessLogicException_AndDoesNotAdd()
    {
        // Arrange
        var ct = CancellationToken.None;

        var productId = Guid.NewGuid();
        var invoiceId = Guid.NewGuid();

        var input = new CreateOrder(
            Product: new FindEntity(Id: productId),
            Invoice: new FindEntity(Id: invoiceId)
        );

        _productEntityService.Setup(s => s.LoadEntityAsync(productId, ct))
            .ReturnsAsync(new Models.Product { Id = productId });

        _invoiceEntityService.Setup(s => s.LoadEntityAsync(invoiceId, ct))
            .ReturnsAsync(new Models.Invoice { Id = invoiceId, ClosedAt = DateTime.UtcNow });

        // Act
        var act = () => _sut.Create(input, ct);

        // Assert
        await act.Should().ThrowAsync<BuisnessLogicException>()
            .WithMessage("Invoice cannot be closed!*");

        _orderRepo.Verify(r => r.Add(It.IsAny<Models.Order>()), Times.Never);
    }

    [Fact]
    public async Task Update_StatusOnly_UpdatesStatus_DoesNotLoadInvoiceOrProduct()
    {
        // Arrange
        var ct = new CancellationTokenSource().Token;
        var orderId = Guid.NewGuid();

        var dbOrder = new Models.Order
        {
            Id = orderId,
            Status = OrderStatus.New,
            Product = new Models.Product { Id = Guid.NewGuid() },
            Invoice = new Models.Invoice { Id = Guid.NewGuid() }
        };

        var update = new UpdateOrder(
            Status: OrderStatus.Done,
            Product: null,
            Invoice: null
        );

        _entityService.Setup(s => s.LoadEntityAsync(orderId, ct))
            .ReturnsAsync(dbOrder);

        _mapper.Setup(m => m.Map<Order>(dbOrder))
            .Returns(new Order { Id = orderId });

        // Act (calls base Update -> derived SetEntityProperties)
        await _sut.Update(orderId, update, ct);

        // Assert
        dbOrder.Status.Should().Be(OrderStatus.Done);

        _invoiceEntityService.Verify(s => s.LoadEntityAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _productEntityService.Verify(s => s.LoadEntityAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Update_InvoiceProvided_LoadsInvoice_RejectsClosedInvoice()
    {
        // Arrange
        var ct = new CancellationTokenSource().Token;
        var orderId = Guid.NewGuid();
        var newInvoiceId = Guid.NewGuid();

        var dbOrder = new Models.Order { Id = orderId, Invoice = new Models.Invoice { Id = Guid.NewGuid() } };

        var update = new UpdateOrder(
            Status: null,
            Product: null,
            Invoice: new FindEntity(Id: newInvoiceId)
        );

        _entityService.Setup(s => s.LoadEntityAsync(orderId, ct))
            .ReturnsAsync(dbOrder);

        _invoiceEntityService.Setup(s => s.LoadEntityAsync(newInvoiceId, ct))
            .ReturnsAsync(new Models.Invoice { Id = newInvoiceId, ClosedAt = DateTime.UtcNow });

        // Act
        var act = () => _sut.Update(orderId, update, ct);

        // Assert
        await act.Should().ThrowAsync<BuisnessLogicException>()
            .WithMessage("Invoice cannot be closed!*");
    }

    [Fact]
    public async Task Update_ProductAndInvoiceOpen_UpdatesRefs()
    {
        // Arrange
        var ct = new CancellationTokenSource().Token;
        var orderId = Guid.NewGuid();

        var newProductId = Guid.NewGuid();
        var newInvoiceId = Guid.NewGuid();

        var dbOrder = new Models.Order
        {
            Id = orderId,
            Product = new Models.Product { Id = Guid.NewGuid() },
            Invoice = new Models.Invoice { Id = Guid.NewGuid(), ClosedAt = null }
        };

        var dbNewProduct = new Models.Product { Id = newProductId };
        var dbNewInvoice = new Models.Invoice { Id = newInvoiceId, ClosedAt = null };

        var update = new UpdateOrder(
            Status: null,
            Product: new FindEntity(Id: newProductId),
            Invoice: new FindEntity(Id: newInvoiceId)
        );

        _entityService.Setup(s => s.LoadEntityAsync(orderId, ct))
            .ReturnsAsync(dbOrder);

        _productEntityService.Setup(s => s.LoadEntityAsync(newProductId, ct))
            .ReturnsAsync(dbNewProduct);

        _invoiceEntityService.Setup(s => s.LoadEntityAsync(newInvoiceId, ct))
            .ReturnsAsync(dbNewInvoice);

        _mapper.Setup(m => m.Map<Order>(dbOrder))
            .Returns(new Order { Id = orderId });

        // Act
        await _sut.Update(orderId, update, ct);

        // Assert
        dbOrder.Product.Should().BeSameAs(dbNewProduct);
        dbOrder.Invoice.Should().BeSameAs(dbNewInvoice);
    }

    [Fact]
    public async Task List_AppliesFilterAndMaps()
    {
        // Arrange
        var ct = new CancellationTokenSource().Token;

        var orderId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        var query = new FindOrder(
            Id: orderId,
            Status: OrderStatus.New,
            Product: new FindProduct(productId, null, null, null, null),
            Invoice: null
        );

        var dbList = new List<Models.Order> { new() { Id = orderId } };

        var mapped = new List<Order> { new() { Id = orderId } };

        _orderRepo.Setup(r => r.ListAsync(It.IsAny<Expression<Func<Models.Order, bool>>>(), It.IsAny<CancellationToken>(), true))
            .ReturnsAsync(dbList);

        _mapper.Setup(m => m.Map<List<Order>>(dbList))
            .Returns(mapped);

        // Act (calls base List -> derived CreateQuery)
        var result = await _sut.List(query, ct);

        // Assert
        result.Should().BeSameAs(mapped);
        result.Should().HaveCount(1);
        result[0].Id.Should().Be(orderId);
    }

    [Fact]
    public async Task FindById_ValidId_LoadsAndMaps()
    {
        // Arrange
        var ct = new CancellationTokenSource().Token;
        var orderId = Guid.NewGuid();

        var dbOrder = new Models.Order { Id = orderId };
        var mapped = new Order { Id = orderId };

        _entityService.Setup(s => s.LoadEntity(orderId, ct))
            .ReturnsAsync(dbOrder);

        _mapper.Setup(m => m.Map<Order>(dbOrder))
            .Returns(mapped);

        // Act
        var result = await _sut.FindById(orderId, ct);

        // Assert
        result.Should().BeSameAs(mapped);
        result.Id.Should().Be(orderId);
    }

    [Fact]
    public async Task Delete_Existing_LoadsAndRemoves_ReturnsTrue()
    {
        // Arrange
        var ct = new CancellationTokenSource().Token;
        var orderId = Guid.NewGuid();

        var dbOrder = new Models.Order { Id = orderId };

        _entityService.Setup(s => s.LoadEntityAsync(orderId, ct))
            .ReturnsAsync(dbOrder);

        // Act
        var result = await _sut.Delete(orderId, ct);

        // Assert
        result.Should().BeTrue();
        _orderRepo.Verify(r => r.Remove(dbOrder), Times.Once);
    }
}
