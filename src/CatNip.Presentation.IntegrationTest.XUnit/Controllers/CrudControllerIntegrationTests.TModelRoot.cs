using CatNip.Domain.Models.Interfaces;
using CatNip.Presentation.IntegrationTest.Controllers;

namespace CatNip.Presentation.IntegrationTest.XUnit.Controllers;

public abstract class CrudControllerIntegrationTests<TModel, TModelRoot, TId>
    : BaseCrudControllerIntegrationTests<TModel, TModelRoot, TId>
    where TModel : IModel<TId>
    where TModelRoot : IModel<TId>
    where TId : IEquatable<TId>
{
    [Fact]
    public override async Task GivenGetAllWhenDataExistsThenReturnsData()
    {
        await base.GivenGetAllWhenDataExistsThenReturnsData();
    }
}
