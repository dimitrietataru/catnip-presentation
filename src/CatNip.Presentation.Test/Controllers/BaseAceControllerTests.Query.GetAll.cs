using CatNip.Domain.ImportExport.Csv;
using CatNip.Domain.Models.Interfaces;
using CatNip.Domain.Query;
using CatNip.Domain.Query.Filtering;
using CatNip.Domain.Query.Sorting.Symbols;
using CatNip.Domain.Services;
using CatNip.Presentation.Controllers;

namespace CatNip.Presentation.Test.Controllers;

public abstract partial class BaseAceControllerTests<TController, TService, TModel, TId, TFiltering, TExchange>
    : BaseCrudControllerTests<TController, TService, TModel, TId>
    where TController : AceController<TService, TModel, TId, TFiltering, TExchange>
    where TService : class, IAceService<TModel, TId, TFiltering, TExchange>
    where TModel : IModel<TId>
    where TId : IEquatable<TId>
    where TFiltering : IFilteringRequest
    where TExchange : ICsvMappable
{
    public override async Task GivenGetAllWhenDataExistsThenReturnsData()
    {
        // Arrange
        ArrangeGetAllOnSuccess();

        // Act
        var result = await Controller.GetAll(
            page: It.IsAny<int?>(),
            size: It.IsAny<int?>(),
            sortBy: It.IsAny<string?>(),
            sortDirection: It.IsAny<SortDirection?>(),
            filter: It.IsAny<TFiltering>(),
            cancellation: It.IsAny<CancellationToken>());

        // Assert
        AssertGetAllOnSuccess(result);
    }

    protected override void ArrangeGetAllOnSuccess()
    {
        ServiceMock
            .Setup(_ => _.GetAsync(It.IsAny<QueryRequest<TFiltering>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(It.IsAny<QueryResponse<TModel>>())
            .Verifiable();
    }

    protected override void AssertGetAllOnSuccess(IActionResult actionResult)
    {
        actionResult.Should().NotBeNull().And.BeOfType<OkObjectResult>();

        var result = actionResult as OkObjectResult;
        result!.StatusCode.Should().Be((int)HttpStatusCode.OK);

        ServiceMock.Verify(
            _ => _.GetAsync(It.IsAny<QueryRequest<TFiltering>>(), It.IsAny<CancellationToken>()),
            Times.Once);
        ServiceMock.VerifyNoOtherCalls();
        ServiceMock.VerifyAll();
    }
}
