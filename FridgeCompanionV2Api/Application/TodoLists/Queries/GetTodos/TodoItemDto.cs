using AutoMapper;
using FridgeCompanionV2Api.Application.Common.Mappings;
using FridgeCompanionV2Api.Domain.Entities;

namespace FridgeCompanionV2Api.Application.TodoLists.Queries.GetTodos
{
    public class TodoItemDto : IMapFrom<TodoItem>
    {
        public int Id { get; set; }

        public int ListId { get; set; }

        public string Title { get; set; }

        public bool Done { get; set; }

        public int Priority { get; set; }

        public string Note { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<TodoItem, TodoItemDto>()
                .ForMember(d => d.Priority, opt => opt.MapFrom(s => (int)s.Priority));
        }
    }
}
