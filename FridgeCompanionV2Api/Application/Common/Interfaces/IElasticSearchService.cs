using FridgeCompanionV2Api.Application.Common.Models;
using FridgeCompanionV2Api.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.Common.Interfaces
{
    public interface IElasticSearchService
    {
        Task BulkIndexIngredientsAsync(List<Ingredient> recipes);
        Task BulkIndexRecipesAsync(List<Recipe> recipes);
        Task<List<ElasticModelRequest>> SearchByIngredientNameAsync(string query);
        Task<List<ElasticModelRequest>> SearchByRecipeNameAsync(string query);
    }
}