namespace Forum.Application.Common.Models;

public class RequestParameters
{
    public int? PageSize { get; set; }
    public int? PageNumber { get; set; }
    public string? OrderBy { get; set; }
    public bool? OrderAscending { get; set; }
    public string? FilterBy { get; set; }
    public string? FilterText { get; set; }
}
