namespace Forum.Application.Common.Models;

public class PaginatedResponse<T>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPagesCount { get; set; }
    public IEnumerable<T> Elements { get; set; } = new List<T>();
}
