namespace TodoServer.Entities.Dtos
{
    public class TodoListResponseDto
    {
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public IEnumerable<TodoItemDto> Items { get; set; } = new List<TodoItemDto>();
    }
}
