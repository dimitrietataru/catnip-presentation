namespace CatNip.Presentation.Test.Controllers.Abstractions;

public interface IAceControllerTests : ICrudControllerTests
{
    Task GivenImportWhenSuccessThenImportsData();
    Task GivenImportWhenFailureThenReturnsFailure();
    Task GivenImportWhenFileIsInvalidThenValidationFails();
    Task GivenImportWhenFileIsEmptyThenValidationFails();
    Task GivenImportWhenFileNameIsInvalidThenValidationFails();
    Task GivenImportWhenFileExtensionIsInvalidThenValidationFails();
}
