using FluentValidation;

namespace FridgeCompanionV2Api.Application.Stats.Queries.GetStats
{
    public class GetStatsQueryValidator : AbstractValidator<GetStatsQuery>
    {
        public GetStatsQueryValidator() 
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("User Id not provided.");
            RuleFor(x => x.Duration).NotEmpty().WithMessage("Stat duration must not be empty.");
        }
    }
}
