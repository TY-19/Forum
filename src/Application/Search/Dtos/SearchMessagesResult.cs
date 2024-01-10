using Forum.Application.Messages.Dtos;

namespace Forum.Application.Search.Dtos;

public class SearchMessagesResult
{
    public IEnumerable<MessageDto> Messages { get; set; } = new List<MessageDto>();
    public int CurrentPage { get; set; }
    public int? NextPage { get; set; }
}
