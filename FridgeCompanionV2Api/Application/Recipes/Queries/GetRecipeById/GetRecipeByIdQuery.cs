using FridgeCompanionV2Api.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.Recipes.Queries.GetRecipeById
{
    public class GetRecipeByIdQuery : IRequest<RecipeDto>
    {
        public int RecipeId { get; set; }
        public string UserId { get; set; }
    }
}
