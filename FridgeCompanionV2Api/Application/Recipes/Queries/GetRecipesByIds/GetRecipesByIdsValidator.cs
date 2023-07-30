using FluentValidation;

namespace FridgeCompanionV2Api.Application.Recipes.Queries.GetRecipesByIds
{
    public class GetRecipesByIdsValidator : AbstractValidator<GetRecipesByIdsQuery>
    {
        public GetRecipesByIdsValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User is not authorized.");
        }
    }
}
