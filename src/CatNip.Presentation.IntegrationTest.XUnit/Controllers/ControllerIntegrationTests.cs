using CatNip.Domain.Models.Interfaces;
using CatNip.Presentation.IntegrationTest.Controllers;

namespace CatNip.Presentation.IntegrationTest.XUnit.Controllers;

public abstract class ControllerIntegrationTests<TModel>
    : BaseControllerIntegrationTests<TModel>
    where TModel : IModel
{
    [Fact]
    public override async Task GivenGetAllWhenDataExistsThenReturnsData()
    {
        await base.GivenGetAllWhenDataExistsThenReturnsData();
    }
}
