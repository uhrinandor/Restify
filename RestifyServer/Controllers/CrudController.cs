using Microsoft.AspNetCore.Mvc;
using RestifyServer.Interfaces.Services;
using RestifyServer.TypeContracts;

namespace RestifyServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class CrudController<TContract, TCreate, TUpdate, TFind>(ICrudService<TContract, TCreate, TUpdate, TFind> crudService) : ControllerBase where TContract : Entity
{
    [HttpPost(Name = "Create[controller]")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<TContract>> Create([FromBody] TCreate data, CancellationToken ct)
    {
        var created = await crudService.Create(data, ct);
        return CreatedAtAction(nameof(Create), new { id = created.Id }, created);
    }
    [HttpGet(Name = "List[controller]")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<TContract>>> List([FromQuery] TFind query, CancellationToken ct)
    {
        var list = await crudService.List(query, ct);
        return Ok(list);
    }

    [HttpGet("{id}", Name = "Get[controller]")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TContract>> Get([FromRoute] Guid id, CancellationToken ct)
    {
        var entity = await crudService.FindById(id, ct);
        if (entity == null) return NotFound();
        return Ok(entity);
    }

    [HttpPut("{id}", Name = "Update[controller]")]
    [ProducesResponseType(typeof(Admin), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<Admin>> Update([FromRoute] Guid id, [FromBody] TUpdate patch, CancellationToken ct)
    {
        var updated = await crudService.Update(id, patch, ct);
        return Ok(updated);
    }
    [HttpDelete("{id}", Name = "Delete[controller]")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete([FromRoute] Guid id, CancellationToken ct)
    {
        await crudService.Delete(id, ct);
        return NoContent();
    }
}
