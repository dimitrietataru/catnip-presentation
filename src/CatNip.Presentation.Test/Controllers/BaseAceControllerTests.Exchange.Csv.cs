using CatNip.Domain.ImportExport;
using CatNip.Domain.ImportExport.Csv;
using CatNip.Domain.ImportExport.Excel;
using CatNip.Domain.Models.Interfaces;
using CatNip.Domain.Query.Filtering;
using CatNip.Domain.Services;
using CatNip.Presentation.Controllers;
using CatNip.Presentation.Models;
using CatNip.Presentation.Test.Controllers.Abstractions;

namespace CatNip.Presentation.Test.Controllers;

public abstract partial class BaseAceControllerTests<TController, TService, TModel, TId, TFiltering, TExchange>
    : BaseCrudControllerTests<TController, TService, TModel, TId>, IAceControllerTests
    where TController : AceController<TService, TModel, TId, TFiltering, TExchange>
    where TService : class, IAceService<TModel, TId, TFiltering, TExchange>
    where TModel : IModel<TId>
    where TId : IEquatable<TId>
    where TFiltering : IFilteringRequest
    where TExchange : ICsvMappable, IExcelMappable
{
    public virtual async Task GivenImportCsvWhenSuccessThenImportsData()
    {
        // Arrange
        var formFile = ArrangeImportCsvOnSuccess();

        // Act
        var result = await Controller.ImportCsv(formFile, CancellationToken.None);

        // Assert
        AssertImportCsvOnSuccess(result);
    }

    public virtual async Task GivenImportCsvWhenFailureThenReturnsFailure()
    {
        // Arrange
        var formFile = ArrangeImportCsvOnFailure();

        // Act
        var result = await Controller.ImportCsv(formFile, CancellationToken.None);

        // Assert
        AssertImportCsvOnFailure(result);
    }

    public virtual async Task GivenImportCsvWhenFileIsInvalidThenValidationFails()
    {
        // Arrange
        var formFile = ArrangeImportCsvOnFileInvalid();

        // Act
        var result = await Controller.ImportCsv(formFile, CancellationToken.None);

        // Assert
        AssertImportCsvOnFileInvalid(result);
    }

    public virtual async Task GivenImportCsvWhenFileIsEmptyThenValidationFails()
    {
        // Arrange
        var formFile = ArrangeImportCsvOnFileEmpty();

        // Act
        var result = await Controller.ImportCsv(formFile, CancellationToken.None);

        // Assert
        AssertImportCsvOnFileEmpty(result);
    }

    public virtual async Task GivenImportCsvWhenFileNameIsInvalidThenValidationFails()
    {
        // Arrange
        var formFile = ArrangeImportCsvOnFileNameInvalid();

        // Act
        var result = await Controller.ImportCsv(formFile, CancellationToken.None);

        // Assert
        AssertImportCsvOnFileNameInvalid(result);
    }

    public virtual async Task GivenImportCsvWhenFileExtensionIsInvalidThenValidationFails()
    {
        // Arrange
        var formFile = ArrangeImportCsvOnFileExtensionInvalid();

        // Act
        var result = await Controller.ImportCsv(formFile, CancellationToken.None);

        // Assert
        AssertImportCsvOnFileExtensionInvalid(result);
    }

    protected virtual IFormFile ArrangeImportCsvOnSuccess()
    {
        var stream = new MemoryStream();
        using var streamWriter = new StreamWriter(stream, leaveOpen: true);
        streamWriter.Write("Foo Bar");
        streamWriter.Flush();
        stream.Position = 0;
        var formFile = new FormFile(stream, 0, stream.Length, "file", "test.csv");

        ServiceMock
            .Setup(_ => _.ImportCsvAsync(It.IsAny<ImportRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ImportResponse.Success(totalRows: 2, createdRecords: 1, updatedRecords: 1))
            .Verifiable();

        return formFile;
    }

    protected virtual void AssertImportCsvOnSuccess(IActionResult actionResult)
    {
        var result = actionResult.Should().NotBeNull().And.BeOfType<OkObjectResult>().Subject;
        result!.StatusCode.Should().Be((int)HttpStatusCode.OK);

        var importResult = result!.Value.Should().NotBeNull().And.BeOfType<ImportResponseModel>().Subject;
        importResult!.TotalRows.Should().Be(2);
        importResult!.CreatedRecords.Should().Be(1);
        importResult!.UpdatedRecords.Should().Be(1);

        ServiceMock.Verify(
            _ => _.ImportCsvAsync(It.IsAny<ImportRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        ServiceMock.VerifyNoOtherCalls();
        ServiceMock.VerifyAll();
    }

    protected virtual IFormFile ArrangeImportCsvOnFailure()
    {
        var stream = new MemoryStream();
        using var streamWriter = new StreamWriter(stream, leaveOpen: true);
        streamWriter.Write("Foo Bar");
        streamWriter.Flush();
        stream.Position = 0;
        var formFile = new FormFile(stream, 0, stream.Length, "file", "test.csv");

        ServiceMock
            .Setup(_ => _.ImportCsvAsync(It.IsAny<ImportRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ImportResponse.Failure(new ImportParseError("Failed..")))
            .Verifiable();

        return formFile;
    }

    protected virtual void AssertImportCsvOnFailure(IActionResult actionResult)
    {
        var result = actionResult.Should().NotBeNull().And.BeOfType<BadRequestObjectResult>().Subject;
        result!.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        result!.Value.Should().NotBeNull().And.BeOfType<ProblemDetails>();

        ServiceMock.Verify(
            _ => _.ImportCsvAsync(It.IsAny<ImportRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        ServiceMock.VerifyNoOtherCalls();
        ServiceMock.VerifyAll();
    }

    protected virtual IFormFile ArrangeImportCsvOnFileInvalid()
    {
        IFormFile formFile = null!;

        return formFile;
    }

    protected virtual void AssertImportCsvOnFileInvalid(IActionResult actionResult)
    {
        var result = actionResult.Should().NotBeNull().And.BeOfType<ObjectResult>().Subject;
        var problemDetails = result!.Value.Should().NotBeNull().And.BeOfType<ValidationProblemDetails>().Subject;
        problemDetails!.Errors.Should().ContainKey("file");

        ServiceMock.Verify(
            _ => _.ImportCsvAsync(It.IsAny<ImportRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        ServiceMock.VerifyNoOtherCalls();
        ServiceMock.VerifyAll();
    }

    protected virtual IFormFile ArrangeImportCsvOnFileEmpty()
    {
        var stream = new MemoryStream();
        var formFile = new FormFile(stream, 0, 0, "file", "test.csv");

        return formFile;
    }

    protected virtual void AssertImportCsvOnFileEmpty(IActionResult actionResult)
    {
        var result = actionResult.Should().NotBeNull().And.BeOfType<ObjectResult>().Subject;
        var problem = result!.Value.Should().NotBeNull().And.BeOfType<ValidationProblemDetails>().Subject;
        problem!.Errors.Should().ContainKey("file");

        ServiceMock.Verify(
            _ => _.ImportCsvAsync(It.IsAny<ImportRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        ServiceMock.VerifyNoOtherCalls();
        ServiceMock.VerifyAll();
    }

    protected virtual IFormFile ArrangeImportCsvOnFileNameInvalid()
    {
        var stream = new MemoryStream();
        using var streamWriter = new StreamWriter(stream, leaveOpen: true);
        streamWriter.Write("Foo Bar");
        streamWriter.Flush();
        stream.Position = 0;
        var formFile = new FormFile(stream, 0, stream.Length, "file", fileName: string.Empty);

        return formFile;
    }

    protected virtual void AssertImportCsvOnFileNameInvalid(IActionResult actionResult)
    {
        var result = actionResult.Should().NotBeNull().And.BeOfType<ObjectResult>().Subject;
        var problemDetails = result!.Value.Should().NotBeNull().And.BeOfType<ValidationProblemDetails>().Subject;
        problemDetails!.Errors.Should().ContainKey("file");

        ServiceMock.Verify(
            _ => _.ImportCsvAsync(It.IsAny<ImportRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        ServiceMock.VerifyNoOtherCalls();
        ServiceMock.VerifyAll();
    }

    protected virtual IFormFile ArrangeImportCsvOnFileExtensionInvalid()
    {
        var stream = new MemoryStream();
        using var streamWriter = new StreamWriter(stream, leaveOpen: true);
        streamWriter.Write("Foo Bar");
        streamWriter.Flush();
        stream.Position = 0;
        var formFile = new FormFile(stream, 0, stream.Length, "file", "test.foo");

        return formFile;
    }

    protected virtual void AssertImportCsvOnFileExtensionInvalid(IActionResult actionResult)
    {
        var result = actionResult.Should().NotBeNull().And.BeOfType<ObjectResult>().Subject;
        var problemDetails = result!.Value.Should().NotBeNull().And.BeOfType<ValidationProblemDetails>().Subject;
        problemDetails!.Errors.Should().ContainKey("file");

        ServiceMock.Verify(
            _ => _.ImportCsvAsync(It.IsAny<ImportRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        ServiceMock.VerifyNoOtherCalls();
        ServiceMock.VerifyAll();
    }
}
