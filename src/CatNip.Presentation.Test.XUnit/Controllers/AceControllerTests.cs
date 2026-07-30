using CatNip.Domain.ImportExport.Csv;
using CatNip.Domain.ImportExport.Excel;
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
    where TExchange : ICsvMappable, IExcelMappable
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
    public override async Task GivenImportCsvWhenSuccessThenImportsData()
    {
        await base.GivenImportCsvWhenSuccessThenImportsData();
    }

    [Fact]
    public override async Task GivenImportCsvWhenFailureThenReturnsFailure()
    {
        await base.GivenImportCsvWhenFailureThenReturnsFailure();
    }

    [Fact]
    public override async Task GivenImportCsvWhenFileIsInvalidThenValidationFails()
    {
        await base.GivenImportCsvWhenFileIsInvalidThenValidationFails();
    }

    [Fact]
    public override async Task GivenImportCsvWhenFileIsEmptyThenValidationFails()
    {
        await base.GivenImportCsvWhenFileIsEmptyThenValidationFails();
    }

    [Fact]
    public override async Task GivenImportCsvWhenFileNameIsInvalidThenValidationFails()
    {
        await base.GivenImportCsvWhenFileNameIsInvalidThenValidationFails();
    }

    [Fact]
    public override async Task GivenImportCsvWhenFileExtensionIsInvalidThenValidationFails()
    {
        await base.GivenImportCsvWhenFileExtensionIsInvalidThenValidationFails();
    }

    [Fact]
    public override async Task GivenImportExcelWhenSuccessThenImportsData()
    {
        await base.GivenImportExcelWhenSuccessThenImportsData();
    }

    [Fact]
    public override async Task GivenImportExcelWhenFailureThenReturnsFailure()
    {
        await base.GivenImportExcelWhenFailureThenReturnsFailure();
    }

    [Fact]
    public override async Task GivenImportExcelWhenFileIsInvalidThenValidationFails()
    {
        await base.GivenImportExcelWhenFileIsInvalidThenValidationFails();
    }

    [Fact]
    public override async Task GivenImportExcelWhenFileIsEmptyThenValidationFails()
    {
        await base.GivenImportExcelWhenFileIsEmptyThenValidationFails();
    }

    [Fact]
    public override async Task GivenImportExcelWhenFileNameIsInvalidThenValidationFails()
    {
        await base.GivenImportExcelWhenFileNameIsInvalidThenValidationFails();
    }

    [Fact]
    public override async Task GivenImportExcelWhenFileExtensionIsInvalidThenValidationFails()
    {
        await base.GivenImportExcelWhenFileExtensionIsInvalidThenValidationFails();
    }
}
