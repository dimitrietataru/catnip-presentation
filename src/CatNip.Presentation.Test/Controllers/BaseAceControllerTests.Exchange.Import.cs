using CatNip.Domain.ImportExport;
using CatNip.Domain.ImportExport.Csv;
using CatNip.Domain.Models.Interfaces;
using CatNip.Domain.Query.Filtering;
using CatNip.Domain.Services;
using CatNip.Presentation.Controllers;
using CatNip.Presentation.Test.Controllers.Abstractions;

namespace CatNip.Presentation.Test.Controllers;

public abstract partial class BaseAceControllerTests<TController, TService, TModel, TId, TFiltering, TExchange>
    : BaseCrudControllerTests<TController, TService, TModel, TId>, IAceControllerTests
    where TController : AceController<TService, TModel, TId, TFiltering, TExchange>
    where TService : class, IAceService<TModel, TId, TFiltering, TExchange>
    where TModel : IModel<TId>
    where TId : IEquatable<TId>
    where TFiltering : IFilteringRequest
    where TExchange : ICsvMappable
{
    public virtual async Task GivenImportWhenSuccessThenImportsData()
    {
        // Arrange
        var formFile = ArrangeImportOnSuccess();

        // Act
        var result = await Controller.Import(formFile, CancellationToken.None);

        // Assert
        AssertImportOnSuccess(result);
    }

    public virtual async Task GivenImportWhenFailureThenReturnsFailure()
    {
        // Arrange
        var formFile = ArrangeImportOnFailure();

        // Act
        var result = await Controller.Import(formFile, CancellationToken.None);

        // Assert
        AssertImportOnFailure(result);
    }

    public virtual async Task GivenImportWhenFileIsInvalidThenValidationFails()
    {
        // Arrange
        var formFile = ArrangeImportOnFileInvalid();

        // Act
        var result = await Controller.Import(formFile, CancellationToken.None);

        // Assert
        AssertImportOnFileInvalid(result);
    }

    public virtual async Task GivenImportWhenFileIsEmptyThenValidationFails()
    {
        // Arrange
        var formFile = ArrangeImportOnFileEmpty();

        // Act
        var result = await Controller.Import(formFile, CancellationToken.None);

        // Assert
        AssertImportOnFileEmpty(result);
    }

    public virtual async Task GivenImportWhenFileNameIsInvalidThenValidationFails()
    {
        // Arrange
        var formFile = ArrangeImportOnFileNameInvalid();

        // Act
        var result = await Controller.Import(formFile, CancellationToken.None);

        // Assert
        AssertImportOnFileNameInvalid(result);
    }

    public virtual async Task GivenImportWhenFileExtensionIsInvalidThenValidationFails()
    {
        // Arrange
        var formFile = ArrangeImportOnFileExtensionInvalid();

        // Act
        var result = await Controller.Import(formFile, CancellationToken.None);

        // Assert
        AssertImportOnFileExtensionInvalid(result);
    }

    protected virtual IFormFile ArrangeImportOnSuccess()
    {
        var stream = new MemoryStream();
        using var streamWriter = new StreamWriter(stream);
        streamWriter.Write("Foo Bar");
        streamWriter.Flush();
        stream.Position = 0;
        var formFile = new FormFile(stream, 0, stream.Length, "file", "test.csv");

        ServiceMock
            .Setup(_ => _.ImportAsync(It.IsAny<ImportRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ImportResponse.Success(totalRows: 2, createdRecords: 1, updatedRecords: 1))
            .Verifiable();

        return formFile;
    }

    protected virtual void AssertImportOnSuccess(IActionResult actionResult)
    {
        actionResult.Should().NotBeNull().And.BeOfType<OkResult>();

        var result = actionResult as OkResult;
        result!.StatusCode.Should().Be((int)HttpStatusCode.OK);

        ServiceMock.Verify(
            _ => _.ImportAsync(It.IsAny<ImportRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        ServiceMock.VerifyNoOtherCalls();
        ServiceMock.VerifyAll();
    }

    protected virtual IFormFile ArrangeImportOnFailure()
    {
        var stream = new MemoryStream();
        using var streamWriter = new StreamWriter(stream);
        streamWriter.Write("Foo Bar");
        streamWriter.Flush();
        stream.Position = 0;
        var formFile = new FormFile(stream, 0, stream.Length, "file", "test.csv");

        ServiceMock
            .Setup(_ => _.ImportAsync(It.IsAny<ImportRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ImportResponse.Failure(new ImportParseError("Failed..")))
            .Verifiable();

        return formFile;
    }

    protected virtual void AssertImportOnFailure(IActionResult actionResult)
    {
        actionResult.Should().NotBeNull().And.BeOfType<BadRequestObjectResult>();

        var result = actionResult as BadRequestObjectResult;
        result!.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        result!.Value.Should().NotBeNull().And.BeOfType<ProblemDetails>();

        ServiceMock.Verify(
            _ => _.ImportAsync(It.IsAny<ImportRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        ServiceMock.VerifyNoOtherCalls();
        ServiceMock.VerifyAll();
    }

    protected virtual IFormFile ArrangeImportOnFileInvalid()
    {
        IFormFile formFile = null!;

        return formFile;
    }

    protected virtual void AssertImportOnFileInvalid(IActionResult actionResult)
    {
        actionResult.Should().NotBeNull().And.BeOfType<BadRequestObjectResult>();

        var result = actionResult as BadRequestObjectResult;
        result!.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        result!.Value.Should().NotBeNull().And.BeOfType<ValidationProblemDetails>();

        ServiceMock.Verify(
            _ => _.ImportAsync(It.IsAny<ImportRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        ServiceMock.VerifyNoOtherCalls();
        ServiceMock.VerifyAll();
    }

    protected virtual IFormFile ArrangeImportOnFileEmpty()
    {
        var stream = new MemoryStream();
        var formFile = new FormFile(stream, 0, 0, "file", "test.csv");

        return formFile;
    }

    protected virtual void AssertImportOnFileEmpty(IActionResult actionResult)
    {
        actionResult.Should().NotBeNull().And.BeOfType<BadRequestObjectResult>();

        var result = actionResult as BadRequestObjectResult;
        result!.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        result!.Value.Should().NotBeNull().And.BeOfType<ValidationProblemDetails>();

        ServiceMock.Verify(
            _ => _.ImportAsync(It.IsAny<ImportRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        ServiceMock.VerifyNoOtherCalls();
        ServiceMock.VerifyAll();
    }

    protected virtual IFormFile ArrangeImportOnFileNameInvalid()
    {
        var stream = new MemoryStream();
        using var streamWriter = new StreamWriter(stream);
        streamWriter.Write("Foo Bar");
        streamWriter.Flush();
        stream.Position = 0;
        var formFile = new FormFile(stream, 0, stream.Length, "file", fileName: string.Empty);

        return formFile;
    }

    protected virtual void AssertImportOnFileNameInvalid(IActionResult actionResult)
    {
        actionResult.Should().NotBeNull().And.BeOfType<BadRequestObjectResult>();

        var result = actionResult as BadRequestObjectResult;
        result!.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        result!.Value.Should().NotBeNull().And.BeOfType<ValidationProblemDetails>();

        ServiceMock.Verify(
            _ => _.ImportAsync(It.IsAny<ImportRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        ServiceMock.VerifyNoOtherCalls();
        ServiceMock.VerifyAll();
    }

    protected virtual IFormFile ArrangeImportOnFileExtensionInvalid()
    {
        var stream = new MemoryStream();
        using var streamWriter = new StreamWriter(stream);
        streamWriter.Write("Foo Bar");
        streamWriter.Flush();
        stream.Position = 0;
        var formFile = new FormFile(stream, 0, stream.Length, "file", "test.foo");

        return formFile;
    }

    protected virtual void AssertImportOnFileExtensionInvalid(IActionResult actionResult)
    {
        actionResult.Should().NotBeNull().And.BeOfType<BadRequestObjectResult>();

        var result = actionResult as BadRequestObjectResult;
        result!.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        result!.Value.Should().NotBeNull().And.BeOfType<ValidationProblemDetails>();

        ServiceMock.Verify(
            _ => _.ImportAsync(It.IsAny<ImportRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        ServiceMock.VerifyNoOtherCalls();
        ServiceMock.VerifyAll();
    }
}
