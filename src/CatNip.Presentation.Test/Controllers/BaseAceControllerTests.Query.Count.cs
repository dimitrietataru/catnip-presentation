using CatNip.Domain.Models.Interfaces;
using CatNip.Domain.Query.Filtering;
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
    public override async Task GivenCountWhenDataExistsThenReturnsCount()
    {
        // Arrange
        ArrangeCountOnSuccess();

        // Act
        var result = await Controller.Count(
            It.IsAny<TFiltering>(), It.IsAny<CancellationToken>());

        // Assert
        AssertCountOnSuccess(result);
    }

    protected override void ArrangeCountOnSuccess()
    {
        ServiceMock
            .Setup(_ => _.CountAsync(It.IsAny<TFiltering>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0)
            .Verifiable();
    }

    protected override void AssertCountOnSuccess(IActionResult actionResult)
    {
        actionResult.Should().NotBeNull().And.BeOfType<OkObjectResult>();

        var result = actionResult as OkObjectResult;
        result!.StatusCode.Should().Be((int)HttpStatusCode.OK);
        result!.Value.Should().NotBeNull().And.BeOfType<int>().And.Be(0);

        ServiceMock.Verify(
            _ => _.CountAsync(It.IsAny<TFiltering>(), It.IsAny<CancellationToken>()), Times.Once);
        ServiceMock.VerifyNoOtherCalls();
        ServiceMock.VerifyAll();
    }
}
