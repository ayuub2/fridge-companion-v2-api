using Amazon.Runtime.Internal;
using FridgeCompanionV2Api.Application.Common.Models;
using MediatR;
using System.Collections.Generic;

namespace FridgeCompanionV2Api.Application.Recipes.Queries.GetRecipesByIds
{
    public class GetRecipesByIdsQuery : IRequest<List<RecipeDto>>
    {
        public string UserId { get; set; }
        public List<int> RecipeIds { get; set; }
    }
}
