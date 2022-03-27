using AutoMapper;
using FridgeCompanionV2Api.Application.Common.Exceptions;
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

namespace FridgeCompanionV2Api.Application.Fridge.Commands.UpdateFridgeItem
{
    public class UpdateFridgeItemCommandHandler : IRequestHandler<UpdateFridgeItemCommand, FridgeItemDto>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public UpdateFridgeItemCommandHandler(IApplicationDbContext applicationDbContext, IMapper mapper, ILogger<UpdateFridgeItemCommandHandler> logger)
        {
            _applicationDbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task<FridgeItemDto> Handle(UpdateFridgeItemCommand request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var entity = _applicationDbContext.FridgeItems.Include(x => x.Ingredient).Include(x => x.Measurement).Include(x => x.IngredientLocation).FirstOrDefault(x => x.Id == request.Id);

            if (entity is null)
            {
                _logger.LogError($"User: {request.UserId} tried to acess fridge item {request.Id} which does not exist.");
                throw new NotFoundException();
            }

            if (entity.UserId != request.UserId) 
            {
                _logger.LogError($"User: {request.UserId} tried to update fridge item {request.Id} which it is not authorised to do so.");
                throw new UnauthorizedAccessException("Accessed unathorised fridge item");
            }

            if (!_applicationDbContext.IngredientMeasurements.Any(x => x.Ingredient.Id == request.IngredientId && x.Measurement.Id == request.MeasurementId)) 
            {
                _logger.LogError($"User: {request.UserId} tried to update fridge item {request.Id} using wrong measurement type.");
                throw new Exception();
            }

            entity.IngredientId = request.IngredientId;
            entity.IngredientLocationId = request.IngredientLocationId;
            entity.MeasurementId = request.MeasurementId;
            entity.IsDeleted = request.IsDeleted;
            entity.Expiration = request.Expiration;
            entity.Amount = request.Amount;
            var updatedEntity = _applicationDbContext.FridgeItems.Update(entity);
            await _applicationDbContext.SaveChangesAsync(cancellationToken);

            return _mapper.Map<FridgeItemDto>(updatedEntity.Entity);
        }
    }
}
