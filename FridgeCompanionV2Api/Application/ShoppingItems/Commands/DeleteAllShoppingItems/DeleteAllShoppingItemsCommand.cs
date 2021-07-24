using FridgeCompanionV2Api.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.ShoppingItems.Commands.CreateShoppingItem
{
    public class DeleteAllShoppingItemsCommand : IRequest<List<ShoppingItemDto>>
    {
        public string UserId { get; set; }
    }
}
