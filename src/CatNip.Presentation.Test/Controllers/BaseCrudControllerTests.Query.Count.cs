using CatNip.Domain.Models.Interfaces;
using CatNip.Domain.Services;
using CatNip.Presentation.Controllers;

namespace CatNip.Presentation.Test.Controllers;

public abstract partial class BaseCrudControllerTests<TController, TService, TModel, TId>
    where TController : CrudController<TService, TModel, TId>
    where TService : class, ICrudService<TModel, TId>
    where TModel : IModel<TId>
    where TId : IEquatable<TId>
{
    public virtual async Task GivenCountWhenDataExistsThenReturnsCount()
    {
        // Arrange
        ArrangeCountOnSuccess();

        // Act
        var result = await Controller.Count(It.IsAny<CancellationToken>());

        // Assert
        AssertCountOnSuccess(result);
    }

    protected virtual void ArrangeCountOnSuccess()
    {
        ServiceMock
            .Setup(_ => _.CountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0)
            .Verifiable();
    }

    protected virtual void AssertCountOnSuccess(IActionResult actionResult)
    {
        actionResult.Should().NotBeNull().And.BeOfType<OkObjectResult>();

        var result = actionResult as OkObjectResult;
        result!.StatusCode.Should().Be((int)HttpStatusCode.OK);
        result!.Value.Should().NotBeNull().And.BeOfType<int>().And.Be(0);

        ServiceMock.Verify(_ => _.CountAsync(It.IsAny<CancellationToken>()), Times.Once);
        ServiceMock.VerifyNoOtherCalls();
        ServiceMock.VerifyAll();
    }
}
