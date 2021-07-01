using FridgeCompanionV2Api.Application.Common.Mappings;
using FridgeCompanionV2Api.Domain.Entities;

namespace FridgeCompanionV2Api.Application.TodoLists.Queries.ExportTodos
{
    public class TodoItemRecord : IMapFrom<TodoItem>
    {
        public string Title { get; set; }

        public bool Done { get; set; }
    }
}
