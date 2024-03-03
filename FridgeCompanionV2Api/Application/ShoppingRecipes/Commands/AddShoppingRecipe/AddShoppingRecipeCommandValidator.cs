using FluentValidation;
using FridgeCompanionV2Api.Application.Common.Interfaces;
using System.Threading.Tasks;
using System.Threading;

namespace FridgeCompanionV2Api.Application.ShoppingRecipes.Commands.AddShoppingRecipe
{
    public class AddShoppingRecipeCommandValidator : AbstractValidator<AddShoppingRecipeCommand>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        public AddShoppingRecipeCommandValidator(IApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
            
            RuleFor(x => x.UserId).NotEmpty().WithMessage("User is not authorised.");

            RuleFor(x => x.RecipeId)
                .NotEmpty().WithMessage("Recipe id is required.")
                .MustAsync(RecipeExists).WithMessage("Recipe does not exist.");
        }

        private async Task<bool> RecipeExists(int recipeId, CancellationToken cancellationToken)
        {
            var measurementType = await _applicationDbContext.Recipes.FindAsync(recipeId);
            return measurementType is not null;
        }
    }
}
