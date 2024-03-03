using AutoMapper;
using FridgeCompanionV2Api.Application.Common.Interfaces;
using FridgeCompanionV2Api.Application.Common.Models;
using FridgeCompanionV2Api.Application.ShoppingItems.Commands.CreateShoppingItem;
using FridgeCompanionV2Api.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.ShoppingRecipes.Commands.AddShoppingRecipe
{
    public class AddShoppingRecipeCommandHandler : IRequestHandler<AddShoppingRecipeCommand, ShoppingListRecipeDto>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public AddShoppingRecipeCommandHandler(IApplicationDbContext applicationDbContext, IMapper mapper, ILogger<AddShoppingRecipeCommandHandler> logger)
        {
            _applicationDbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ShoppingListRecipeDto> Handle(AddShoppingRecipeCommand request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            _logger.LogInformation("Adding shopping list item for user {UserId}", request.UserId);
            var shoppingList = await _applicationDbContext.ShoppingList.Include(x => x.Items).FirstOrDefaultAsync(x => x.UserId == request.UserId);

            if (shoppingList == null) throw new Exception("Default shopping list isn't created, call get shopping list endpoint first.");

            var recipe = await _applicationDbContext.Recipes
                .AsNoTracking()
                .Include(x => x.Ingredients)
                    .ThenInclude(x => x.Measurement)
                .FirstOrDefaultAsync(x => x.Id == request.RecipeId);

            var shoppingListRecipe = _applicationDbContext.ShoppingListRecipeItem.Add(new ShoppingListRecipeItem() 
            {
                RecipeId = request.RecipeId,
                ShoppingListId = shoppingList.Id,
            });


            // get recipe and add each item to the database, combine ingredients if they existr in the shopping list
            foreach(var ingredient in recipe.Ingredients)
            {
                var ingredientAlreadyExists = shoppingList.Items.FirstOrDefault(x => x.IngredientId == ingredient.IngredientId);
                if (ingredientAlreadyExists is not null)
                {
                    // if the ingredients have the same measurment then just add the amounts.
                    if(ingredientAlreadyExists.MeasurementId == ingredient.MeasurementId)
                    {
                        _applicationDbContext.ShoppingListItem.Add(new ShoppingListItem()
                        {
                            Id = Guid.NewGuid(),
                            Amount = ingredient.Amount + ingredientAlreadyExists.Amount,
                            MeasurementId = ingredient.MeasurementId,
                            IngredientId = ingredient.IngredientId,
                            ShoppingListId = shoppingList.Id
                        });
                    } else
                    {
                        // if measurements are different convert to grams and then add them together 
                        var newIngredientConverter = _applicationDbContext.IngredientMeasurements.FirstOrDefault(x => x.IngredientId == ingredient.IngredientId && x.MeasurementId == ingredient.MeasurementId);
                        var oldIngredientConverter = _applicationDbContext.IngredientMeasurements.FirstOrDefault(x => x.IngredientId == ingredientAlreadyExists.IngredientId && x.MeasurementId == ingredientAlreadyExists.MeasurementId);
                        var newIngredientGrams = ConvertMeasurementToGrams(newIngredientConverter, ingredient.Amount, ingredient.Measurement);
                        var oldIngredientGrams = ConvertMeasurementToGrams(oldIngredientConverter, ingredientAlreadyExists.Amount, ingredientAlreadyExists.Measurement);

                        _applicationDbContext.ShoppingListItem.Add(new ShoppingListItem()
                        {
                            Id = Guid.NewGuid(),
                            Amount = newIngredientGrams + oldIngredientGrams,
                            MeasurementId = 3,
                            IngredientId = ingredient.IngredientId,
                            ShoppingListId = shoppingList.Id
                        });
                    }
                } else
                {
                    _applicationDbContext.ShoppingListItem.Add(new ShoppingListItem()
                    {
                        Id = Guid.NewGuid(),
                        Amount = ingredient.Amount,
                        MeasurementId = ingredient.MeasurementId,
                        IngredientId = ingredient.IngredientId,
                        ShoppingListId = shoppingList.Id
                    });
                }
                
            }
            await _applicationDbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Succesfully added shopping list item for user {UserId}", request.UserId);

            return _mapper.Map<ShoppingListRecipeDto>(shoppingListRecipe.Entity);
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
