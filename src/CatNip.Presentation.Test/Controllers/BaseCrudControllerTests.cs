using CatNip.Domain.Models.Interfaces;
using CatNip.Domain.Services;
using CatNip.Presentation.Controllers;
using CatNip.Presentation.Test.Controllers.Abstractions;

namespace CatNip.Presentation.Test.Controllers;

public abstract partial class BaseCrudControllerTests<TController, TService, TModel, TId>
    : ICrudControllerTests
    where TController : CrudController<TService, TModel, TId>
    where TService : class, ICrudService<TModel, TId>
    where TModel : IModel<TId>
    where TId : IEquatable<TId>
{
    protected abstract TController Controller { get; }
    protected abstract Mock<TService> ServiceMock { get; }
}
