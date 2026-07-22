using CatNip.Domain.ImportExport.Csv;
using CatNip.Domain.Models.Interfaces;
using CatNip.Domain.Query.Filtering;
using CatNip.Domain.Services;
using CatNip.Presentation.Controllers;
using CatNip.Presentation.Test.Controllers;

namespace CatNip.Presentation.Test.XUnit.Controllers;

public abstract class AceControllerTests<TController, TService, TModel, TId, TFiltering, TExchange>
    : BaseAceControllerTests<TController, TService, TModel, TId, TFiltering, TExchange>
    where TController : AceController<TService, TModel, TId, TFiltering, TExchange>
    where TService : class, IAceService<TModel, TId, TFiltering, TExchange>
    where TModel : IModel<TId>
    where TId : IEquatable<TId>
    where TFiltering : IFilteringRequest
    where TExchange : ICsvMappable
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
    public override async Task GivenUpdateWhenDataExistsThenUpdatesData()
    {
        await base.GivenUpdateWhenDataExistsThenUpdatesData();
    }

    [Fact]
    public override async Task GivenUpdateWhenDataNotFoundThenThrowsException()
    {
        await base.GivenUpdateWhenDataNotFoundThenThrowsException();
    }

    [Fact]
    public override async Task GivenDeleteWhenDataExistsThenDeletesData()
    {
        await base.GivenDeleteWhenDataExistsThenDeletesData();
    }

    [Fact]
    public override async Task GivenDeleteWhenDataNotFoundThenThrowsException()
    {
        await base.GivenDeleteWhenDataNotFoundThenThrowsException();
    }

    [Fact]
    public override async Task GivenImportWhenSuccessThenImportsData()
    {
        await base.GivenImportWhenSuccessThenImportsData();
    }

    [Fact]
    public override async Task GivenImportWhenFailureThenReturnsFailure()
    {
        await base.GivenImportWhenFailureThenReturnsFailure();
    }

    [Fact]
    public override async Task GivenImportWhenFileIsInvalidThenValidationFails()
    {
        await base.GivenImportWhenFileIsInvalidThenValidationFails();
    }

    [Fact]
    public override async Task GivenImportWhenFileIsEmptyThenValidationFails()
    {
        await base.GivenImportWhenFileIsEmptyThenValidationFails();
    }

    [Fact]
    public override async Task GivenImportWhenFileNameIsInvalidThenValidationFails()
    {
        await base.GivenImportWhenFileNameIsInvalidThenValidationFails();
    }

    [Fact]
    public override async Task GivenImportWhenFileExtensionIsInvalidThenValidationFails()
    {
        await base.GivenImportWhenFileExtensionIsInvalidThenValidationFails();
    }
}
