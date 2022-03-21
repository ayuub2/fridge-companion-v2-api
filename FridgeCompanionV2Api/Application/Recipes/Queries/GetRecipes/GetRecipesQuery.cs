using FridgeCompanionV2Api.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.Recipes.Queries.GetRecipes
{
    public class GetRecipesQuery : IRequest<List<RecipeDto>>
    {
        public string UserId { get; set; }
        public List<int> ExcludeRecipes { get; set; } = new List<int>();
    }
}
