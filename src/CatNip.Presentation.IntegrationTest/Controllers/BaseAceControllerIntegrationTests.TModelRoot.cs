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
    public override async Task GivenGetAllWhenDataExistsThenReturnsData()
    {
        // Arrange
        var uri = new Uri(Endpoint, UriKind.Relative);

        // Act
        var response = await HttpClient.GetAsync(uri);
        var result = await response.Content.ReadFromJsonAsync<QueryResponse<TModelRoot>>();

        // Assert
        response.Should().NotBeNull();
        response.Should().HaveStatusCode(HttpStatusCode.OK);
        result!.Should().NotBeNull();
        result!.Should().BeOfType<QueryResponse<TModelRoot>>();
        result!.Items.Should().NotBeNull();
        result!.Items.Should().NotBeEmpty();
        result!.Items.Should().BeAssignableTo<IEnumerable<TModelRoot>>();
    }
}
