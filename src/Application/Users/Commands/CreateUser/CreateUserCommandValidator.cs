using FluentValidation;

namespace Forum.Application.Users.Commands.CreateUser;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(u => u.UserName).NotEmpty().MinimumLength(3).MaximumLength(100);
        RuleFor(u => u.UserEmail).NotEmpty().EmailAddress().MaximumLength(500);
        RuleFor(u => u.Password).NotEmpty().MinimumLength(8).MaximumLength(50);
    }
}
