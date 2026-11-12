using CatNip.Domain.Models.Interfaces;
using CatNip.Domain.Services;
using CatNip.Presentation.Controllers;
using CatNip.Presentation.Test.Controllers;

namespace CatNip.Presentation.Test.XUnit.Controllers;

public abstract class CrudControllerTests<TController, TService, TModel, TId>
    : BaseCrudControllerTests<TController, TService, TModel, TId>
    where TController : CrudController<TService, TModel, TId>
    where TService : class, ICrudService<TModel, TId>
    where TModel : ICrudModel<TId>
    where TId : IEquatable<TId>
{
    [Fact]
    public override async Task GivenGetAllWhenDataExistsThenReturnsData()
    {
        await base.GivenGetAllWhenDataExistsThenReturnsData();
    }

    [Fact]
    public override async Task GivenCountWhenDataExistsThenReturnsCount()
    {
        await base.GivenCountWhenDataExistsThenReturnsCount();
    }

    [Fact]
    public override async Task GivenGetByIdWhenDataExistsThenReturnsData()
    {
        await base.GivenGetByIdWhenDataExistsThenReturnsData();
    }

    [Fact]
    public override async Task GivenGetByIdWhenDataNotFoundThenThrowsException()
    {
        await base.GivenGetByIdWhenDataNotFoundThenThrowsException();
    }

    [Fact]
    public override async Task GivenCreateWhenInputIsValidThenCreatesData()
    {
        await base.GivenCreateWhenInputIsValidThenCreatesData();
    }

    [Fact]
    public override async Task GivenUpdateByIdWhenDataExistsThenUpdatesData()
    {
        await base.GivenUpdateByIdWhenDataExistsThenUpdatesData();
    }

    [Fact]
    public override async Task GivenUpdateByIdWhenDataNotFoundThenThrowsException()
    {
        await base.GivenUpdateByIdWhenDataNotFoundThenThrowsException();
    }

    [Fact]
    public override async Task GivenDeleteByIdWhenDataExistsThenDeletesData()
    {
        await base.GivenDeleteByIdWhenDataExistsThenDeletesData();
    }

    [Fact]
    public override async Task GivenDeleteByIdWhenDataNotFoundThenThrowsException()
    {
        await base.GivenDeleteByIdWhenDataNotFoundThenThrowsException();
    }
}
