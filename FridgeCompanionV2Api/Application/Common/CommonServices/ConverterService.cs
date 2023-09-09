using FridgeCompanionV2Api.Application.Common.Exceptions;
using FridgeCompanionV2Api.Application.Common.Interfaces;
using FridgeCompanionV2Api.Application.Common.Models;
using FridgeCompanionV2Api.Domain.Entities;
using System;
using System.Linq;

namespace FridgeCompanionV2Api.Application.Common.CommonServices
{
    public class ConverterService : IConverterService
    {
        private readonly IApplicationDbContext _applicationDbContext;

        public ConverterService(IApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext ?? throw new System.ArgumentNullException(nameof(applicationDbContext));
        }

        public decimal ConvertIngredientAmountToGrams(RecipeIngredientDto ingredient)
        {
            var amount = ingredient.Amount;
            var measurement = ingredient.Measurement;
            var measurementType = _applicationDbContext.IngredientMeasurements.FirstOrDefault(x => x.MeasurementId == ingredient.Measurement.Id &&
                                                x.IngredientId == ingredient.Ingredient.Id);
            if (measurementType != null)
            {
                // Using the average grams of the measurement and the amount of that measurement we have, we can get the grams of that ingredient
                return measurementType.AverageGrams != null ? measurementType.AverageGrams.Value * ingredient.Amount : Convert.ToDecimal(ingredient.Amount);
            }
            throw new NotFoundException();
        }
    }
}
