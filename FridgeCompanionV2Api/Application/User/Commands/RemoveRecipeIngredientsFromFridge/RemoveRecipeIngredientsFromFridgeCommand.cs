using MediatR;

namespace FridgeCompanionV2Api.Application.User.Commands.RemoveRecipeIngredientsFromFridge
{
    public class RemoveRecipeIngredientsFromFridgeCommand : IRequest
    {
        public string UserId { get; set; }
        public int RecipeId { get; set; }
        public int Servings { get; set; }
    }
}
