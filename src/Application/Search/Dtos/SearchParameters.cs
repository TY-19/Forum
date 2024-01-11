namespace Forum.Application.Search.Dtos;

public class SearchParameters
{
    public int? PageSize { get; set; }
    public int? SkipNumberElements { get; set; }
    public bool SearchExact { get; set; }
    public bool SearchAllWords { get; set; }
    public Func<string, bool>? CustomSearchFunction { get; set; }
}
