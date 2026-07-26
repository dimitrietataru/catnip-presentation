namespace CatNip.Presentation.Models;

public class ImportResponseModel
{
    public required int TotalRows { get; init; }
    public required int CreatedRecords { get; init; }
    public required int UpdatedRecords { get; init; }
}
