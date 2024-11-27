using FluentValidation;

namespace Forum.Application.Topics.Commands.CreateTopic;

public class CreateTopicCommandValidator : AbstractValidator<CreateTopicCommand>
{
    public CreateTopicCommandValidator()
    {
        RuleFor(t => t.Title).NotEmpty().MaximumLength(500);
        RuleFor(t => t.ParentForumId).NotNull();
    }
}
