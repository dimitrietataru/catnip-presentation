using CatNip.Domain.Models.Interfaces;
using CatNip.Domain.Services;
using CatNip.Presentation.Symbols;

namespace CatNip.Presentation.Controllers;

[ApiController]
public abstract class Controller<TService, TModel> : ControllerBase
    where TService : IService<TModel>
    where TModel : IModel
{
    protected Controller(TService service)
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
    [Route(DefaultRoutes.Count)]
    public virtual async Task<IActionResult> Count(CancellationToken cancellation)
    {
        int count = await Service.CountAsync(cancellation);

        return Ok(count);
    }

    [HttpPost]
    public virtual async Task<IActionResult> Create([FromBody] TModel model, CancellationToken cancellation)
    {
        var result = await Service.CreateAsync(model, cancellation);

        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpPut]
    public virtual async Task<IActionResult> Update([FromBody] TModel model, CancellationToken cancellation)
    {
        await Service.UpdateAsync(model, cancellation);

        return NoContent();
    }

    [HttpDelete]
    public virtual async Task<IActionResult> Delete([FromBody] TModel model, CancellationToken cancellation)
    {
        await Service.DeleteAsync(model, cancellation);

        return NoContent();
    }
}
