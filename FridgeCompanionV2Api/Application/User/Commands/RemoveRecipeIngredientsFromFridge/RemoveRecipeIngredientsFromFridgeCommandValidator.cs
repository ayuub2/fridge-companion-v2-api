using FluentValidation;

namespace FridgeCompanionV2Api.Application.User.Commands.RemoveRecipeIngredientsFromFridge
{
    public class RemoveRecipeIngredientsFromFridgeCommandValidator : AbstractValidator<RemoveRecipeIngredientsFromFridgeCommand>
    {
        public RemoveRecipeIngredientsFromFridgeCommandValidator()
        {
            RuleFor(x => x.UserId)
                   .NotEmpty().WithMessage("User is not authorized");
            RuleFor(x => x.RecipeId)
                .NotEmpty().WithMessage("Please supply a recipe id.");
            RuleFor(x => x.Servings)
                .NotEmpty().WithMessage("Please supply the number of servings.");
        }
    }
}