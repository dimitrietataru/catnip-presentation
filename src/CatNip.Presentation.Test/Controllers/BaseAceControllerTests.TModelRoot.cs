using CatNip.Domain.Models.Interfaces;
using CatNip.Domain.Query;
using CatNip.Domain.Query.Filtering;
using CatNip.Domain.Services;
using CatNip.Presentation.Controllers;

namespace CatNip.Presentation.Test.Controllers;

public abstract partial class BaseAceControllerTests<TController, TService, TModel, TModelRoot, TId, TFiltering>
    : BaseAceControllerTests<TController, TService, TModel, TId, TFiltering>
    where TController : AceController<TService, TModel, TId, TFiltering>
    where TService : class, IAceService<TModel, TId, TFiltering>
    where TModel : IModel<TId>
    where TModelRoot : IModel<TId>
    where TId : IEquatable<TId>
    where TFiltering : IFilteringRequest
{
    protected override void ArrangeGetAllOnSuccess()
    {
        ServiceMock
            .Setup(_ => _.GetAsync<TModelRoot>(
                It.IsAny<QueryRequest<TFiltering>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(It.IsAny<QueryResponse<TModelRoot>>())
            .Verifiable();
    }

    protected override void AssertGetAllOnSuccess(IActionResult actionResult)
    {
        actionResult.Should().NotBeNull().And.BeOfType<OkObjectResult>();

        var result = actionResult as OkObjectResult;
        result!.StatusCode.Should().Be((int)HttpStatusCode.OK);

        ServiceMock.Verify(
            _ => _.GetAsync<TModelRoot>(It.IsAny<QueryRequest<TFiltering>>(), It.IsAny<CancellationToken>()),
            Times.Once);
        ServiceMock.VerifyNoOtherCalls();
        ServiceMock.VerifyAll();
    }
}
