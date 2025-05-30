using System.ComponentModel.DataAnnotations;
using TodoServer.Entities.Enums;
using TodoServer.Entities.Interfaces;

namespace TodoServer.Entities
{
    public class Todo : IAuditableEntity
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ModifiedAt { get; set; }
        public bool IsDeleted { get; set; }
        public PriorityLevel Priority { get; set; } = PriorityLevel.Medium;
        public string Tag { get; set; }
    }
}
