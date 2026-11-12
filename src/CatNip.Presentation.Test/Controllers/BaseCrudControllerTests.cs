using CatNip.Domain.Exceptions;
using CatNip.Domain.Models.Interfaces;
using CatNip.Domain.Services;
using CatNip.Presentation.Controllers;

namespace CatNip.Presentation.Test.Controllers;

public abstract class BaseCrudControllerTests<TController, TService, TModel, TId>
    : BaseControllerTests<TController, TService, TModel>
    where TController : CrudController<TService, TModel, TId>
    where TService : class, ICrudService<TModel, TId>
    where TModel : ICrudModel<TId>
    where TId : IEquatable<TId>
{
    public virtual async Task GivenGetByIdWhenDataExistsThenReturnsData()
    {
        // Arrange
        ServiceMock
            .Setup(_ => _.GetByIdAsync(It.IsAny<TId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Activator.CreateInstance<TModel>())
            .Verifiable();

        // Act
        var actionResult = await Controller.GetById(It.IsAny<TId>(), It.IsAny<CancellationToken>());

        // Assert
        var result = actionResult.Should().NotBeNull().And.BeOfType<OkObjectResult>().Subject;
        result.StatusCode.Should().Be((int)HttpStatusCode.OK);
        result.Value.Should().NotBeNull().And.BeOfType<TModel>();
        ServiceMock.Verify(_ => _.GetByIdAsync(It.IsAny<TId>(), It.IsAny<CancellationToken>()), Times.Once);
        ServiceMock.VerifyNoOtherCalls();
        ServiceMock.VerifyAll();
    }

    public virtual async Task GivenGetByIdWhenDataNotFoundThenThrowsException()
    {
        // Arrange
        ServiceMock
            .Setup(_ => _.GetByIdAsync(It.IsAny<TId>(), It.IsAny<CancellationToken>()))
            .Throws<DataNotFoundException>()
            .Verifiable();

        // Act
        var action = () => Controller.GetById(It.IsAny<TId>(), It.IsAny<CancellationToken>());

        // Assert
        await action.Should().ThrowAsync<DataNotFoundException>();
        ServiceMock.Verify(_ => _.GetByIdAsync(It.IsAny<TId>(), It.IsAny<CancellationToken>()), Times.Once);
        ServiceMock.VerifyNoOtherCalls();
        ServiceMock.VerifyAll();
    }

    public override async Task GivenCreateWhenInputIsValidThenCreatesData()
    {
        // Arrange
        ServiceMock
            .Setup(_ => _.CreateAsync(It.IsAny<TModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Activator.CreateInstance<TModel>())
            .Verifiable();

        // Act
        var actionResult = await Controller.Create(It.IsAny<TModel>(), It.IsAny<CancellationToken>());

        // Assert
        var result = actionResult.Should().NotBeNull().And.BeOfType<CreatedAtActionResult>().Subject;
        result.StatusCode.Should().Be((int)HttpStatusCode.Created);
        result.ActionName.Should().Be(nameof(Controller.GetById));
        result.RouteValues.Should().NotBeNull().And.ContainKey("id");
        result.Value.Should().NotBeNull().And.BeOfType<TModel>();
        ServiceMock.Verify(_ => _.CreateAsync(It.IsAny<TModel>(), It.IsAny<CancellationToken>()), Times.Once);
        ServiceMock.VerifyNoOtherCalls();
        ServiceMock.VerifyAll();
    }

    public virtual async Task GivenUpdateByIdWhenDataExistsThenUpdatesData()
    {
        // Arrange
        ServiceMock
            .Setup(_ => _.UpdateAsync(It.IsAny<TId>(), It.IsAny<TModel>(), It.IsAny<CancellationToken>()))
            .Verifiable();

        // Act
        var actionResult = await Controller.Update(Activator.CreateInstance<TId>(), Activator.CreateInstance<TModel>(), It.IsAny<CancellationToken>());

        // Assert
        var result = actionResult.Should().NotBeNull().And.BeOfType<NoContentResult>().Subject;
        result.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
        ServiceMock.Verify(_ => _.UpdateAsync(It.IsAny<TId>(), It.IsAny<TModel>(), It.IsAny<CancellationToken>()), Times.Once);
        ServiceMock.VerifyNoOtherCalls();
        ServiceMock.VerifyAll();
    }

    public virtual async Task GivenUpdateByIdWhenDataNotFoundThenThrowsException()
    {
        // Arrange
        ServiceMock
            .Setup(_ => _.UpdateAsync(It.IsAny<TId>(), It.IsAny<TModel>(), It.IsAny<CancellationToken>()))
            .Throws<DataNotFoundException>()
            .Verifiable();

        // Act
        var action = () => Controller.Update(Activator.CreateInstance<TId>(), Activator.CreateInstance<TModel>(), It.IsAny<CancellationToken>());

        // Assert
        await action.Should().ThrowAsync<DataNotFoundException>();
        ServiceMock.Verify(_ => _.UpdateAsync(It.IsAny<TId>(), It.IsAny<TModel>(), It.IsAny<CancellationToken>()), Times.Once);
        ServiceMock.VerifyNoOtherCalls();
        ServiceMock.VerifyAll();
    }

    public virtual async Task GivenDeleteByIdWhenDataExistsThenDeletesData()
    {
        // Arrange
        ServiceMock
            .Setup(_ => _.DeleteAsync(It.IsAny<TId>(), It.IsAny<CancellationToken>()))
            .Verifiable();

        // Act
        var actionResult = await Controller.Delete(It.IsAny<TId>(), It.IsAny<CancellationToken>());

        // Assert
        var result = actionResult.Should().NotBeNull().And.BeOfType<NoContentResult>().Subject;
        result.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
        ServiceMock.Verify(_ => _.DeleteAsync(It.IsAny<TId>(), It.IsAny<CancellationToken>()), Times.Once);
        ServiceMock.VerifyNoOtherCalls();
        ServiceMock.VerifyAll();
    }

    public virtual async Task GivenDeleteByIdWhenDataNotFoundThenThrowsException()
    {
        // Arrange
        ServiceMock
            .Setup(_ => _.DeleteAsync(It.IsAny<TId>(), It.IsAny<CancellationToken>()))
            .Throws<DataNotFoundException>()
            .Verifiable();

        // Act
        var action = () => Controller.Delete(It.IsAny<TId>(), It.IsAny<CancellationToken>());

        // Assert
        await action.Should().ThrowAsync<DataNotFoundException>();
        ServiceMock.Verify(_ => _.DeleteAsync(It.IsAny<TId>(), It.IsAny<CancellationToken>()), Times.Once);
        ServiceMock.VerifyNoOtherCalls();
        ServiceMock.VerifyAll();
    }
}
