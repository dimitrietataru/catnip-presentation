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
    public virtual async Task GivenImportExcelWhenSuccessThenImportsData()
    {
        // Arrange
        var formFile = ArrangeImportExcelOnSuccess();

        // Act
        var result = await Controller.ImportExcel(formFile, CancellationToken.None);

        // Assert
        AssertImportExcelOnSuccess(result);
    }

    public virtual async Task GivenImportExcelWhenFailureThenReturnsFailure()
    {
        // Arrange
        var formFile = ArrangeImportExcelOnFailure();

        // Act
        var result = await Controller.ImportExcel(formFile, CancellationToken.None);

        // Assert
        AssertImportExcelOnFailure(result);
    }

    public virtual async Task GivenImportExcelWhenFileIsInvalidThenValidationFails()
    {
        // Arrange
        var formFile = ArrangeImportExcelOnFileInvalid();

        // Act
        var result = await Controller.ImportExcel(formFile, CancellationToken.None);

        // Assert
        AssertImportExcelOnFileInvalid(result);
    }

    public virtual async Task GivenImportExcelWhenFileIsEmptyThenValidationFails()
    {
        // Arrange
        var formFile = ArrangeImportExcelOnFileEmpty();

        // Act
        var result = await Controller.ImportExcel(formFile, CancellationToken.None);

        // Assert
        AssertImportExcelOnFileEmpty(result);
    }

    public virtual async Task GivenImportExcelWhenFileNameIsInvalidThenValidationFails()
    {
        // Arrange
        var formFile = ArrangeImportExcelOnFileNameInvalid();

        // Act
        var result = await Controller.ImportExcel(formFile, CancellationToken.None);

        // Assert
        AssertImportExcelOnFileNameInvalid(result);
    }

    public virtual async Task GivenImportExcelWhenFileExtensionIsInvalidThenValidationFails()
    {
        // Arrange
        var formFile = ArrangeImportExcelOnFileExtensionInvalid();

        // Act
        var result = await Controller.ImportExcel(formFile, CancellationToken.None);

        // Assert
        AssertImportExcelOnFileExtensionInvalid(result);
    }

    protected virtual IFormFile ArrangeImportExcelOnSuccess()
    {
        var stream = new MemoryStream();
        using var streamWriter = new StreamWriter(stream, leaveOpen: true);
        streamWriter.Write("Foo Bar");
        streamWriter.Flush();
        stream.Position = 0;
        var formFile = new FormFile(stream, 0, stream.Length, "file", "test.xlsx");

        ServiceMock
            .Setup(_ => _.ImportExcelAsync(It.IsAny<ImportRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ImportResponse.Success(totalRows: 2, createdRecords: 1, updatedRecords: 1))
            .Verifiable();

        return formFile;
    }

    protected virtual void AssertImportExcelOnSuccess(IActionResult actionResult)
    {
        var result = actionResult.Should().NotBeNull().And.BeOfType<OkObjectResult>().Subject;
        result!.StatusCode.Should().Be((int)HttpStatusCode.OK);

        var importResult = result!.Value.Should().NotBeNull().And.BeOfType<ImportResponseModel>().Subject;
        importResult!.TotalRows.Should().Be(2);
        importResult!.CreatedRecords.Should().Be(1);
        importResult!.UpdatedRecords.Should().Be(1);

        ServiceMock.Verify(
            _ => _.ImportExcelAsync(It.IsAny<ImportRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        ServiceMock.VerifyNoOtherCalls();
        ServiceMock.VerifyAll();
    }

    protected virtual IFormFile ArrangeImportExcelOnFailure()
    {
        var stream = new MemoryStream();
        using var streamWriter = new StreamWriter(stream, leaveOpen: true);
        streamWriter.Write("Foo Bar");
        streamWriter.Flush();
        stream.Position = 0;
        var formFile = new FormFile(stream, 0, stream.Length, "file", "test.xlsx");

        ServiceMock
            .Setup(_ => _.ImportExcelAsync(It.IsAny<ImportRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ImportResponse.Failure(new ImportParseError("Failed..")))
            .Verifiable();

        return formFile;
    }

    protected virtual void AssertImportExcelOnFailure(IActionResult actionResult)
    {
        var result = actionResult.Should().NotBeNull().And.BeOfType<BadRequestObjectResult>().Subject;
        result!.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        result!.Value.Should().NotBeNull().And.BeOfType<ProblemDetails>();

        ServiceMock.Verify(
            _ => _.ImportExcelAsync(It.IsAny<ImportRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        ServiceMock.VerifyNoOtherCalls();
        ServiceMock.VerifyAll();
    }

    protected virtual IFormFile ArrangeImportExcelOnFileInvalid()
    {
        IFormFile formFile = null!;

        return formFile;
    }

    protected virtual void AssertImportExcelOnFileInvalid(IActionResult actionResult)
    {
        var result = actionResult.Should().NotBeNull().And.BeOfType<ObjectResult>().Subject;
        var problemDetails = result!.Value.Should().NotBeNull().And.BeOfType<ValidationProblemDetails>().Subject;
        problemDetails!.Errors.Should().ContainKey("file");

        ServiceMock.Verify(
            _ => _.ImportExcelAsync(It.IsAny<ImportRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        ServiceMock.VerifyNoOtherCalls();
        ServiceMock.VerifyAll();
    }

    protected virtual IFormFile ArrangeImportExcelOnFileEmpty()
    {
        var stream = new MemoryStream();
        var formFile = new FormFile(stream, 0, 0, "file", "test.xlsx");

        return formFile;
    }

    protected virtual void AssertImportExcelOnFileEmpty(IActionResult actionResult)
    {
        var result = actionResult.Should().NotBeNull().And.BeOfType<ObjectResult>().Subject;
        var problem = result!.Value.Should().NotBeNull().And.BeOfType<ValidationProblemDetails>().Subject;
        problem!.Errors.Should().ContainKey("file");

        ServiceMock.Verify(
            _ => _.ImportExcelAsync(It.IsAny<ImportRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        ServiceMock.VerifyNoOtherCalls();
        ServiceMock.VerifyAll();
    }

    protected virtual IFormFile ArrangeImportExcelOnFileNameInvalid()
    {
        var stream = new MemoryStream();
        using var streamWriter = new StreamWriter(stream, leaveOpen: true);
        streamWriter.Write("Foo Bar");
        streamWriter.Flush();
        stream.Position = 0;
        var formFile = new FormFile(stream, 0, stream.Length, "file", fileName: string.Empty);

        return formFile;
    }

    protected virtual void AssertImportExcelOnFileNameInvalid(IActionResult actionResult)
    {
        var result = actionResult.Should().NotBeNull().And.BeOfType<ObjectResult>().Subject;
        var problemDetails = result!.Value.Should().NotBeNull().And.BeOfType<ValidationProblemDetails>().Subject;
        problemDetails!.Errors.Should().ContainKey("file");

        ServiceMock.Verify(
            _ => _.ImportExcelAsync(It.IsAny<ImportRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        ServiceMock.VerifyNoOtherCalls();
        ServiceMock.VerifyAll();
    }

    protected virtual IFormFile ArrangeImportExcelOnFileExtensionInvalid()
    {
        var stream = new MemoryStream();
        using var streamWriter = new StreamWriter(stream, leaveOpen: true);
        streamWriter.Write("Foo Bar");
        streamWriter.Flush();
        stream.Position = 0;
        var formFile = new FormFile(stream, 0, stream.Length, "file", "test.foo");

        return formFile;
    }

    protected virtual void AssertImportExcelOnFileExtensionInvalid(IActionResult actionResult)
    {
        var result = actionResult.Should().NotBeNull().And.BeOfType<ObjectResult>().Subject;
        var problemDetails = result!.Value.Should().NotBeNull().And.BeOfType<ValidationProblemDetails>().Subject;
        problemDetails!.Errors.Should().ContainKey("file");

        ServiceMock.Verify(
            _ => _.ImportExcelAsync(It.IsAny<ImportRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        ServiceMock.VerifyNoOtherCalls();
        ServiceMock.VerifyAll();
    }
}
