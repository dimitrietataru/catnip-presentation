using CatNip.Domain.Models.Interfaces;
using CatNip.Domain.Query;
using CatNip.Domain.Query.Filtering;

namespace CatNip.Presentation.IntegrationTest.Controllers;

public abstract class BaseAceControllerIntegrationTests<TModel, TId, TModelRoot, TFiltering>
    : BaseAceControllerIntegrationTests<TModel, TId, TId, TModelRoot, TFiltering>
    where TModel : IAceModel<TId>
    where TId : IEquatable<TId>
    where TModelRoot : ICrudModel<TId>
    where TFiltering : IFilteringRequest
{
}

public abstract class BaseAceControllerIntegrationTests<TModel, TId, TTraceId, TModelRoot, TFiltering>
    : BaseCrudControllerIntegrationTests<TModel, TId>
    where TModel : IAceModel<TId, TTraceId>
    where TId : IEquatable<TId>
    where TTraceId : IEquatable<TTraceId>
    where TModelRoot : ICrudModel<TId>
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
        result.Should().NotBeNull().And.BeOfType<QueryResponse<TModelRoot>>();
        result!.Items.Should().NotBeNull();
        result!.Items.Should().NotBeEmpty();
        result!.Items.Should().BeAssignableTo<IEnumerable<TModelRoot>>();
    }
}
