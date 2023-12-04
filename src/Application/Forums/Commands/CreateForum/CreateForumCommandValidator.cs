using FluentValidation;

namespace Forum.Application.Forums.Commands.CreateForum;

public class CreateForumCommandValidator : AbstractValidator<CreateForumCommand>
{
    public CreateForumCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MinimumLength(3);
    }
}
