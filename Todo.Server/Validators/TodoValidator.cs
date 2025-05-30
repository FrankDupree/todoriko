using FluentValidation;
using TodoServer.Entities;

namespace TodoServer.Validators
{
    public class TodoValidator : AbstractValidator<Todo>
    {
        public TodoValidator()
        {
            // Title validation
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .Length(2, 200).WithMessage("Title must be between 2 and 200 characters")
                .Matches(@"^[\w\s.,!?-]+$").WithMessage("Title contains invalid characters");

            // Description validation
            RuleFor(x => x.Description)
                .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters")
                .When(x => !string.IsNullOrEmpty(x.Description));

            // DueDate validation
            RuleFor(x => x.DueDate)
                .GreaterThanOrEqualTo(DateTime.Today)
                .WithMessage("Due date must be today or in the future")
                .When(x => x.DueDate.HasValue);

            // Priority validation
            RuleFor(x => x.Priority)
                .IsInEnum().WithMessage("Invalid priority level");

            // Tag validation
            RuleFor(x => x.Tag)
                .MaximumLength(100).WithMessage("Tag cannot exceed 100 characters")
                .Matches(@"^[a-zA-Z0-9\s-]+$").WithMessage("Tag contains invalid characters")
                .When(x => !string.IsNullOrEmpty(x.Tag));
        }
    }
}
