using FridgeCompanionV2Api.Application.TodoLists.Queries.ExportTodos;
using System.Collections.Generic;

namespace FridgeCompanionV2Api.Application.Common.Interfaces
{
    public interface ICsvFileBuilder
    {
        byte[] BuildTodoItemsFile(IEnumerable<TodoItemRecord> records);
    }
}
