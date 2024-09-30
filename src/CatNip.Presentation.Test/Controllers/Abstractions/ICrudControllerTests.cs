namespace CatNip.Presentation.Test.Controllers.Abstractions;

public interface ICrudControllerTests
{
    Task GivenGetAllWhenDataExistsThenReturnsData();

    Task GivenCountWhenDataExistsThenReturnsCount();

    Task GivenGetByIdWhenDataExistsThenReturnsData();
    Task GivenGetByIdWhenDataNotFoundThenThrowsException();

    Task GivenCreateWhenInputIsValidThenCreatesData();

    Task GivenUpdateWhenDataExistsThenUpdatesData();
    Task GivenUpdateWhenDataNotFoundThenThrowsException();

    Task GivenDeleteWhenDataExistsThenDeletesData();
    Task GivenDeleteWhenDataNotFoundThenThrowsException();
}
