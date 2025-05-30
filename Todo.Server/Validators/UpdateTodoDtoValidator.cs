using FluentValidation;
using TodoServer.Entities;
using TodoServer.Entities.Dtos;

namespace TodoServer.Validators
{
    public class UpdateTodoDtoValidator : AbstractValidator<UpdateTodoDto>
    {
        public UpdateTodoDtoValidator()
        {
            RuleFor(x => x.DueDate)
            .Must((dto, dueDate, context) =>
            {
                if (context.RootContextData.TryGetValue("ExistingTodo", out var existingTodoObj) &&
                    existingTodoObj is Todo existingTodo &&
                    existingTodo.IsCompleted)
                {
                    return dueDate == null ||
                           existingTodo.DueDate?.Date == dueDate.Value.Date;
                }
                return true;
            }).WithMessage("Cannot change due date of a completed todo");
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");

            RuleFor(x => x.Description)
                .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.DueDate)
                .GreaterThanOrEqualTo(DateTime.Today)
                .When(x => x.DueDate.HasValue)
                .WithMessage("Due date must be today or in the future");

            RuleFor(x => x.Tag)
                .MaximumLength(100).WithMessage("Tag cannot exceed 100 characters")
                .When(x => !string.IsNullOrEmpty(x.Tag));

            RuleFor(x => x.Priority)
                .IsInEnum().WithMessage("Invalid priority level");
        }
    }

}
