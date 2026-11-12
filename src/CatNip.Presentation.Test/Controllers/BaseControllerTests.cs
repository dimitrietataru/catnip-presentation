using CatNip.Domain.Exceptions;
using CatNip.Domain.Models.Interfaces;
using CatNip.Domain.Services;
using CatNip.Presentation.Controllers;

namespace CatNip.Presentation.Test.Controllers;

public abstract class BaseControllerTests<TController, TService, TModel>
    where TController : Controller<TService, TModel>
    where TService : class, IService<TModel>
    where TModel : IModel
{
    protected abstract TController Controller { get; }
    protected abstract Mock<TService> ServiceMock { get; }

    public virtual async Task GivenGetAllWhenDataExistsThenReturnsData()
    {
        // Arrange
        ServiceMock
            .Setup(_ => _.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([])
            .Verifiable();

        // Act
        var actionResult = await Controller.GetAll(It.IsAny<CancellationToken>());

        // Assert
        var result = actionResult.Should().NotBeNull().And.BeOfType<OkObjectResult>().Subject;
        result.StatusCode.Should().Be((int)HttpStatusCode.OK);
        result.Value.Should().NotBeNull().And.BeAssignableTo<IEnumerable<TModel>>();
        ServiceMock.Verify(_ => _.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        ServiceMock.VerifyNoOtherCalls();
        ServiceMock.VerifyAll();
    }

    public virtual async Task GivenCountWhenDataExistsThenReturnsCount()
    {
        // Arrange
        ServiceMock
            .Setup(_ => _.CountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0)
            .Verifiable();

        // Act
        var actionResult = await Controller.Count(It.IsAny<CancellationToken>());

        // Assert
        var result = actionResult.Should().NotBeNull().And.BeOfType<OkObjectResult>().Subject;
        result.StatusCode.Should().Be((int)HttpStatusCode.OK);
        result.Value.Should().NotBeNull().And.BeOfType<int>().And.Be(0);
        ServiceMock.Verify(_ => _.CountAsync(It.IsAny<CancellationToken>()), Times.Once);
        ServiceMock.VerifyNoOtherCalls();
        ServiceMock.VerifyAll();
    }

    public virtual async Task GivenCreateWhenInputIsValidThenCreatesData()
    {
        // Arrange
        ServiceMock
            .Setup(_ => _.CreateAsync(It.IsAny<TModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Activator.CreateInstance<TModel>())
            .Verifiable();

        // Act
        var actionResult = await Controller.Create(It.IsAny<TModel>(), It.IsAny<CancellationToken>());

        // Assert
        var result = actionResult.Should().NotBeNull().And.BeAssignableTo<ObjectResult>().Subject;
        result.StatusCode.Should().Be((int)HttpStatusCode.Created);
        result.Value.Should().NotBeNull().And.BeOfType<TModel>();
        ServiceMock.Verify(_ => _.CreateAsync(It.IsAny<TModel>(), It.IsAny<CancellationToken>()), Times.Once);
        ServiceMock.VerifyNoOtherCalls();
        ServiceMock.VerifyAll();
    }

    public virtual async Task GivenUpdateWhenDataExistsThenUpdatesData()
    {
        // Arrange
        ServiceMock
            .Setup(_ => _.UpdateAsync(It.IsAny<TModel>(), It.IsAny<CancellationToken>()))
            .Verifiable();

        // Act
        var actionResult = await Controller.Update(It.IsAny<TModel>(), It.IsAny<CancellationToken>());

        // Assert
        var result = actionResult.Should().NotBeNull().And.BeOfType<NoContentResult>().Subject;
        result.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
        ServiceMock.Verify(_ => _.UpdateAsync(It.IsAny<TModel>(), It.IsAny<CancellationToken>()), Times.Once);
        ServiceMock.VerifyNoOtherCalls();
        ServiceMock.VerifyAll();
    }

    public virtual async Task GivenUpdateWhenDataNotFoundThenThrowsException()
    {
        // Arrange
        ServiceMock
            .Setup(_ => _.UpdateAsync(It.IsAny<TModel>(), It.IsAny<CancellationToken>()))
            .Throws<DataNotFoundException>()
            .Verifiable();

        // Act
        var action = () => Controller.Update(It.IsAny<TModel>(), It.IsAny<CancellationToken>());

        // Assert
        await action.Should().ThrowAsync<DataNotFoundException>();
        ServiceMock.Verify(_ => _.UpdateAsync(It.IsAny<TModel>(), It.IsAny<CancellationToken>()), Times.Once);
        ServiceMock.VerifyNoOtherCalls();
        ServiceMock.VerifyAll();
    }

    public virtual async Task GivenDeleteWhenDataExistsThenDeletesData()
    {
        // Arrange
        ServiceMock
            .Setup(_ => _.DeleteAsync(It.IsAny<TModel>(), It.IsAny<CancellationToken>()))
            .Verifiable();

        // Act
        var actionResult = await Controller.Delete(It.IsAny<TModel>(), It.IsAny<CancellationToken>());

        // Assert
        var result = actionResult.Should().NotBeNull().And.BeOfType<NoContentResult>().Subject;
        result.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
        ServiceMock.Verify(_ => _.DeleteAsync(It.IsAny<TModel>(), It.IsAny<CancellationToken>()), Times.Once);
        ServiceMock.VerifyNoOtherCalls();
        ServiceMock.VerifyAll();
    }

    public virtual async Task GivenDeleteWhenDataNotFoundThenThrowsException()
    {
        // Arrange
        ServiceMock
            .Setup(_ => _.DeleteAsync(It.IsAny<TModel>(), It.IsAny<CancellationToken>()))
            .Throws<DataNotFoundException>()
            .Verifiable();

        // Act
        var action = () => Controller.Delete(It.IsAny<TModel>(), It.IsAny<CancellationToken>());

        // Assert
        await action.Should().ThrowAsync<DataNotFoundException>();
        ServiceMock.Verify(_ => _.DeleteAsync(It.IsAny<TModel>(), It.IsAny<CancellationToken>()), Times.Once);
        ServiceMock.VerifyNoOtherCalls();
        ServiceMock.VerifyAll();
    }
}
