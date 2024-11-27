using FluentValidation;

namespace Forum.Application.Messages.Commands.CreateMessage;

public class CreateMessageCommandValidator : AbstractValidator<CreateMessageCommand>
{
    public CreateMessageCommandValidator()
    {
        RuleFor(x => x.UserName).NotEmpty();
        RuleFor(x => x.Text).MaximumLength(5000);
    }
}
