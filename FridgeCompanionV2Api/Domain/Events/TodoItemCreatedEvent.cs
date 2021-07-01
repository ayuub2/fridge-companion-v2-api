using FridgeCompanionV2Api.Domain.Common;
using FridgeCompanionV2Api.Domain.Entities;

namespace FridgeCompanionV2Api.Domain.Events
{
    public class TodoItemCreatedEvent : DomainEvent
    {
        public TodoItemCreatedEvent(TodoItem item)
        {
            Item = item;
        }

        public TodoItem Item { get; }
    }
}
