using Amazon.Runtime.Internal;
using AutoMapper;
using FridgeCompanionV2Api.Application.Common.Interfaces;
using FridgeCompanionV2Api.Application.Common.Models;
using FridgeCompanionV2Api.Application.User.Queries.GetUserProfile;
using FridgeCompanionV2Api.Domain.Entities;
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

        public async Task<StatDto> Handle(GetStatsQuery request, CancellationToken cancellationToken)
        {
            if (request is null)  
            {
                throw new ArgumentNullException(nameof(request));
            }

            var dateTo = DateTime.UtcNow;
            var dateFrom = DateTime.UtcNow;
            switch (request.Duration.ToLower())
            {
                case "week":
                    dateTo = DateTime.Now.AddDays(-7);
                    dateFrom = DateTime.Now.AddDays(-14);
                    break;
                case "month":
                    dateTo = DateTime.Now.AddMonths(-1);
                    dateFrom = DateTime.Now.AddMonths(-2);
                    break;
                case "year":
                    dateTo = DateTime.Now.AddYears(-1);
                    dateFrom = DateTime.Now.AddYears(-2);
                    break;
                default:
                    throw new InvalidOperationException("Incorrect date duration supplied");
            }

            StatItem recipes = GetRecipesStat(request.UserId, dateTo, dateFrom, request.Duration);

            StatItem expiredIngredients = GetExpiredIngredientStat(request.UserId, dateTo, dateFrom, request.Duration); 

            StatItem favouriteRecipe = GetFavouriteRecipeStat(request.UserId);

            StatItem mostUsedIngredient = GetMostUsedIngredientStat(request.UserId);
            // Money saved stat - In the future add price per ingredient on the ingredients table and use that to calculate money saved
            StatItem moneySaved = GetMoneySavedStat(request.UserId);
            StatItem numberOfIngredientsAdded = GetNumberOfIngredientsAdded(request.UserId);

            return new StatDto()
            {
                Recipes = recipes,
                ExpiredIngredients = expiredIngredients,
                FavouriteRecipe = favouriteRecipe,
                MostUsedIngredient = mostUsedIngredient,
                MoneySaved = moneySaved,
                NumberOfIngredientsAdded = numberOfIngredientsAdded
            };
        }

        private StatItem GetNumberOfIngredientsAdded(string userId)
        {
            var numberOfItemsAddedToFridge = _applicationDbContext.FridgeItems.Where(x => x.UserId == userId).Count();
            return new StatItem()
            {
                Name = "Number of ingredients added",
                Value = numberOfItemsAddedToFridge.ToString()
            };
        }

        private StatItem GetMoneySavedStat(string userId)
        {
            var numberOfItemsAddedToFridge = _applicationDbContext.FridgeItems.Where(x => x.UserId == userId).Count();
            return new StatItem()
            {
                Name = "Money Saved",
                Value = $"£{numberOfItemsAddedToFridge}"
            };
        }

        private StatItem GetMostUsedIngredientStat(string userId)
        {
            var mostUsedIngredient = _applicationDbContext.FridgeItems.Where(x => x.UserId == userId).GroupBy(x => x.IngredientId)
                .Select(x => new
                {
                    IngredientId = x.Key,
                    Count = x.Count()
                }).OrderByDescending(x => x.Count).FirstOrDefault();
            if(mostUsedIngredient is null)
            {
                return new StatItem()
                {
                    Name = "Most used ingredient",
                    Value = "Not enough data"
                };
            }
            var ingredientName = _applicationDbContext.Ingredients.FirstOrDefault(x => x.Id == mostUsedIngredient.IngredientId).Name;
            return new StatItem()
            {
                Name = "Most used ingredient",
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
            if(favouriteRecipeDuringThisRound is null)
            {
                return new StatItem()
                {
                    Name = "Your favourite recipe",
                    Value = "Not enough data"
                };
            }
            var favouriteRecipeName = _applicationDbContext.Recipes.FirstOrDefault(x => x.Id == favouriteRecipeDuringThisRound.RecipeId).Name;
            return new StatItem()
            {
                Name = "Your favourite recipe",
                Value = favouriteRecipeName
            };
        }

        private StatItem GetExpiredIngredientStat(string userId, DateTime dateDuringThisStatRound, DateTime dateDuringLastStatRound, string duration)
        {
            var userFridgeItems = _applicationDbContext.FridgeItems.Where(x => !x.IsDeleted && x.UserId == userId && x.Expiration < DateTime.Now).ToList();
            if(!userFridgeItems.Any())
            {
                return new StatItem()
                {
                    Name = "Expired Ingredients",
                    Value = "0",
                };
            }
            var expiredStatDuringThisRound = userFridgeItems.Where(x => x.Expiration >= dateDuringThisStatRound).Count();
            var expiredStatDuringLastRound = userFridgeItems.Where(x => x.Expiration >= dateDuringLastStatRound
                && x.Expiration < dateDuringThisStatRound).Count();
            var isUp = expiredStatDuringThisRound >= expiredStatDuringLastRound;
            var up = isUp ? "Up" : "Down";
            return new StatItem()
            {
                Name = "Expired Ingredients",
                Value = expiredStatDuringThisRound.ToString(),
                From = new StatFrom()
                {
                    Value = $"{up} {Math.Abs(expiredStatDuringLastRound - expiredStatDuringThisRound)} from last {duration.ToLower()}",
                    IsUp = isUp
                }
            };
        }

        private StatItem GetRecipesStat(string userId, DateTime dateDuringThisStatRound, DateTime dateDuringLastStatRound, string duration)
        {
            var userMadeRecipes = _applicationDbContext.UserMadeRecipes.Where(x => x.UserId == userId);
            if(!userMadeRecipes.Any()) 
            {
                return new StatItem()
                {
                    Name = "Recipes Made",
                    Value = "0",
                };
            }
            var recipesMadeDuringThisRound = userMadeRecipes.Where(x => x.CreatedDate >= dateDuringThisStatRound).Count();
            var recipesMadeDuringLastRound = userMadeRecipes.Where(x => x.CreatedDate >= dateDuringLastStatRound && x.CreatedDate < dateDuringThisStatRound).Count();
            var isUp = recipesMadeDuringThisRound >= recipesMadeDuringLastRound;
            var up = isUp ? "Up" : "Down";
            return new StatItem()
            {
                Name = "Recipes Made",
                Value = recipesMadeDuringThisRound.ToString(),
                From = new StatFrom() 
                {
                    Value = $"{up} {Math.Abs(recipesMadeDuringLastRound - recipesMadeDuringThisRound)} from last {duration.ToLower()}",
                    IsUp = isUp
                }
            };
        }
    }
}
