using CatNip.Domain.Exceptions;
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
    public virtual async Task GivenGetByIdWhenDataExistsThenReturnsData()
    {
        // Arrange
        ArrangeGetByIdOnSuccess();

        // Act
        var result = await Controller.GetById(It.IsAny<TId>(), It.IsAny<CancellationToken>());

        // Assert
        AssertGetByIdOnSuccess(result);
    }

    public virtual async Task GivenGetByIdWhenDataNotFoundThenThrowsException()
    {
        // Arrange
        ArrangeGetByIdOnNotFound();

        // Act
        var action = () => Controller.GetById(It.IsAny<TId>(), It.IsAny<CancellationToken>());

        // Assert
        await action.Should().ThrowAsync<DataNotFoundException>();
        AssertGetByIdOnNotFound();
    }

    protected virtual void ArrangeGetByIdOnSuccess()
    {
        ServiceMock
            .Setup(_ => _.GetByIdAsync(It.IsAny<TId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(It.IsAny<TModel>())
            .Verifiable();
    }

    protected virtual void AssertGetByIdOnSuccess(IActionResult actionResult)
    {
        actionResult.Should().NotBeNull().And.BeOfType<OkObjectResult>();

        var result = actionResult as OkObjectResult;
        result!.StatusCode.Should().Be((int)HttpStatusCode.OK);

        ServiceMock.Verify(
            _ => _.GetByIdAsync(It.IsAny<TId>(), It.IsAny<CancellationToken>()), Times.Once);
        ServiceMock.VerifyNoOtherCalls();
        ServiceMock.VerifyAll();
    }

    protected virtual void ArrangeGetByIdOnNotFound()
    {
        ServiceMock
            .Setup(_ => _.GetByIdAsync(It.IsAny<TId>(), It.IsAny<CancellationToken>()))
            .Throws<DataNotFoundException>()
            .Verifiable();
    }

    protected virtual void AssertGetByIdOnNotFound()
    {
        ServiceMock.Verify(
            _ => _.GetByIdAsync(It.IsAny<TId>(), It.IsAny<CancellationToken>()), Times.Once);
        ServiceMock.VerifyNoOtherCalls();
        ServiceMock.VerifyAll();
    }
}
