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
    public virtual async Task GivenDeleteWhenDataExistsThenDeletesData()
    {
        // Arrange
        ArrangeDeleteOnSuccess();

        // Act
        var result = await Controller.Delete(It.IsAny<TId>(), It.IsAny<CancellationToken>());

        // Assert
        AssertDeleteOnSuccess(result);
    }

    public virtual async Task GivenDeleteWhenDataNotFoundThenThrowsException()
    {
        // Arrange
        ArrangeDeleteOnNotFound();

        // Act
        var action = () => Controller.Delete(It.IsAny<TId>(), It.IsAny<CancellationToken>());

        // Assert
        await action.Should().ThrowAsync<DataNotFoundException>();
        AssertDeleteOnNotFound();
    }

    protected virtual void ArrangeDeleteOnSuccess()
    {
        ServiceMock
            .Setup(_ => _.DeleteAsync(It.IsAny<TId>(), It.IsAny<CancellationToken>()))
            .Verifiable();
    }

    protected virtual void AssertDeleteOnSuccess(IActionResult actionResult)
    {
        actionResult.Should().NotBeNull().And.BeOfType<NoContentResult>();

        var result = actionResult as NoContentResult;
        result!.StatusCode.Should().Be((int)HttpStatusCode.NoContent);

        ServiceMock.Verify(
            _ => _.DeleteAsync(It.IsAny<TId>(), It.IsAny<CancellationToken>()), Times.Once);
        ServiceMock.VerifyNoOtherCalls();
        ServiceMock.VerifyAll();
    }

    protected virtual void ArrangeDeleteOnNotFound()
    {
        ServiceMock
            .Setup(_ => _.DeleteAsync(It.IsAny<TId>(), It.IsAny<CancellationToken>()))
            .Throws<DataNotFoundException>()
            .Verifiable();
    }

    protected virtual void AssertDeleteOnNotFound()
    {
        ServiceMock.Verify(
            _ => _.DeleteAsync(It.IsAny<TId>(), It.IsAny<CancellationToken>()), Times.Once);
        ServiceMock.VerifyNoOtherCalls();
        ServiceMock.VerifyAll();
    }
}
