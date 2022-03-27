using AutoMapper;
using FridgeCompanionV2Api.Application.Common.Exceptions;
using FridgeCompanionV2Api.Application.Common.Interfaces;
using FridgeCompanionV2Api.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.User.Commands.RemoveRecipeIngredientsFromFridge
{
    public class RemoveRecipeIngredientsFromFridgeCommandHandler : IRequestHandler<RemoveRecipeIngredientsFromFridgeCommand>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public RemoveRecipeIngredientsFromFridgeCommandHandler(IApplicationDbContext applicationDbContext, IMapper mapper, ILogger<RemoveRecipeIngredientsFromFridgeCommandHandler> logger)
        {
            _applicationDbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        async Task<Unit> IRequestHandler<RemoveRecipeIngredientsFromFridgeCommand, Unit>.Handle(RemoveRecipeIngredientsFromFridgeCommand request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var freshFridgeItems = _applicationDbContext.FreshFridgeItems(request.UserId).ToList();
            if(freshFridgeItems is not null && freshFridgeItems.Any()) 
            {
                var recipe = _applicationDbContext.GetRecipesWithDetails().FirstOrDefault(x => x.Id == request.RecipeId);
                if (recipe is not null) 
                {
                    foreach (var recipeIngredient in recipe.Ingredients)
                    {
                        var ingredient = recipeIngredient.Ingredient;
                        var recipeAmount = recipeIngredient.Amount * request.Servings;
                        var userIngredient = freshFridgeItems.FirstOrDefault(x => x.IngredientId == ingredient.Id);
                        if (userIngredient is not null) 
                        {
                            // If measurements of both user ingredient and recipe ingredient the same we can subtract them easily
                            if (recipeIngredient.MeasurementId == userIngredient.MeasurementId)
                            {
                                RemoveIngredientFromAmount(userIngredient, userIngredient.Amount, recipeAmount);
                            }
                            else 
                            {
                                // if not we must convert both user and recipe ignredients to grams
                                var recipeIngredientConverter = _applicationDbContext.IngredientMeasurements.FirstOrDefault(x => x.IngredientId == recipeIngredient.Ingredient.Id && x.MeasurementId == recipeIngredient.Measurement.Id);
                                var fridgeIngredientConverter = _applicationDbContext.IngredientMeasurements.FirstOrDefault(x => x.IngredientId == userIngredient.IngredientId && x.MeasurementId == userIngredient.MeasurementId);
                                decimal fridgeIngredientAmountGrams = ConvertMeasurementToGrams(fridgeIngredientConverter, userIngredient.Amount, userIngredient.Measurement);
                                decimal recipeIngredientAmountGrams = ConvertMeasurementToGrams(recipeIngredientConverter, recipeAmount, recipeIngredient.Measurement);

                                // if the users ingredients measurement type is 
                                if (userIngredient.Measurement.Name == "Grams")
                                {
                                    RemoveIngredientFromAmount(userIngredient, userIngredient.Amount, recipeAmount);
                                }
                                else if (userIngredient.Measurement.Name != "Grams")
                                {
                                    var remainingGrams = fridgeIngredientAmountGrams - recipeIngredientAmountGrams;

                                    if (remainingGrams > 0)
                                    {
                                        var amountInFridgeMeasurement = remainingGrams / fridgeIngredientConverter.AverageGrams.Value;
                                        userIngredient.Amount = amountInFridgeMeasurement;
                                    }
                                    else if (remainingGrams <= 0)
                                    {
                                        userIngredient.IsDeleted = true;
                                    }

                                }

                            }
                        }
                    }
                    await _applicationDbContext.SaveChangesAsync(cancellationToken);
                } 
                else 
                {
                    throw new NotFoundException("Recipe not found.");
                }
            }
            return new Unit();
        }

        private void RemoveIngredientFromAmount(FridgeItem fridgeItem, decimal fridgeAmount, decimal recipeAmount)
        {
            if (fridgeAmount > recipeAmount)
            {
                fridgeItem.Amount = fridgeAmount - recipeAmount;
            }
            else
            {
                fridgeItem.IsDeleted = true;
            }
        }

        private decimal ConvertMeasurementToGrams(IngredientMeasurement ingredientMeasurement, decimal amount, MeasurementType measurement) 
        {
            decimal grams;
            if (ingredientMeasurement.AverageGrams is not null)
            {
                grams = ingredientMeasurement.AverageGrams.Value * amount;
            }
            else if (measurement.Name == "Grams")
            {
                grams = amount;
            }
            else
            {
                _logger.LogError($"Unable to convert measurement to grams - {measurement.Id}");
                throw new Exception($"Unable to convert measurement to grams - {measurement.Id}");
            }
            return grams;
        }
    }
}
