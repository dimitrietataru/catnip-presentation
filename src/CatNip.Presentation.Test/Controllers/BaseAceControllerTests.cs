using CatNip.Domain.ImportExport;
using CatNip.Domain.ImportExport.Csv;
using CatNip.Domain.ImportExport.Excel;
using CatNip.Domain.Models.Interfaces;
using CatNip.Domain.Query;
using CatNip.Domain.Query.Filtering;
using CatNip.Domain.Query.Sorting.Symbols;
using CatNip.Domain.Services;
using CatNip.Presentation.Controllers;
using CatNip.Presentation.Models;

namespace CatNip.Presentation.Test.Controllers;

public abstract class BaseAceControllerTests<TController, TService, TModel, TId, TModelRoot, TFiltering, TExchange>
    : BaseAceControllerTests<TController, TService, TModel, TId, TId, TModelRoot, TFiltering, TExchange>
    where TController : AceController<TService, TModel, TId, TModelRoot, TFiltering, TExchange>
    where TService : class, IAceService<TModel, TId, TModelRoot, TFiltering, TExchange>
    where TModel : IAceModel<TId>
    where TId : IEquatable<TId>
    where TModelRoot : ICrudModel<TId>
    where TFiltering : IFilteringRequest
    where TExchange : ICsvMappable, IExcelMappable
{
}

