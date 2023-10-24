using Amazon.Runtime.Internal;
using AutoMapper;
using FridgeCompanionV2Api.Application.Common.Interfaces;
using FridgeCompanionV2Api.Application.Common.Models;
using FridgeCompanionV2Api.Application.User.Queries.GetUserProfile;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.Stats.Queries.GetStats
{
    public class GetStatsQueryHandler : IRequestHandler<GetStatsQuery, StatDto>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public GetStatsQueryHandler(IApplicationDbContext applicationDbContext, IMapper mapper, ILogger<GetStatsQueryHandler> logger)
        {
            _applicationDbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task<StatDto> Handle(GetStatsQuery request, CancellationToken cancellationToken)
        {
            if (request is null)  
            {
                throw new ArgumentNullException(nameof(request));
            }

            var dateTo = DateTime.UtcNow;
            var dateFrom = DateTime.UtcNow;
            switch (request.Duration)
            {
                case "Week":
                    dateTo = DateTime.Now.AddDays(-7);
                    dateFrom = DateTime.Now.AddDays(-14);
                    break;
                case "Month":
                    dateTo = DateTime.Now.AddMonths(-1);
                    dateFrom = DateTime.Now.AddMonths(-2);
                    break;
                case "Year":
                    dateTo = DateTime.Now.AddYears(-1);
                    dateFrom = DateTime.Now.AddYears(-2);
                    break;
                default:
                    throw new InvalidOperationException("Incorrect date duration supplied");
            }

            var recipes = GetRecipesStat(request.UserId, dateTo, dateFrom, request.Duration);

            StatItem expiredIngredients = GetExpiredIngredientStat(request.UserId, dateTo, dateFrom, request.Duration);

            StatItem favouriteRecipe = GetFavouriteRecipeStat(request.UserId);

            StatItem mostUsedIngredient = GetMostUsedIngredientStat(request.UserId);
        }

        private StatItem GetMostUsedIngredientStat(string userId)
        {
            var mostUsedIngredient = _applicationDbContext.FridgeItems.Where(x => x.UserId == userId).GroupBy(x => x.IngredientId)
                .Select(x => new
                {
                    IngredientId = x.Key,
                    Count = x.Count()
                }).OrderByDescending(x => x.Count).FirstOrDefault();
            var ingredientName = _applicationDbContext.Ingredients.FirstOrDefault(x => x.Id == mostUsedIngredient.IngredientId).Name;
            return new StatItem()
            {
                Name = "Most used ingredient.",
                Value = ingredientName
            };
        }

        private StatItem GetFavouriteRecipeStat(string userId)
        {
            var favouriteRecipeDuringThisRound = _applicationDbContext.UserMadeRecipes.Where(x => x.UserId == userId).GroupBy(x => x.RecipeId)
                .Select(x => new
                {
                    RecipeId = x.Key,
                    Count = x.Count()
                }).OrderByDescending(x => x.Count).FirstOrDefault();
            var favouriteRecipeName = _applicationDbContext.Recipes.FirstOrDefault(x => x.Id == favouriteRecipeDuringThisRound.RecipeId).Name;
            return new StatItem()
            {
                Name = "Your favourite recipe",
                Value = favouriteRecipeName
            };
        }

        private StatItem GetExpiredIngredientStat(string userId, DateTime dateDuringThisStatRound, DateTime dateDuringLastStatRound, string duration)
        {
            var expiredStatDuringThisRound = _applicationDbContext.FridgeItems.Where(x => !x.IsDeleted && x.UserId == userId && x.Expiration <= dateDuringThisStatRound).Count();
            var expiredStatDuringLastRound = _applicationDbContext.FridgeItems.Where(x => !x.IsDeleted && x.UserId == userId
                && x.Expiration >= dateDuringLastStatRound
                && x.Expiration < dateDuringThisStatRound).Count();
            var isUp = expiredStatDuringThisRound > expiredStatDuringLastRound;
            var up = isUp ? "Up" : "Down";
            return new StatItem()
            {
                Name = "Expired Ingredients",
                Value = expiredStatDuringThisRound.ToString(),
                From = $"{up} {Math.Abs(expiredStatDuringLastRound - expiredStatDuringThisRound)} last {duration.ToLower()}",
                IsUp = isUp
            };
        }

        private StatItem GetRecipesStat(string userId, DateTime dateDuringThisStatRound, DateTime dateDuringLastStatRound, string duration)
        {

            var recipesMadeDuringThisRound = _applicationDbContext.UserMadeRecipes.Where(x => x.UserId == userId && x.CreatedDate >= dateDuringThisStatRound).Count();
            var recipesMadeDuringLastRound = _applicationDbContext.UserMadeRecipes.Where(x => x.UserId == userId && x.CreatedDate >= dateDuringLastStatRound && x.CreatedDate < dateDuringThisStatRound).Count();
            var isUp = recipesMadeDuringThisRound > recipesMadeDuringLastRound;
            var up = isUp ? "Up" : "Down";
            return new StatItem()
            {
                Name = "Recipes",
                Value = recipesMadeDuringThisRound.ToString(),
                From =  $"{up} {Math.Abs(recipesMadeDuringLastRound - recipesMadeDuringThisRound)} last {duration.ToLower()}",
                IsUp = isUp
            };
        }
    }
}
