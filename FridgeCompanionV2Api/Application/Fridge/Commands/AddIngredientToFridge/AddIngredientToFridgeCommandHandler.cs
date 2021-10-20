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
                var entity = _applicationDbContext.FridgeItems.Add(item);

                await _applicationDbContext.SaveChangesAsync(cancellationToken);
                var attachedItem = _applicationDbContext.FridgeItems.Include(x => x.Ingredient).Include(x => x.Measurement).Include(x => x.IngredientLocation).FirstOrDefault(x => x.Id == entity.Entity.Id);


                return _mapper.Map<FridgeItemDto>(attachedItem);
            }
            catch (Exception exc) 
            {
                _logger.LogError($"Unable to add new fridge item for user - {request.UserId}", exc);
                throw;
            }
        }
    }
}
