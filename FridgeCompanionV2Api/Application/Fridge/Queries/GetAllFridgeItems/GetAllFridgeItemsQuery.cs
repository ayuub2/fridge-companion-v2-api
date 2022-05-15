using FridgeCompanionV2Api.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.Fridge.Queries.GetAllFridgeItems
{
    public class GetAllFridgeItemsQuery : IRequest<List<FridgeItemDto>>
    {
        public string UserId { get; set; }
    }
}
