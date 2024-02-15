using CatNip.Domain.Models.Interfaces;
using CatNip.Domain.Services;

namespace CatNip.Presentation.Controllers;

[ApiController]
public abstract class CrudController<TService, TModel, TId> : ControllerBase
    where TService : ICrudService<TModel, TId>
    where TModel : IModel<TId>
    where TId : IEquatable<TId>
{
    protected CrudController(TService service)
    {
        Service = service;
    }

    protected virtual TService Service { get; init; }

    [HttpGet]
    public virtual async Task<IActionResult> GetAll(CancellationToken cancellation)
    {
        var result = await Service.GetAllAsync(cancellation);

        return Ok(result);
    }

    [HttpGet]
    [Route("count")]
    public virtual async Task<IActionResult> Count(CancellationToken cancellation)
    {
        int count = await Service.CountAsync(cancellation);

        return Ok(count);
    }

    [HttpGet]
    [Route("{id}")]
    public virtual async Task<IActionResult> GetById([FromRoute] TId id, CancellationToken cancellation)
    {
        var result = await Service.GetByIdAsync(id, cancellation);

        return Ok(result);
    }

    [HttpPost]
    public virtual async Task<IActionResult> Create(
        [FromBody] TModel model,
        CancellationToken cancellation)
    {
        var result = await Service.CreateAsync(model, cancellation);

        return Created(new Uri($"/{result.Id}", UriKind.Relative), result);
    }

    [HttpPut]
    [Route("{id}")]
    public virtual async Task<IActionResult> Update(
        [FromRoute] TId id,
        [FromBody] TModel model, CancellationToken cancellation)
    {
        if (!id.Equals(model.Id))
        {
            return BadRequest(
                new ProblemDetails
                {
                    Detail = "Id mismatch"
                });
        }

        await Service.UpdateAsync(id, model, cancellation);

        return NoContent();
    }

    [HttpDelete]
    [Route("{id}")]
    public virtual async Task<IActionResult> Delete(
        [FromRoute] TId id,
        CancellationToken cancellation)
    {
        await Service.DeleteAsync(id, cancellation);

        return NoContent();
    }
}
