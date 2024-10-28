using CatNip.Domain.Models.Interfaces;
using CatNip.Domain.Query.Filtering;
using CatNip.Presentation.IntegrationTest.Controllers;

namespace CatNip.Presentation.IntegrationTest.XUnit.Controllers;

public abstract class AceControllerIntegrationTests<TModel, TModelRoot, TId, TFiltering>
    : BaseAceControllerIntegrationTests<TModel, TModelRoot, TId, TFiltering>
    where TModel : IModel<TId>
    where TModelRoot : IModel<TId>
    where TId : IEquatable<TId>
    where TFiltering : IFilteringRequest
{
    [Fact]
    public override async Task GivenGetAllWhenDataExistsThenReturnsData()
    {
        await base.GivenGetAllWhenDataExistsThenReturnsData();
    }
}
