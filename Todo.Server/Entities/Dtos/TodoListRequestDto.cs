using System.ComponentModel.DataAnnotations;

namespace TodoServer.Entities.Dtos
{
    public class TodoListRequestDto
    {
        [Range(1, int.MaxValue)]
        public int PageNumber { get; set; } = 1;

        [Range(1, 100)]
        public int PageSize { get; set; } = 10;

        public string? TitleFilter { get; set; }
        public DateTime? CreatedFrom { get; set; }
        public DateTime? CreatedTo { get; set; }
        public bool IncludeDeleted { get; set; } = false;
    }
}
