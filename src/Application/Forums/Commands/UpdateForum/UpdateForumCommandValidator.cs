using FluentValidation;

namespace Forum.Application.Forums.Commands.UpdateForum;

public class UpdateForumCommandValidator : AbstractValidator<UpdateForumCommand>
{
    public UpdateForumCommandValidator()
    {
        RuleFor(x => x.Name).MinimumLength(3).MaximumLength(50);
        RuleFor(x => x.Category).MaximumLength(50);
        RuleFor(x => x.Description).MaximumLength(500);
    }
}
