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
    public virtual async Task GivenGetAllWhenDataExistsThenReturnsData()
    {
        // Arrange
        ArrangeGetAllOnSuccess();

        // Act
        var result = await Controller.GetAll(It.IsAny<CancellationToken>());

        // Assert
        AssertGetAllOnSuccess(result);
    }

    protected virtual void ArrangeGetAllOnSuccess()
    {
        ServiceMock
            .Setup(_ => _.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([])
            .Verifiable();
    }

    protected virtual void AssertGetAllOnSuccess(IActionResult actionResult)
    {
        actionResult.Should().NotBeNull().And.BeOfType<OkObjectResult>();

        var result = actionResult as OkObjectResult;
        result!.StatusCode.Should().Be((int)HttpStatusCode.OK);
        result!.Value.Should().NotBeNull().And.BeAssignableTo<IEnumerable<TModel>>();

        ServiceMock.Verify(_ => _.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        ServiceMock.VerifyNoOtherCalls();
        ServiceMock.VerifyAll();
    }
}
