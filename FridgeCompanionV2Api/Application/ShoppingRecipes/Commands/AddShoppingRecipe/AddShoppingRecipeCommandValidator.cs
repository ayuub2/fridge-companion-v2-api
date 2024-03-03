using FluentValidation;
using FridgeCompanionV2Api.Application.Common.Interfaces;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace FridgeCompanionV2Api.Application.ShoppingRecipes.Commands.AddShoppingRecipe
{
    public class AddShoppingRecipeCommandValidator : AbstractValidator<AddShoppingRecipeCommand>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly ICurrentUserService _currentUserService;
        public AddShoppingRecipeCommandValidator(IApplicationDbContext applicationDbContext, ICurrentUserService currentUserService)
        {
            _applicationDbContext = applicationDbContext;
            _currentUserService = currentUserService;



            RuleFor(x => x.ServingSize).NotEmpty().WithMessage("Serving size is required.");

            RuleFor(x => x.RecipeId)
                .NotEmpty().WithMessage("Recipe id is required.")
                .MustAsync(RecipeExists).WithMessage("Recipe does not exist.")
                .MustAsync((command, properties, cancellationToken) => RecipeAddedAlready(command.RecipeId, _currentUserService.UserId, cancellationToken)).WithMessage("Recipe already added to shopping list, clear recipe if you want to add it again.");
        }

        private async Task<bool> RecipeExists(int recipeId, CancellationToken cancellationToken)
        {
            var recipe = await _applicationDbContext.Recipes.FindAsync(recipeId);
            return recipe is not null;
        }

        private async Task<bool> RecipeAddedAlready(int recipeId, string userId, CancellationToken cancellationToken)
        {
            var shoppingList = await _applicationDbContext.ShoppingList.FirstOrDefaultAsync(x => x.UserId == userId);
            var recipe = await _applicationDbContext.ShoppingListRecipeItem.FirstOrDefaultAsync(x => x.ShoppingListId == shoppingList.Id && x.RecipeId == recipeId);
            return recipe is null;
        }
    }
}
