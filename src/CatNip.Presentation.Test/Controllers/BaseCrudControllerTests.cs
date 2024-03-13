using CatNip.Domain.Models.Interfaces;
using CatNip.Domain.Services;
using CatNip.Presentation.Controllers;

namespace CatNip.Presentation.Test.Controllers;

public abstract partial class BaseCrudControllerTests<TController, TService, TModel, TId>
    where TController : CrudController<TService, TModel, TId>
    where TService : class, ICrudService<TModel, TId>
    where TModel : IModel<TId>
    where TId : IEquatable<TId>
{
    protected abstract TController Controller { get; }
    protected abstract Mock<TService> ServiceMock { get; }
}
