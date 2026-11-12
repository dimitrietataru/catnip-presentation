using CatNip.Domain.Models.Interfaces;
using CatNip.Domain.Query.Filtering;
using CatNip.Presentation.IntegrationTest.Controllers;

namespace CatNip.Presentation.IntegrationTest.XUnit.Controllers;

public abstract class AceControllerIntegrationTests<TModel, TId, TModelRoot, TFiltering>
    : AceControllerIntegrationTests<TModel, TId, TId, TModelRoot, TFiltering>
    where TModel : IAceModel<TId>
    where TId : IEquatable<TId>
    where TModelRoot : ICrudModel<TId>
    where TFiltering : IFilteringRequest
{
}

public abstract class AceControllerIntegrationTests<TModel, TId, TTraceId, TModelRoot, TFiltering>
    : BaseAceControllerIntegrationTests<TModel, TId, TTraceId, TModelRoot, TFiltering>
    where TModel : IAceModel<TId, TTraceId>
    where TId : IEquatable<TId>
    where TTraceId : IEquatable<TTraceId>
    where TModelRoot : ICrudModel<TId>
    where TFiltering : IFilteringRequest
{
    [Fact]
    public override async Task GivenGetAllWhenDataExistsThenReturnsData()
    {
        await base.GivenGetAllWhenDataExistsThenReturnsData();
    }
}
