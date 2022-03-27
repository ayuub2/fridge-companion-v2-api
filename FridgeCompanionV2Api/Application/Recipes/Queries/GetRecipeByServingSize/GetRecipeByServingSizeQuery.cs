using FridgeCompanionV2Api.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.Recipes.Queries.GetRecipeByServingSize
{
    public class GetRecipeByServingSizeQuery : IRequest<RecipeDto>
    {
        public string UserId { get; set; }
        public int RecipeId { get; set; }
        public int ServingSize { get; set; }
    }
}
