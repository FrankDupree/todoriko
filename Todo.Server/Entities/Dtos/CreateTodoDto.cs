using System.ComponentModel.DataAnnotations;
using TodoServer.Entities.Enums;

namespace TodoServer.Entities.Dtos
{
    public class CreateTodoDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [StringLength(2000)]
        public string? Description { get; set; }

        public DateTime? DueDate { get; set; }
        public string? Tag { get; set; }
        public PriorityLevel Priority { get; set; } = PriorityLevel.Medium;
    }
}
