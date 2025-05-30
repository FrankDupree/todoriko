using TodoServer.Entities;
using TodoServer.Entities.Dtos;

namespace TodoServer.Application.Interfaces
{
    public interface ITodoService
    {
        Task<IEnumerable<Todo>> GetAllTodosAsync(bool includeDeleted = false);
        Task<TodoListResponseDto> GetTodosAsync(TodoListRequestDto request);
        Task<Todo> GetTodoByIdAsync(Guid id);
        Task<TodoItemDto> CreateTodoAsync(Todo todo);
        Task UpdateTodoAsync(Guid id, UpdateTodoDto updateDto);
        Task UpdateTodoAsync(Todo todo);
        Task SoftDeleteTodoAsync(Guid id);
        Task HardDeleteTodoAsync(Guid id);
    }
}
