using CatNip.Domain.Models.Interfaces;
using CatNip.Domain.Query;
using CatNip.Domain.Query.Filtering;

namespace CatNip.Presentation.IntegrationTest.Controllers;

public abstract partial class BaseAceControllerIntegrationTests<TModel, TModelRoot, TId, TFiltering>
    : BaseCrudControllerIntegrationTests<TModel, TModelRoot, TId>
    where TModel : IModel<TId>
    where TModelRoot : IModel<TId>
    where TId : IEquatable<TId>
    where TFiltering : IFilteringRequest
{
    protected override async Task AssertGetAllOnSuccessAsync(HttpResponseMessage? response)
    {
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<QueryResponse<TModelRoot>>();
        result.Should().NotBeNull().And.BeOfType<QueryResponse<TModel>>();
        result!.Items.Should().NotBeNull().And.BeAssignableTo<IEnumerable<TModelRoot>>();
        result!.Items.Should().NotBeEmpty();
    }
}
