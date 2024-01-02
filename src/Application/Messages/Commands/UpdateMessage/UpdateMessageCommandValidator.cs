using FluentValidation;

namespace Forum.Application.Messages.Commands.UpdateMessage;

public class UpdateMessageCommandValidator : AbstractValidator<UpdateMessageCommand>
{
    public UpdateMessageCommandValidator()
    {
        RuleFor(x => x.UserName).NotEmpty();
        RuleFor(x => x.Text).MaximumLength(5000);
    }
}
