using CatNip.Domain.Models.Interfaces;
using CatNip.Domain.Query.Filtering;

namespace CatNip.Presentation.IntegrationTest.Controllers;

public abstract partial class BaseAceControllerIntegrationTests<TModel, TId, TFiltering>
    : BaseCrudControllerIntegrationTests<TModel, TId>
    where TModel : IModel<TId>
    where TId : IEquatable<TId>
    where TFiltering : IFilteringRequest
{
}
