﻿using FluentValidation;

namespace Forum.Application.Topics.Commands.UpdateTopic;

public class UpdateTopicCommandValidator : AbstractValidator<UpdateTopicCommand>
{
    public UpdateTopicCommandValidator()
    {
        RuleFor(c => c.Title).NotEmpty().MaximumLength(500);
    }
}
