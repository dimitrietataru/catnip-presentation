using CatNip.Domain.ImportExport;
using CatNip.Domain.ImportExport.Csv;
using CatNip.Domain.ImportExport.Excel;
using CatNip.Domain.Models.Interfaces;
using CatNip.Domain.Query;
using CatNip.Domain.Query.Filtering;
using CatNip.Domain.Query.Sorting.Symbols;
using CatNip.Domain.Services;
using CatNip.Presentation.Extensions;
using CatNip.Presentation.Models;
using CatNip.Presentation.Symbols;

namespace CatNip.Presentation.Controllers;

[ApiController]
public abstract class AceController<TService, TModel, TId, TModelRoot, TFiltering, TExchange>
    : AceController<TService, TModel, TId, TId, TModelRoot, TFiltering, TExchange>
    where TService : IAceService<TModel, TId, TModelRoot, TFiltering, TExchange>
    where TModel : IAceModel<TId>
    where TId : IEquatable<TId>
    where TModelRoot : ICrudModel<TId>
    where TFiltering : IFilteringRequest
    where TExchange : ICsvMappable, IExcelMappable
{
    protected AceController(TService service)
        : base(service)
    {
    }
}

[ApiController]
public abstract class AceController<TService, TModel, TId, TTraceId, TModelRoot, TFiltering, TExchange> : CrudController<TService, TModel, TId>
    where TService : IAceService<TModel, TId, TTraceId, TModelRoot, TFiltering, TExchange>
    where TModel : IAceModel<TId, TTraceId>
    where TId : IEquatable<TId>
    where TTraceId : IEquatable<TTraceId>
    where TModelRoot : ICrudModel<TId>
    where TFiltering : IFilteringRequest
    where TExchange : ICsvMappable, IExcelMappable
{
    protected AceController(TService service)
        : base(service)
    {
    }

    [HttpGet]
    public virtual async Task<IActionResult> GetAll(
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
    [Route(DefaultRoutes.Count)]
    public virtual async Task<IActionResult> Count([FromQuery] TFiltering filter, CancellationToken cancellation)
    {
        int count = await Service.CountAsync(filter, cancellation);

        return Ok(count);
    }

    [HttpPost]
    [Route(DefaultRoutes.ImportCsv)]
    public virtual async Task<IActionResult> ImportCsv([FromForm] IFormFile file, CancellationToken cancellation)
    {
        if (file is null)
        {
            return ValidationProblem(ModelState.WithError("file", "Invalid import file."));
        }

        if (file.Length == 0)
        {
            return ValidationProblem(ModelState.WithError("file", "Empty import file."));
        }

        if (string.IsNullOrEmpty(file.FileName))
        {
            return ValidationProblem(ModelState.WithError("file", "Invalid import file name."));
        }

        if (!string.Equals(FileExtensions.Csv, Path.GetExtension(file.FileName), StringComparison.OrdinalIgnoreCase))
        {
            return ValidationProblem(ModelState.WithError("file", "Invalid import file extension. Only files with .csv extension are supported"));
        }

        using var request = new ImportRequest(file.OpenReadStream(), file.FileName);
        var response = await Service.ImportCsvAsync(request, cancellation);

        if (!response.IsSuccessful)
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Failed to import file.",
                Extensions = response.Errors.GroupBy(e => e.RowNumber).ToDictionary(g => g.Key.ToString(CultureInfo.InvariantCulture), g => (object?)g.Select(e => e.ErrorMessage).ToArray()),
                Status = StatusCodes.Status400BadRequest
            };

            return BadRequest(problemDetails);
        }

        var importResponse = new ImportResponseModel
        {
            TotalRows = response.TotalRows,
            CreatedRecords = response.CreatedRecords,
            UpdatedRecords = response.UpdatedRecords
        };

        return Ok(importResponse);
    }

    [HttpPost]
    [Route(DefaultRoutes.ImportExcel)]
    public virtual async Task<IActionResult> ImportExcel([FromForm] IFormFile file, CancellationToken cancellation)
    {
        if (file is null)
        {
            return ValidationProblem(ModelState.WithError("file", "Invalid import file."));
        }

        if (file.Length == 0)
        {
            return ValidationProblem(ModelState.WithError("file", "Empty import file."));
        }

        if (string.IsNullOrEmpty(file.FileName))
        {
            return ValidationProblem(ModelState.WithError("file", "Invalid import file name."));
        }

        var allowedFileExtensions = new List<string> { FileExtensions.Xls, FileExtensions.Xlsx };
        if (!allowedFileExtensions.Contains(Path.GetExtension(file.FileName), StringComparer.OrdinalIgnoreCase))
        {
            return ValidationProblem(ModelState.WithError("file", "Invalid import file extension. Only files with .xls or .xlsx extension are supported"));
        }

        using var request = new ImportRequest(file.OpenReadStream(), file.FileName);
        var response = await Service.ImportExcelAsync(request, cancellation);

        if (!response.IsSuccessful)
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Failed to import file.",
                Extensions = response.Errors.GroupBy(e => e.RowNumber).ToDictionary(g => g.Key.ToString(CultureInfo.InvariantCulture), g => (object?)g.Select(e => e.ErrorMessage).ToArray()),
                Status = StatusCodes.Status400BadRequest
            };

            return BadRequest(problemDetails);
        }

        var importResponse = new ImportResponseModel
        {
            TotalRows = response.TotalRows,
            CreatedRecords = response.CreatedRecords,
            UpdatedRecords = response.UpdatedRecords
        };

        return Ok(importResponse);
    }

    /// <summary>
    /// Replaced by the <see cref="GetAll(int?, int?, string?, SortDirection?, TFiltering, CancellationToken)"/> action.
    /// </summary>
    [NonAction]
    public sealed override async Task<IActionResult> GetAll(CancellationToken cancellation)
    {
        return await base.GetAll(cancellation);
    }

    /// <summary>
    /// Replaced by the <see cref="Count(TFiltering, CancellationToken)"/> action.
    /// </summary>
    [NonAction]
    public sealed override async Task<IActionResult> Count(CancellationToken cancellation)
    {
        return await base.Count(cancellation);
    }
}
