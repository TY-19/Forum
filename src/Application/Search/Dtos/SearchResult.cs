namespace Forum.Application.Search.Dtos;

public class SearchResult<T>
{
    public IEnumerable<T> Elements { get; set; } = new List<T>();
    public bool HasNextPage { get; set; }
    public int? SkipElementsForNextPage { get; set; }
}
