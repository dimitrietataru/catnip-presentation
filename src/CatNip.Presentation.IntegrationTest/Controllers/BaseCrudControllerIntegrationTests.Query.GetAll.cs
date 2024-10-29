using CatNip.Domain.Models.Interfaces;

namespace CatNip.Presentation.IntegrationTest.Controllers;

public abstract partial class BaseCrudControllerIntegrationTests<TModel, TId>
    where TModel : IModel<TId>
    where TId : IEquatable<TId>
{
    public virtual async Task GivenGetAllWhenDataExistsThenReturnsData()
    {
        // Arrange
        var uri = new Uri(Endpoint, UriKind.Relative);

        // Act
        var response = await HttpClient.GetAsync(uri);
        var result = await response.Content.ReadFromJsonAsync<IEnumerable<TModel>>();

        // Assert
        response.Should().NotBeNull();
        response.Should().HaveStatusCode(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
        result.Should().BeAssignableTo<IEnumerable<TModel>>();
    }
}
