using FridgeCompanionV2Api.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.Fridge.Commands.UpdateFridgeItem
{
    public class UpdateFridgeItemCommand : IRequest<FridgeItemDto>
    {
        public Guid Id { get; set; }
        public int Amount { get; set; }
        public string UserId { get; set; }
        public DateTime Expiration { get; set; }
        public bool IsDeleted { get; set; }
        public int IngredientId { get; set; }
        public int IngredientLocationId { get; set; }
        public int MeasurementId { get; set; }
    }
}
