using FridgeCompanionV2Api.Domain.Common;
using FridgeCompanionV2Api.Domain.Entities;

namespace FridgeCompanionV2Api.Domain.Events
{
    public class TodoItemCompletedEvent : DomainEvent
    {
        public TodoItemCompletedEvent(TodoItem item)
        {
            Item = item;
        }

        public TodoItem Item { get; }
    }
}
