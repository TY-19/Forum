using Forum.Application.Common.Interfaces.Repositories;
using Forum.Application.Messages.Dtos;
using MediatR;

namespace Forum.Application.Messages.Queries.GetMessage;

public class GetMessageCommandHandler(IMessageRepository repository) : IRequestHandler<GetMessageCommand, MessageDto?>
{
    public async Task<MessageDto?> Handle(GetMessageCommand request, CancellationToken cancellationToken)
    {
        return await repository.GetMessageDtoAsync(request.Id, cancellationToken);
    }
}
