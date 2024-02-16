using CatNip.Domain.Models.Interfaces;
using CatNip.Domain.Query;
using CatNip.Domain.Query.Filtering;
using CatNip.Domain.Query.Sorting.Symbols;
using CatNip.Domain.Services;

namespace CatNip.Presentation.Controllers;

[ApiController]
public abstract class AceController<TService, TModel, TId, TFiltering> : CrudController<TService, TModel, TId>
    where TService : IAceService<TModel, TId, TFiltering>
    where TModel : IModel<TId>
    where TId : IEquatable<TId>
    where TFiltering : IFilteringRequest
{
    protected AceController(TService service)
        : base(service)
    {
    }

    [HttpGet]
    [Route("filter")]
    public virtual async Task<IActionResult> Filter(
        [FromQuery] int? page,
        [FromQuery] int? size,
        [FromQuery] string? sortBy,
        [FromQuery] SortDirection? sortDirection,
        [FromQuery] TFiltering filter,
        CancellationToken cancellation)
    {
        var request = new QueryRequest<TFiltering>(filter, page, size, sortBy, sortDirection);
        var result = await Service.GetAsync(request, cancellation);

        return Ok(result);
    }

    [HttpGet]
    [Route("filter/count")]
    public virtual async Task<IActionResult> FilterCount(
        [FromQuery] TFiltering filter,
        CancellationToken cancellation)
    {
        int count = await Service.CountAsync(filter, cancellation);

        return Ok(count);
    }
}
