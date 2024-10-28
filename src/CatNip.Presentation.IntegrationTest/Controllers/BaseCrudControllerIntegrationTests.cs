using CatNip.Domain.Models.Interfaces;

namespace CatNip.Presentation.IntegrationTest.Controllers;

public abstract partial class BaseCrudControllerIntegrationTests<TModel, TId>
    where TModel : IModel<TId>
    where TId : IEquatable<TId>
{
    protected abstract HttpClient HttpClient { get; }

    protected abstract string Endpoint { get; }
}
