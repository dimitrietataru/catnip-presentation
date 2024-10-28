using CatNip.Domain.Models.Interfaces;

namespace CatNip.Presentation.IntegrationTest.Controllers;

public abstract partial class BaseCrudControllerIntegrationTests<TModel, TId>
    where TModel : IModel<TId>
    where TId : IEquatable<TId>
{
    public virtual async Task GivenGetAllWhenDataExistsThenReturnsData()
    {
        // Arrange
        var uri = ArrangeGetAllUri();

        // Act
        var response = await HttpClient.GetAsync(uri);

        // Assert
        await AssertGetAllOnSuccessAsync(response);
    }

    protected virtual Uri ArrangeGetAllUri()
    {
        var uri = new Uri(Endpoint, UriKind.Relative);

        return uri;
    }

    protected virtual async Task AssertGetAllOnSuccessAsync(HttpResponseMessage? response)
    {
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<IEnumerable<TModel>>();
        result.Should().NotBeNull().And.BeAssignableTo<IEnumerable<TModel>>();
        result.Should().NotBeEmpty();
    }
}
