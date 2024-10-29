using CatNip.Domain.Models.Interfaces;

namespace CatNip.Presentation.IntegrationTest.Controllers;

public abstract partial class BaseCrudControllerIntegrationTests<TModel, TModelRoot, TId>
    : BaseCrudControllerIntegrationTests<TModel, TId>
    where TModel : IModel<TId>
    where TModelRoot : IModel<TId>
    where TId : IEquatable<TId>
{
    public override async Task GivenGetAllWhenDataExistsThenReturnsData()
    {
        // Arrange
        var uri = new Uri(Endpoint, UriKind.Relative);

        // Act
        var response = await HttpClient.GetAsync(uri);
        var result = await response.Content.ReadFromJsonAsync<IEnumerable<TModelRoot>>();

        // Assert
        response.Should().NotBeNull();
        response.Should().HaveStatusCode(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
        result.Should().BeAssignableTo<IEnumerable<TModelRoot>>();
    }
}
