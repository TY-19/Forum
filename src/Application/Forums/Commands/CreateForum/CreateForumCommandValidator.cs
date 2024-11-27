using FluentValidation;

namespace Forum.Application.Forums.Commands.CreateForum;

public class CreateForumCommandValidator : AbstractValidator<CreateForumCommand>
{
    public CreateForumCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MinimumLength(3).MaximumLength(50);
        RuleFor(x => x.Category).MaximumLength(50);
        RuleFor(x => x.Description).MaximumLength(500);
    }
}
