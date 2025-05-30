using FluentValidation;
using TodoServer.Application.Interfaces;
using TodoServer.Entities.Interfaces;
using TodoServer.Entities;
using TodoServer.Entities.Dtos;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using FluentValidation.Results;

namespace TodoServer.Application.Services
{
    public class TodoService : ITodoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<Todo> _validator;
        private readonly IMapper _mapper; 


        public TodoService(IUnitOfWork unitOfWork, IValidator<Todo> validator, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Todo>> GetAllTodosAsync(bool includeDeleted = false)
        {
            return includeDeleted
                ? await _unitOfWork.TodoRepository.GetAllAsync()
                : await _unitOfWork.TodoRepository.FindAsync(t => !t.IsDeleted);
        }

        public async Task<TodoListResponseDto> GetTodosAsync(TodoListRequestDto request)
        {
            var query = _unitOfWork.TodoRepository.AsQueryable();

            if (!string.IsNullOrEmpty(request.TitleFilter))
            {
                query = query.Where(t => t.Title.Contains(request.TitleFilter.ToLower()));
            }

            if (request.CreatedFrom.HasValue)
            {
                query = query.Where(t => t.CreatedAt >= request.CreatedFrom.Value);
            }

            if (request.CreatedTo.HasValue)
            {
                query = query.Where(t => t.CreatedAt <= request.CreatedTo.Value);
            }

            if (!request.IncludeDeleted)
            {
                query = query.Where(t => !t.IsDeleted);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(t => new TodoItemDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    IsCompleted = t.IsCompleted,
                    CreatedAt = t.CreatedAt,
                    Tag = t.Tag,
                    Priority = t.Priority,
                    DueDate = t.DueDate
                })
                .ToListAsync();

            return new TodoListResponseDto
            {
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                Items = items
            };
        }

        public async Task<Todo> GetTodoByIdAsync(Guid id)
        {
            var todo = await _unitOfWork.TodoRepository.GetByIdAsync(id);
            if (todo == null || todo.IsDeleted)
                throw new KeyNotFoundException($"Todo with id {id} not found");
            return todo;
        }

        public async Task<TodoItemDto> CreateTodoAsync(Todo todo)
        {
            var validationResult = await _validator.ValidateAsync(todo);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            await _unitOfWork.TodoRepository.AddAsync(todo);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<TodoItemDto>(todo); ;
        }

        public async Task UpdateTodoAsync(Todo todo)
        {
            var validationResult = await _validator.ValidateAsync(todo);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var existingTodo = await GetTodoByIdAsync(todo.Id);
            await _unitOfWork.TodoRepository.UpdateAsync(todo);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateTodoAsync(Guid id, UpdateTodoDto updateDto)
        {
            var validationResult = await _validator.ValidateAsync(_mapper.Map<Todo>(updateDto));
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var existingTodo = await _unitOfWork.TodoRepository.GetByIdAsync(id);
            if (existingTodo == null)
            {
                throw new KeyNotFoundException($"Todo with id {id} not found");
            }

            if (existingTodo.IsCompleted && updateDto.DueDate.HasValue)
            {
                var existingDueDate = existingTodo.DueDate ?? DateTime.MinValue;
                var newDueDate = updateDto.DueDate.Value;

                if (existingDueDate.Date != newDueDate.Date)
                {
                    var errors = new List<ValidationFailure>
                {
                    new ValidationFailure("DueDate", "Cannot change due date of a completed todo")
                };
                    throw new ValidationException(errors);
                }
            }

            _mapper.Map(updateDto, existingTodo);
            existingTodo.ModifiedAt = DateTime.UtcNow;

            var entityValidationResult = await _validator.ValidateAsync(existingTodo);
            if (!entityValidationResult.IsValid)
            {
                throw new ValidationException(entityValidationResult.Errors);
            }

            await _unitOfWork.CompleteAsync();
        }

        public async Task SoftDeleteTodoAsync(Guid id)
        {
            var todo = await GetTodoByIdAsync(id);
            todo.IsDeleted = true;
            await _unitOfWork.CompleteAsync();
        }

        public async Task HardDeleteTodoAsync(Guid id)
        {
            var todo = await GetTodoByIdAsync(id);
            await _unitOfWork.TodoRepository.RemoveAsync(todo);
            await _unitOfWork.CompleteAsync();
        }
    }
}
