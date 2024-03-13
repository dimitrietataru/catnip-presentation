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
    public virtual async Task GivenUpdateWhenDataExistsThenUpdatesData()
    {
        // Arrange
        ArrangeUpdateOnSuccess();

        // Act
        var result = await Controller.Update(
            Activator.CreateInstance<TId>(),
            Activator.CreateInstance<TModel>(),
            It.IsAny<CancellationToken>());

        // Assert
        AssertUpdateOnSuccess(result);
    }

    public virtual async Task GivenUpdateWhenDataNotFoundThenThrowsException()
    {
        // Arrange
        ArrangeUpdateOnNotFound();

        // Act
        var action = () => Controller.Update(
            Activator.CreateInstance<TId>(),
            Activator.CreateInstance<TModel>(),
            It.IsAny<CancellationToken>());

        // Assert
        await action.Should().ThrowAsync<DataNotFoundException>();
        AssertUpdateOnNotFound();
    }

    protected virtual void ArrangeUpdateOnSuccess()
    {
        ServiceMock
            .Setup(_ => _.UpdateAsync(
                It.IsAny<TId>(), It.IsAny<TModel>(), It.IsAny<CancellationToken>()))
            .Verifiable();
    }

    protected virtual void AssertUpdateOnSuccess(IActionResult actionResult)
    {
        actionResult.Should().NotBeNull().And.BeOfType<NoContentResult>();

        var result = actionResult as NoContentResult;
        result!.StatusCode.Should().Be((int)HttpStatusCode.NoContent);

        ServiceMock.Verify(
            _ => _.UpdateAsync(It.IsAny<TId>(), It.IsAny<TModel>(), It.IsAny<CancellationToken>()),
            Times.Once);
        ServiceMock.VerifyNoOtherCalls();
        ServiceMock.VerifyAll();
    }

    protected virtual void ArrangeUpdateOnNotFound()
    {
        ServiceMock
            .Setup(_ => _.UpdateAsync(
                It.IsAny<TId>(), It.IsAny<TModel>(), It.IsAny<CancellationToken>()))
            .Throws<DataNotFoundException>()
            .Verifiable();
    }

    protected virtual void AssertUpdateOnNotFound()
    {
        ServiceMock.Verify(
            _ => _.UpdateAsync(It.IsAny<TId>(), It.IsAny<TModel>(), It.IsAny<CancellationToken>()),
            Times.Once);
        ServiceMock.VerifyNoOtherCalls();
        ServiceMock.VerifyAll();
    }
}
