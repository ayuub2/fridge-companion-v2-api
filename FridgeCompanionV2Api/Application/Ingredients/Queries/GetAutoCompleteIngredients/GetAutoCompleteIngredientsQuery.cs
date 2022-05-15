using FridgeCompanionV2Api.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.Ingredients.Queries.GetAutoCompleteIngredients
{
    public class GetAutoCompleteIngredientsQuery : IRequest<List<IngredientDto>>
    {
        public string UserId { get; set; }
        public string Query { get; set; }
    }
}
