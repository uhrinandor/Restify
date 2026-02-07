using Microsoft.AspNetCore.Mvc;
using RestifyServer.Dto;
using RestifyServer.Interfaces.Services;
using RestifyServer.TypeContracts;

namespace RestifyServer.Controllers;



public class WaitersController(IWaiterService waiterService) : CrudController<Waiter, CreateWaiter, UpdateWaiter, FindWaiter>(waiterService)
{
    [HttpPut("{id}/password", Name = "UpdateWaiterPassword")]
    [ProducesResponseType(typeof(OkResult), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdatePassword([FromRoute] Guid id, [FromBody] UpdatePassword credentials, CancellationToken ct)
    {
        await waiterService.UpdatePassword(id, credentials, ct);
        return NoContent();
    }
}
