using TodoServer.Entities.Enums;

namespace TodoServer.Entities.Dtos
{
    public class TodoItemDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Tag { get; set; }
        public PriorityLevel Priority { get; set; }
    }
}
