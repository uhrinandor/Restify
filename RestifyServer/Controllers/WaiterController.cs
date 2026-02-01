using Microsoft.AspNetCore.Mvc;
using RestifyServer.Dto;
using RestifyServer.Interfaces.Services;
using RestifyServer.TypeContracts;

namespace RestifyServer.Controllers;


[ApiController]
[Route("api/waiters")]
public class WaiterController(IWaiterService waiterService) : ControllerBase
{
    [HttpPost(Name = "CreateWaiter")]
    [ProducesResponseType(typeof(Waiter), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<Waiter>> Create([FromBody] CreateWaiter data, CancellationToken ct)
    {
        var created = await waiterService.Create(data, ct);
        return CreatedAtAction(nameof(Create), new { id = created.Id }, created);
    }
    [HttpGet(Name = "ListWaiters")]
    [ProducesResponseType(typeof(List<Waiter>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<Waiter>>> List([FromQuery] FindWaiter query, CancellationToken ct)
    {
        var list = await waiterService.List(query, ct);
        return Ok(list);
    }

    [HttpGet("{id}", Name = "GetWaiter")]
    [ProducesResponseType(typeof(Waiter), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Waiter>> Get([FromRoute] Guid id, CancellationToken ct)
    {
        var admin = await waiterService.FindById(id, ct);
        if (admin == null) return NotFound();
        return Ok(admin);
    }
    [HttpPut("{id}", Name = "UpdateWaiter")]
    [ProducesResponseType(typeof(Waiter), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<Waiter>> Update([FromRoute] Guid id, [FromBody] UpdateWaiter patch, CancellationToken ct)
    {
        var admin = await waiterService.Update(id, patch, ct);
        return Ok(admin);
    }
    [HttpDelete("{id}", Name = "DeleteWaiter")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete([FromRoute] Guid id, CancellationToken ct)
    {
        await waiterService.Delete(id, ct);
        return NoContent();
    }
    [HttpPut("{id}/password", Name = "UpdateWaiterPassword")]
    [ProducesResponseType(typeof(OkResult), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdatePassword([FromRoute] Guid id, [FromBody] UpdatePassword credentials, CancellationToken ct)
    {
        await waiterService.UpdatePassword(id, credentials, ct);
        return NoContent();
    }
}
