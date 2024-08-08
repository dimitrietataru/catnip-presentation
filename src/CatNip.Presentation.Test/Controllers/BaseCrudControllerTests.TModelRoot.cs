using CatNip.Domain.Models.Interfaces;
using CatNip.Domain.Services;
using CatNip.Presentation.Controllers;

namespace CatNip.Presentation.Test.Controllers;

public abstract partial class BaseCrudControllerTests<TController, TService, TModel, TModelRoot, TId>
    : BaseCrudControllerTests<TController, TService, TModel, TId>
    where TController : CrudController<TService, TModel, TModelRoot, TId>
    where TService : class, ICrudService<TModel, TId>
    where TModel : IModel<TId>
    where TModelRoot : IModel<TId>
    where TId : IEquatable<TId>
{
    protected override void ArrangeGetAllOnSuccess()
    {
        ServiceMock
            .Setup(_ => _.GetAllAsync<TModelRoot>(It.IsAny<CancellationToken>()))
            .ReturnsAsync([])
            .Verifiable();
    }

    protected override void AssertGetAllOnSuccess(IActionResult actionResult)
    {
        actionResult.Should().NotBeNull().And.BeOfType<OkObjectResult>();

        var result = actionResult as OkObjectResult;
        result!.StatusCode.Should().Be((int)HttpStatusCode.OK);
        result!.Value.Should().NotBeNull().And.BeAssignableTo<IEnumerable<TModelRoot>>();

        ServiceMock.Verify(_ => _.GetAllAsync<TModelRoot>(It.IsAny<CancellationToken>()), Times.Once);
        ServiceMock.VerifyNoOtherCalls();
        ServiceMock.VerifyAll();
    }
}
