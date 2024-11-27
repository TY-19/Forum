using FluentValidation.Results;

namespace Forum.Application.Common.Exceptions;

public class CustomValidationException : Exception
{
    public IEnumerable<string> Errors { get; }

    public CustomValidationException() : base("One or more validation failures have occured")
    {
        Errors = [];
    }

    public CustomValidationException(IEnumerable<ValidationFailure> failures) : this()
    {
        Errors = failures.Select(f => f.ErrorMessage).ToList();
    }
}
