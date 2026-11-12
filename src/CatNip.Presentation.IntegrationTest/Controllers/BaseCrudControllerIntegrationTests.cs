using CatNip.Domain.Models.Interfaces;

namespace CatNip.Presentation.IntegrationTest.Controllers;

public abstract class BaseCrudControllerIntegrationTests<TModel, TId>
    : BaseControllerIntegrationTests<TModel>
    where TModel : ICrudModel<TId>
    where TId : IEquatable<TId>
{
}
