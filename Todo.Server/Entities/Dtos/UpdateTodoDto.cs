using System.ComponentModel.DataAnnotations;
using TodoServer.Entities.Enums;

namespace TodoServer.Entities.Dtos
{
    public class UpdateTodoDto
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; }

        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
        public string? Description { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DueDate { get; set; }

        [StringLength(100, ErrorMessage = "Tag cannot exceed 100 characters")]
        public string? Tag { get; set; }

        [Required(ErrorMessage = "Priority is required")]
        [Range(0, 3, ErrorMessage = "Priority must be between 0 and 3")]
        public PriorityLevel Priority { get; set; }

        public bool? IsCompleted { get; set; }
    }
}
