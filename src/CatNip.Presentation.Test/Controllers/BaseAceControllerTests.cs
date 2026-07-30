using CatNip.Domain.ImportExport.Csv;
using CatNip.Domain.ImportExport.Excel;
using CatNip.Domain.Models.Interfaces;
using CatNip.Domain.Query.Filtering;
using CatNip.Domain.Services;
using CatNip.Presentation.Controllers;
using CatNip.Presentation.Test.Controllers.Abstractions;

namespace CatNip.Presentation.Test.Controllers;

public abstract partial class BaseAceControllerTests<TController, TService, TModel, TId, TFiltering, TExchange>
    : BaseCrudControllerTests<TController, TService, TModel, TId>, IAceControllerTests
    where TController : AceController<TService, TModel, TId, TFiltering, TExchange>
    where TService : class, IAceService<TModel, TId, TFiltering, TExchange>
    where TModel : IModel<TId>
    where TId : IEquatable<TId>
    where TFiltering : IFilteringRequest
    where TExchange : ICsvMappable, IExcelMappable
{
}
