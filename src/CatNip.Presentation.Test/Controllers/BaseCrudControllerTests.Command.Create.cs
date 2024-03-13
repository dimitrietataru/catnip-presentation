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
    public virtual async Task GivenCreateWhenInputIsValidThenCreatesData()
    {
        // Arrange
        ArrangeCreateOnSuccess();

        // Act
        var result = await Controller.Create(It.IsAny<TModel>(), It.IsAny<CancellationToken>());

        // Assert
        AssertCreateOnSuccess(result);
    }

    protected virtual void ArrangeCreateOnSuccess()
    {
        ServiceMock
            .Setup(_ => _.CreateAsync(It.IsAny<TModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Activator.CreateInstance<TModel>())
            .Verifiable();
    }

    protected virtual void AssertCreateOnSuccess(IActionResult actionResult)
    {
        actionResult.Should().NotBeNull().And.BeOfType<CreatedResult>();

        var result = actionResult as CreatedResult;
        result!.StatusCode.Should().Be((int)HttpStatusCode.Created);
        result!.Value.Should().NotBeNull().And.BeOfType<TModel>();

        ServiceMock.Verify(
            _ => _.CreateAsync(It.IsAny<TModel>(), It.IsAny<CancellationToken>()), Times.Once);
        ServiceMock.VerifyNoOtherCalls();
        ServiceMock.VerifyAll();
    }
}
