using CatNip.Domain.Models.Interfaces;

namespace CatNip.Presentation.IntegrationTest.Controllers;

public abstract partial class BaseCrudControllerIntegrationTests<TModel, TModelRoot, TId>
    : BaseCrudControllerIntegrationTests<TModel, TId>
    where TModel : IModel<TId>
    where TModelRoot : IModel<TId>
    where TId : IEquatable<TId>
{
    protected override async Task AssertGetAllOnSuccessAsync(HttpResponseMessage? response)
    {
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<IEnumerable<TModelRoot>>();
        result.Should().NotBeNull().And.BeAssignableTo<IEnumerable<TModelRoot>>();
        result.Should().NotBeEmpty();
    }
}
