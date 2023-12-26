using Forum.Application.Messages.Dtos;
using MediatR;

namespace Forum.Application.Messages.Queries.GetMessage;

public class GetMessageCommand : IRequest<MessageDto?>
{
    public int Id { get; set; }
}
