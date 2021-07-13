using FridgeCompanionV2Api.Domain.Common;
using FridgeCompanionV2Api.Domain.Enums;
using System;
using System.Collections.Generic;

namespace FridgeCompanionV2Api.Domain.Entities
{
    public class TodoItem : AuditableEntity
    {
        public int Id { get; set; }

        public TodoList List { get; set; }

        public int ListId { get; set; }

        public string Title { get; set; }

        public string Note { get; set; }

        public PriorityLevel Priority { get; set; }

        public DateTime? Reminder { get; set; }

        private bool _done;
 
    }
}
