using Microsoft.AspNetCore.Mvc;
using RestifyServer.Dto;
using RestifyServer.Interfaces.Services;
using RestifyServer.TypeContracts;

namespace RestifyServer.Controllers;


public class AdminsController(IAdminService adminService)
    : CrudController<Admin, CreateAdmin, UpdateAdmin, FindAdmin>(adminService)
{
    [HttpPut("{id}/password", Name = "UpdateAdminPassword")]
    [ProducesResponseType(typeof(OkResult), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdatePassword([FromRoute] Guid id, [FromBody] UpdatePassword credentials, CancellationToken ct)
    {
        await adminService.UpdatePassword(id, credentials, ct);
        return NoContent();
    }
}
