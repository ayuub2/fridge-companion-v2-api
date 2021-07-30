using FridgeCompanionV2Api.Domain.Common;
using FridgeCompanionV2Api.Domain.ValueObjects;
using System.Collections.Generic;

namespace FridgeCompanionV2Api.Domain.Entities
{
    public class TodoList : AuditableEntity
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public Colour Colour { get; set; } = Colour.White;

        public virtual IList<TodoItem> Items { get; private set; } = new List<TodoItem>();
    }
}
