using static System.Runtime.InteropServices.JavaScript.JSType;
using TodoServer.Entities.Dtos;
using TodoServer.Entities;
using AutoMapper;

namespace TodoServer.Application.Mapping
{
    public class TodoProfile : Profile
    {
        public TodoProfile()
        {
            CreateMap<CreateTodoDto, Todo>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Tag, opt => opt.MapFrom(src =>
                    string.IsNullOrEmpty(src.Tag) ? "uncategorized" : src.Tag))
                .ForMember(dest => dest.IsCompleted, opt => opt.MapFrom(_ => false))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.ModifiedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src =>
                    string.IsNullOrEmpty(src.Description) ? null : src.Description))
                .ForMember(dest => dest.DueDate, opt => opt.MapFrom(src =>
                    src.DueDate.HasValue && src.DueDate.Value > DateTime.UtcNow
                        ? src.DueDate.Value
                        : (DateTime?)null))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false));

            CreateMap<Todo, TodoItemDto>();

            CreateMap<UpdateTodoDto, Todo>()
                  .ForMember(dest => dest.Title, opt =>
                  {
                      opt.PreCondition(src => !string.IsNullOrWhiteSpace(src.Title));
                      opt.MapFrom(src => src.Title.Trim());
                  })
                  .ForMember(dest => dest.Description, opt =>
                  {
                      opt.PreCondition(src => !string.IsNullOrWhiteSpace(src.Description));
                      opt.MapFrom(src => src.Description!.Trim());
                  })
                  .ForMember(dest => dest.DueDate, opt =>
                  {
                      opt.PreCondition(src => src.DueDate.HasValue);
                      opt.MapFrom(src => src.DueDate.Value.Date);
                  })
                  .ForMember(dest => dest.Tag, opt =>
                  {
                      opt.PreCondition(src => !string.IsNullOrWhiteSpace(src.Tag));
                      opt.MapFrom(src => src.Tag!.Trim());
                  })
                  .ForMember(dest => dest.Priority, opt =>
                  {
                      opt.MapFrom(src => src.Priority);
                  })
                  .ForMember(dest => dest.IsCompleted, opt =>
                  {
                      opt.PreCondition(src => src.IsCompleted.HasValue);
                      opt.MapFrom(src => src.IsCompleted.Value);
                  });

        }
    }
}
