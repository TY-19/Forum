using FluentValidation;

namespace Forum.Application.UnreadElements.Commands.MarkTopicsAsRead;

public class MarkTopicAsReadCommandValidator : AbstractValidator<MarkTopicAsReadCommand>
{
    public MarkTopicAsReadCommandValidator()
    {
        RuleFor(x => x.UserName).NotEmpty();
        RuleFor(x => x.TopicId).NotEmpty();
    }
}
