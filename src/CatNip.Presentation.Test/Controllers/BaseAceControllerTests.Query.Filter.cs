using CatNip.Domain.Models.Interfaces;
using CatNip.Domain.Query;
using CatNip.Domain.Query.Filtering;
using CatNip.Domain.Query.Sorting.Symbols;
using CatNip.Domain.Services;
using CatNip.Presentation.Controllers;

namespace CatNip.Presentation.Test.Controllers;

public abstract partial class BaseAceControllerTests<TController, TService, TModel, TId, TFiltering>
    : BaseCrudControllerTests<TController, TService, TModel, TId>
    where TController : AceController<TService, TModel, TId, TFiltering>
    where TService : class, IAceService<TModel, TId, TFiltering>
    where TModel : IModel<TId>
    where TId : IEquatable<TId>
    where TFiltering : IFilteringRequest
{
    public virtual async Task GivenFilterWhenDataExistsThenReturnsData()
    {
        // Arrange
        ArrangeFilterOnSuccess();

        // Act
        var result = await Controller.Filter(
            page: It.IsAny<int?>(),
            size: It.IsAny<int?>(),
            sortBy: It.IsAny<string?>(),
            sortDirection: It.IsAny<SortDirection?>(),
            filter: It.IsAny<TFiltering>(),
            cancellation: It.IsAny<CancellationToken>());

        // Assert
        AssertFilterOnSuccess(result);
    }

    protected virtual void ArrangeFilterOnSuccess()
    {
        ServiceMock
            .Setup(_ => _.GetAsync(It.IsAny<QueryRequest<TFiltering>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(It.IsAny<QueryResponse<TModel>>())
            .Verifiable();
    }

    protected virtual void AssertFilterOnSuccess(IActionResult actionResult)
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
