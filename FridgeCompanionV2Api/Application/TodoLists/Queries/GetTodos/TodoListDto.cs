﻿using FridgeCompanionV2Api.Application.Common.Mappings;
using FridgeCompanionV2Api.Domain.Entities;
using System.Collections.Generic;

namespace FridgeCompanionV2Api.Application.TodoLists.Queries.GetTodos
{
    public class TodoListDto : IMapFrom<TodoList>
    {
        public TodoListDto()
        {
            Items = new List<TodoItemDto>();
        }

        public int Id { get; set; }

        public string Title { get; set; }

        public string Colour { get; set; }

        public IList<TodoItemDto> Items { get; set; }
    }
}
