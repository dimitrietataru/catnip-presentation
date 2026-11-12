using CatNip.Domain.Models.Interfaces;
using CatNip.Domain.Services;
using CatNip.Presentation.Extensions;

namespace CatNip.Presentation.Controllers;

[ApiController]
public abstract class CrudController<TService, TModel, TId> : Controller<TService, TModel>
    where TService : ICrudService<TModel, TId>
    where TModel : ICrudModel<TId>
    where TId : IEquatable<TId>
{
    protected CrudController(TService service)
        : base(service)
    {
    }

    [HttpGet]
    [Route("{id}")]
    public virtual async Task<IActionResult> GetById([FromRoute] TId id, CancellationToken cancellation)
    {
        var result = await Service.GetByIdAsync(id, cancellation);

        return Ok(result);
    }

    [HttpPost]
    public override async Task<IActionResult> Create([FromBody] TModel model, CancellationToken cancellation)
    {
        var result = await Service.CreateAsync(model, cancellation);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut]
    [Route("{id}")]
    public virtual async Task<IActionResult> Update([FromRoute] TId id, [FromBody] TModel model, CancellationToken cancellation)
    {
        if (!id.Equals(model.Id))
        {
            return ValidationProblem(ModelState.WithError("id", "Id mismatch."));
        }

        await Service.UpdateAsync(id, model, cancellation);

        return NoContent();
    }

    [HttpDelete]
    [Route("{id}")]
    public virtual async Task<IActionResult> Delete([FromRoute] TId id, CancellationToken cancellation)
    {
        await Service.DeleteAsync(id, cancellation);

        return NoContent();
    }

    /// <summary>
    /// Replaced by the <see cref="Update(TId, TModel, CancellationToken)"/> action.
    /// </summary>
    [NonAction]
    public sealed override async Task<IActionResult> Update(TModel model, CancellationToken cancellation)
    {
        return await base.Update(model, cancellation);
    }

    /// <summary>
    /// Replaced by the <see cref="Delete(TId, CancellationToken)"/> action.
    /// </summary>
    [NonAction]
    public sealed override async Task<IActionResult> Delete(TModel model, CancellationToken cancellation)
    {
        return await base.Delete(model, cancellation);
    }
}
