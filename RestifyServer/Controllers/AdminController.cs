using Microsoft.AspNetCore.Mvc;
using RestifyServer.Dto;
using RestifyServer.Interfaces.Services;
using RestifyServer.TypeContracts;

namespace RestifyServer.Controllers;

[ApiController]
[Route("api/admins")]
public class AdminController(IAdminService adminService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(Admin), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<Admin>> Create([FromBody] CreateAdmin data, CancellationToken ct)
    {
        var created = await adminService.Create(data, ct);
        return CreatedAtAction(nameof(Create), new { id = created.Id }, created);
    }
    [HttpGet]
    [ProducesResponseType(typeof(List<Admin>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<Admin>>> List([FromQuery] FindAdmin query, CancellationToken ct)
    {
        var list = await adminService.List(query, ct);
        return Ok(list);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Admin), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Admin>> Get([FromRoute] Guid id, CancellationToken ct)
    {
        var admin = await adminService.FindById(id, ct);
        if (admin == null) return NotFound();
        return Ok(admin);
    }
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Admin), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<Admin>> Update([FromRoute] Guid id, [FromBody] UpdateAdmin patch, CancellationToken ct)
    {
        var admin = await adminService.Update(id, patch, ct);
        return Ok(admin);
    }
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete([FromRoute] Guid id, CancellationToken ct)
    {
        await adminService.Delete(id, ct);
        return NoContent();
    }
    [HttpPut("{id}/password")]
    [ProducesResponseType(typeof(OkResult), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdatePassword([FromRoute] Guid id, [FromBody] UpdateAdminPassword credentials, CancellationToken ct)
    {
        await adminService.UpdatePassword(id, credentials, ct);
        return NoContent();
    }
}
