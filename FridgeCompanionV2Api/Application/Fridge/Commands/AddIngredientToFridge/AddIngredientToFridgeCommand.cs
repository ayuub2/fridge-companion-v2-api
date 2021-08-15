using FridgeCompanionV2Api.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.Fridge.Commands.AddIngredientToFridge
{
    public class AddIngredientToFridgeCommand : IRequest<IngredientDto>
    {
        public string UserId { get; set; }
        public int IngredientId { get; set; }
        public DateTime ExpirationDate { get; set; }
        public int MeasurementId { get; set; }
        public int Amount { get; set; }
        public int LocationId { get; set; }
    }
}
