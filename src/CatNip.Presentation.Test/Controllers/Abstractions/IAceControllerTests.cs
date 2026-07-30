namespace CatNip.Presentation.Test.Controllers.Abstractions;

public interface IAceControllerTests : ICrudControllerTests
{
    Task GivenImportCsvWhenSuccessThenImportsData();
    Task GivenImportCsvWhenFailureThenReturnsFailure();
    Task GivenImportCsvWhenFileIsInvalidThenValidationFails();
    Task GivenImportCsvWhenFileIsEmptyThenValidationFails();
    Task GivenImportCsvWhenFileNameIsInvalidThenValidationFails();
    Task GivenImportCsvWhenFileExtensionIsInvalidThenValidationFails();

    Task GivenImportExcelWhenSuccessThenImportsData();
    Task GivenImportExcelWhenFailureThenReturnsFailure();
    Task GivenImportExcelWhenFileIsInvalidThenValidationFails();
    Task GivenImportExcelWhenFileIsEmptyThenValidationFails();
    Task GivenImportExcelWhenFileNameIsInvalidThenValidationFails();
    Task GivenImportExcelWhenFileExtensionIsInvalidThenValidationFails();
}
