using AutoMapper;
using FridgeCompanionV2Api.Application.Common.Interfaces;
using FridgeCompanionV2Api.Application.Common.Models;
using FridgeCompanionV2Api.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.Fridge.Commands.AddIngredientToFridge
{
    public class AddIngredientToFridgeCommandHandler : IRequestHandler<AddIngredientToFridgeCommand, FridgeItemDto>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public AddIngredientToFridgeCommandHandler(IApplicationDbContext applicationDbContext, IMapper mapper, ILogger<AddIngredientToFridgeCommandHandler> logger)
        {
            _applicationDbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<FridgeItemDto> Handle(AddIngredientToFridgeCommand request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            try
            {
                if (!_applicationDbContext.IngredientMeasurements.Include(x => x.Measurement).Any(x => x.Ingredient.Id == request.IngredientId && x.Measurement.Id == request.MeasurementId)) 
                {
                    throw new Exception();
                }
                var item = new FridgeItem()
                {
                    IngredientId = request.IngredientId,
                    IngredientLocationId = request.LocationId,
                    MeasurementId = request.MeasurementId,
                    Amount = request.Amount,
                    Expiration = request.ExpirationDate,
                    UserId = request.UserId,
                    IsDeleted = false
                };
                var itemInFridgeAlready = _applicationDbContext.FridgeItems.Include(x => x.Measurement).FirstOrDefault(x => x.IngredientId == request.IngredientId && !x.IsDeleted && x.UserId == request.UserId && DateTime.Now < x.Expiration);
                if (itemInFridgeAlready is not null)
                {
                    if (itemInFridgeAlready.MeasurementId == request.MeasurementId)
                    {
                        itemInFridgeAlready.Amount = itemInFridgeAlready.Amount + request.Amount;
                        itemInFridgeAlready.IngredientLocationId = request.LocationId;
                        itemInFridgeAlready.Expiration = request.ExpirationDate;
                    }
                    else
                    {
                        var fridgeIngredientConverter = _applicationDbContext.IngredientMeasurements.FirstOrDefault(x => x.IngredientId == itemInFridgeAlready.IngredientId && x.MeasurementId == itemInFridgeAlready.Measurement.Id);
                        var newFridgeItemConverter = _applicationDbContext.IngredientMeasurements.Include(x => x.Measurement).FirstOrDefault(x => x.IngredientId == request.IngredientId && x.MeasurementId == request.MeasurementId);
                        decimal fridgeIngredientAmountGrams = ConvertMeasurementToGrams(fridgeIngredientConverter, itemInFridgeAlready.Amount, itemInFridgeAlready.Measurement);
                        decimal newFridgeAmountGrams = ConvertMeasurementToGrams(newFridgeItemConverter, request.Amount, newFridgeItemConverter.Measurement);
                        itemInFridgeAlready.Amount = newFridgeItemConverter.AverageGrams is null ? (decimal)fridgeIngredientAmountGrams + newFridgeAmountGrams : (decimal)((fridgeIngredientAmountGrams + newFridgeAmountGrams) / newFridgeItemConverter.AverageGrams);
                        itemInFridgeAlready.MeasurementId = request.MeasurementId;
                    }
                    await _applicationDbContext.SaveChangesAsync(cancellationToken);
                    var updatedItem = _applicationDbContext.FreshFridgeItems(request.UserId).FirstOrDefault(x => request.IngredientId == x.IngredientId);
                    return _mapper.Map<FridgeItemDto>(updatedItem);
                }
                else 
                {
                    var entity = _applicationDbContext.FridgeItems.Add(item);

                    await _applicationDbContext.SaveChangesAsync(cancellationToken);
                    var attachedItem = _applicationDbContext.FridgeItems.Include(x => x.Ingredient).Include(x => x.Measurement).Include(x => x.IngredientLocation).FirstOrDefault(x => x.Id == entity.Entity.Id);


                    return _mapper.Map<FridgeItemDto>(attachedItem);
                }
               
            }
            catch (Exception exc) 
            {
                _logger.LogError($"Unable to add new fridge item for user - {request.UserId}", exc);
                throw;
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