public abstract class BaseAceControllerTests<TController, TService, TModel, TId, TTraceId, TModelRoot, TFiltering, TExchange>
    : BaseCrudControllerTests<TController, TService, TModel, TId>
    where TController : AceController<TService, TModel, TId, TTraceId, TModelRoot, TFiltering, TExchange>
    where TService : class, IAceService<TModel, TId, TTraceId, TModelRoot, TFiltering, TExchange>
    where TModel : IAceModel<TId, TTraceId>
    where TId : IEquatable<TId>
    where TTraceId : IEquatable<TTraceId>
    where TModelRoot : ICrudModel<TId>
    where TFiltering : IFilteringRequest
    where TExchange : ICsvMappable, IExcelMappable
{
    public virtual async Task GivenGetByFilterWhenDataExistsThenReturnsData()
    {
        // Arrange
        ServiceMock
            .Setup(_ => _.GetAsync(It.IsAny<QueryRequest<TFiltering>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new QueryResponse<TModelRoot>())
            .Verifiable();

        // Act
        var actionResult = await Controller.GetAll(
            page: It.IsAny<int?>(),
            size: It.IsAny<int?>(),
            sortBy: It.IsAny<string?>(),
            sortDirection: It.IsAny<SortDirection?>(),
            filter: It.IsAny<TFiltering>(),
            cancellation: It.IsAny<CancellationToken>());

        // Assert
        var result = actionResult.Should().NotBeNull().And.BeOfType<OkObjectResult>().Subject;
        result.StatusCode.Should().Be((int)HttpStatusCode.OK);
        result.Value.Should().NotBeNull().And.BeOfType<QueryResponse<TModelRoot>>();
        ServiceMock.Verify(_ => _.GetAsync(It.IsAny<QueryRequest<TFiltering>>(), It.IsAny<CancellationToken>()), Times.Once);
        ServiceMock.VerifyNoOtherCalls();
        ServiceMock.VerifyAll();
    }

    public virtual async Task GivenCountByFilterWhenDataExistsThenReturnsCount()
    {
        // Arrange
        ServiceMock
            .Setup(_ => _.CountAsync(It.IsAny<TFiltering>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0)
            .Verifiable();

        // Act
        var actionResult = await Controller.Count(It.IsAny<TFiltering>(), It.IsAny<CancellationToken>());

        // Assert
        var result = actionResult.Should().NotBeNull().And.BeOfType<OkObjectResult>().Subject;
        result.StatusCode.Should().Be((int)HttpStatusCode.OK);
        result.Value.Should().NotBeNull().And.BeOfType<int>().And.Be(0);
        ServiceMock.Verify(_ => _.CountAsync(It.IsAny<TFiltering>(), It.IsAny<CancellationToken>()), Times.Once);
        ServiceMock.VerifyNoOtherCalls();
        ServiceMock.VerifyAll();
    }

    public virtual async Task GivenImportCsvWhenSuccessThenImportsData()
    {
        // Arrange
        using var stream = new MemoryStream("Foo Bar"u8.ToArray());
        var formFile = new FormFile(stream, 0, stream.Length, "file", "test.csv");
        ServiceMock
            .Setup(_ => _.ImportCsvAsync(It.IsAny<ImportRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ImportResponse.Success(totalRows: 2, createdRecords: 1, updatedRecords: 1))
            .Verifiable();

        // Act
        var actionResult = await Controller.ImportCsv(formFile, CancellationToken.None);

        // Assert
        var result = actionResult.Should().NotBeNull().And.BeOfType<OkObjectResult>().Subject;
        result.StatusCode.Should().Be((int)HttpStatusCode.OK);
        var importResponse = result.Value.Should().NotBeNull().And.BeOfType<ImportResponseModel>().Subject;
        importResponse.TotalRows.Should().Be(2);
        importResponse.CreatedRecords.Should().Be(1);
        importResponse.UpdatedRecords.Should().Be(1);
        ServiceMock.Verify(_ => _.ImportCsvAsync(It.IsAny<ImportRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        ServiceMock.VerifyNoOtherCalls();
        ServiceMock.VerifyAll();
    }

    public virtual async Task GivenImportCsvWhenFailureThenReturnsFailure()
    {
        // Arrange
        const string Error = "Parse Error";
        using var stream = new MemoryStream("Foo Bar"u8.ToArray());
        var formFile = new FormFile(stream, 0, stream.Length, "file", "test.csv");
        ServiceMock
            .Setup(_ => _.ImportCsvAsync(It.IsAny<ImportRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ImportResponse.Failure(new ImportParseError(Error)))
            .Verifiable();

        // Act
        var actionResult = await Controller.ImportCsv(formFile, CancellationToken.None);

        // Assert
        var result = actionResult.Should().NotBeNull().And.BeOfType<BadRequestObjectResult>().Subject;
        result.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        var problemDetails = result.Value.Should().NotBeNull().And.BeOfType<ProblemDetails>().Subject;
        problemDetails.Extensions.Should().ContainKey("-1").WhoseValue.Should().BeEquivalentTo(Error);
        ServiceMock.Verify(_ => _.ImportCsvAsync(It.IsAny<ImportRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        ServiceMock.VerifyNoOtherCalls();
        ServiceMock.VerifyAll();
    }

    public virtual async Task GivenImportCsvWhenRowHasMultipleErrorsThenGroupsErrors()
    {
        // Arrange
        string[] errors = ["Validation Error", "Data Integrity Error"];
        using var stream = new MemoryStream("Foo Bar"u8.ToArray());
        var formFile = new FormFile(stream, 0, stream.Length, "file", "test.csv");
        ServiceMock
            .Setup(_ => _.ImportCsvAsync(It.IsAny<ImportRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ImportResponse.Failure(
                new ImportValidationError(rowNumber: 10, errors[0]),
                new ImportDataIntegrityError(rowNumber: 10, errors[1])))
            .Verifiable();

        // Act
        var actionResult = await Controller.ImportCsv(formFile, CancellationToken.None);

        // Assert
        var result = actionResult.Should().NotBeNull().And.BeOfType<BadRequestObjectResult>().Subject;
        result.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        var problemDetails = result.Value.Should().NotBeNull().And.BeOfType<ProblemDetails>().Subject;
        problemDetails.Extensions.Should().HaveCount(1);
        problemDetails.Extensions.Should().ContainKey("10").WhoseValue.Should().BeEquivalentTo(errors);
        ServiceMock.Verify(_ => _.ImportCsvAsync(It.IsAny<ImportRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        ServiceMock.VerifyNoOtherCalls();
        ServiceMock.VerifyAll();
    }

    public virtual async Task GivenImportCsvWhenFileIsInvalidThenValidationFails()
    {
        // Arrange
        IFormFile formFile = null!;

        // Act
        var actionResult = await Controller.ImportCsv(formFile, CancellationToken.None);

        // Assert
        var result = actionResult.Should().NotBeNull().And.BeOfType<ObjectResult>().Subject;
        var problemDetails = result.Value.Should().NotBeNull().And.BeOfType<ValidationProblemDetails>().Subject;
        problemDetails.Errors.Should().ContainKey("file");
        ServiceMock.Verify(_ => _.ImportCsvAsync(It.IsAny<ImportRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        ServiceMock.VerifyNoOtherCalls();
        ServiceMock.VerifyAll();
    }

    public virtual async Task GivenImportCsvWhenFileIsEmptyThenValidationFails()
    {
        // Arrange
        using var stream = new MemoryStream();
        var formFile = new FormFile(stream, 0, 0, "file", "test.csv");

        // Act
        var actionResult = await Controller.ImportCsv(formFile, CancellationToken.None);

        // Assert
        var result = actionResult.Should().NotBeNull().And.BeOfType<ObjectResult>().Subject;
        var problemDetails = result.Value.Should().NotBeNull().And.BeOfType<ValidationProblemDetails>().Subject;
        problemDetails.Errors.Should().ContainKey("file");
        ServiceMock.Verify(_ => _.ImportCsvAsync(It.IsAny<ImportRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        ServiceMock.VerifyNoOtherCalls();
        ServiceMock.VerifyAll();
    }

    public virtual async Task GivenImportCsvWhenFileNameIsInvalidThenValidationFails()
    {
        // Arrange
        using var stream = new MemoryStream("Foo Bar"u8.ToArray());
        var formFile = new FormFile(stream, 0, stream.Length, "file", fileName: string.Empty);

        // Act
        var actionResult = await Controller.ImportCsv(formFile, CancellationToken.None);

        // Assert
        var result = actionResult.Should().NotBeNull().And.BeOfType<ObjectResult>().Subject;
        var problemDetails = result.Value.Should().NotBeNull().And.BeOfType<ValidationProblemDetails>().Subject;
        problemDetails.Errors.Should().ContainKey("file");
        ServiceMock.Verify(_ => _.ImportCsvAsync(It.IsAny<ImportRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        ServiceMock.VerifyNoOtherCalls();
        ServiceMock.VerifyAll();
    }

    public virtual async Task GivenImportCsvWhenFileExtensionIsInvalidThenValidationFails()
    {
        // Arrange
        using var stream = new MemoryStream("Foo Bar"u8.ToArray());
        var formFile = new FormFile(stream, 0, stream.Length, "file", "test.foo");

        // Act
        var actionResult = await Controller.ImportCsv(formFile, CancellationToken.None);

        // Assert
        var result = actionResult.Should().NotBeNull().And.BeOfType<ObjectResult>().Subject;
        var problemDetails = result.Value.Should().NotBeNull().And.BeOfType<ValidationProblemDetails>().Subject;
        problemDetails.Errors.Should().ContainKey("file");
        ServiceMock.Verify(_ => _.ImportCsvAsync(It.IsAny<ImportRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        ServiceMock.VerifyNoOtherCalls();
        ServiceMock.VerifyAll();
    }

    public virtual async Task GivenImportExcelWhenSuccessThenImportsData()
    {
        // Arrange
        using var stream = new MemoryStream("Foo Bar"u8.ToArray());
        var formFile = new FormFile(stream, 0, stream.Length, "file", "test.xlsx");
        ServiceMock
            .Setup(_ => _.ImportExcelAsync(It.IsAny<ImportRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ImportResponse.Success(totalRows: 2, createdRecords: 1, updatedRecords: 1))
            .Verifiable();

        // Act
        var actionResult = await Controller.ImportExcel(formFile, CancellationToken.None);

        // Assert
        var result = actionResult.Should().NotBeNull().And.BeOfType<OkObjectResult>().Subject;
        result.StatusCode.Should().Be((int)HttpStatusCode.OK);
        var importResponse = result.Value.Should().NotBeNull().And.BeOfType<ImportResponseModel>().Subject;
        importResponse.TotalRows.Should().Be(2);
        importResponse.CreatedRecords.Should().Be(1);
        importResponse.UpdatedRecords.Should().Be(1);
        ServiceMock.Verify(_ => _.ImportExcelAsync(It.IsAny<ImportRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        ServiceMock.VerifyNoOtherCalls();
        ServiceMock.VerifyAll();
    }

    public virtual async Task GivenImportExcelWhenFailureThenReturnsFailure()
    {
        // Arrange
        const string Error = "Parse Error";
        using var stream = new MemoryStream("Foo Bar"u8.ToArray());
        var formFile = new FormFile(stream, 0, stream.Length, "file", "test.xlsx");
        ServiceMock
            .Setup(_ => _.ImportExcelAsync(It.IsAny<ImportRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ImportResponse.Failure(new ImportParseError(Error)))
            .Verifiable();

        // Act
        var actionResult = await Controller.ImportExcel(formFile, CancellationToken.None);

        // Assert
        var result = actionResult.Should().NotBeNull().And.BeOfType<BadRequestObjectResult>().Subject;
        result.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        var problemDetails = result.Value.Should().NotBeNull().And.BeOfType<ProblemDetails>().Subject;
        problemDetails.Extensions.Should().ContainKey("-1").WhoseValue.Should().BeEquivalentTo(Error);
        ServiceMock.Verify(_ => _.ImportExcelAsync(It.IsAny<ImportRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        ServiceMock.VerifyNoOtherCalls();
        ServiceMock.VerifyAll();
    }

    public virtual async Task GivenImportExcelWhenRowHasMultipleErrorsThenGroupsErrors()
    {
        // Arrange
        string[] errors = ["Validation Error", "Data Integrity Error"];
        using var stream = new MemoryStream("Foo Bar"u8.ToArray());
        var formFile = new FormFile(stream, 0, stream.Length, "file", "test.xlsx");
        ServiceMock
            .Setup(_ => _.ImportExcelAsync(It.IsAny<ImportRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ImportResponse.Failure(
                new ImportValidationError(rowNumber: 10, errors[0]),
                new ImportDataIntegrityError(rowNumber: 10, errors[1])))
            .Verifiable();

        // Act
        var actionResult = await Controller.ImportExcel(formFile, CancellationToken.None);

        // Assert
        var result = actionResult.Should().NotBeNull().And.BeOfType<BadRequestObjectResult>().Subject;
        result.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        var problemDetails = result.Value.Should().NotBeNull().And.BeOfType<ProblemDetails>().Subject;
        problemDetails.Extensions.Should().HaveCount(1);
        problemDetails.Extensions.Should().ContainKey("10").WhoseValue.Should().BeEquivalentTo(errors);
        ServiceMock.Verify(_ => _.ImportExcelAsync(It.IsAny<ImportRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        ServiceMock.VerifyNoOtherCalls();
        ServiceMock.VerifyAll();
    }

    public virtual async Task GivenImportExcelWhenFileIsInvalidThenValidationFails()
    {
        // Arrange
        IFormFile formFile = null!;

        // Act
        var actionResult = await Controller.ImportExcel(formFile, CancellationToken.None);

        // Assert
        var result = actionResult.Should().NotBeNull().And.BeOfType<ObjectResult>().Subject;
        var problemDetails = result.Value.Should().NotBeNull().And.BeOfType<ValidationProblemDetails>().Subject;
        problemDetails.Errors.Should().ContainKey("file");
        ServiceMock.Verify(_ => _.ImportExcelAsync(It.IsAny<ImportRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        ServiceMock.VerifyNoOtherCalls();
        ServiceMock.VerifyAll();
    }

    public virtual async Task GivenImportExcelWhenFileIsEmptyThenValidationFails()
    {
        // Arrange
        using var stream = new MemoryStream();
        var formFile = new FormFile(stream, 0, 0, "file", "test.xlsx");

        // Act
        var actionResult = await Controller.ImportExcel(formFile, CancellationToken.None);

        // Assert
        var result = actionResult.Should().NotBeNull().And.BeOfType<ObjectResult>().Subject;
        var problemDetails = result.Value.Should().NotBeNull().And.BeOfType<ValidationProblemDetails>().Subject;
        problemDetails.Errors.Should().ContainKey("file");
        ServiceMock.Verify(_ => _.ImportExcelAsync(It.IsAny<ImportRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        ServiceMock.VerifyNoOtherCalls();
        ServiceMock.VerifyAll();
    }

    public virtual async Task GivenImportExcelWhenFileNameIsInvalidThenValidationFails()
    {
        // Arrange
        using var stream = new MemoryStream("Foo Bar"u8.ToArray());
        var formFile = new FormFile(stream, 0, stream.Length, "file", fileName: string.Empty);

        // Act
        var actionResult = await Controller.ImportExcel(formFile, CancellationToken.None);

        // Assert
        var result = actionResult.Should().NotBeNull().And.BeOfType<ObjectResult>().Subject;
        var problemDetails = result.Value.Should().NotBeNull().And.BeOfType<ValidationProblemDetails>().Subject;
        problemDetails.Errors.Should().ContainKey("file");
        ServiceMock.Verify(_ => _.ImportExcelAsync(It.IsAny<ImportRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        ServiceMock.VerifyNoOtherCalls();
        ServiceMock.VerifyAll();
    }

    public virtual async Task GivenImportExcelWhenFileExtensionIsInvalidThenValidationFails()
    {
        // Arrange
        using var stream = new MemoryStream("Foo Bar"u8.ToArray());
        var formFile = new FormFile(stream, 0, stream.Length, "file", "test.foo");

        // Act
        var actionResult = await Controller.ImportExcel(formFile, CancellationToken.None);

        // Assert
        var result = actionResult.Should().NotBeNull().And.BeOfType<ObjectResult>().Subject;
        var problemDetails = result.Value.Should().NotBeNull().And.BeOfType<ValidationProblemDetails>().Subject;
        problemDetails.Errors.Should().ContainKey("file");
        ServiceMock.Verify(_ => _.ImportExcelAsync(It.IsAny<ImportRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        ServiceMock.VerifyNoOtherCalls();
        ServiceMock.VerifyAll();
    }
}
