using FridgeCompanionV2Api.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.ShoppingItems.Commands.CreateShoppingItem
{
    public class DeleteShoppingItemCommand : IRequest<ShoppingItemDto>
    {
        public string UserId { get; set; }
        public Guid Id { get; set; }
    }
}
