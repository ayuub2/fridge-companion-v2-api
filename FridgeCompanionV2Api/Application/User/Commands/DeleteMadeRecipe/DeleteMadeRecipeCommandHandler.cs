using AutoMapper;
using FridgeCompanionV2Api.Application.Common.Exceptions;
using FridgeCompanionV2Api.Application.Common.Interfaces;
using FridgeCompanionV2Api.Application.Common.Models;
using FridgeCompanionV2Api.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.User.Commands.DeleteMadeRecipe
{
    public class DeleteMadeRecipeCommandHandler : IRequestHandler<DeleteMadeRecipeCommand, UserDto>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public DeleteMadeRecipeCommandHandler(IApplicationDbContext applicationDbContext, IMapper mapper, ILogger<DeleteMadeRecipeCommandHandler> logger)
        {
            _applicationDbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task<UserDto> Handle(DeleteMadeRecipeCommand request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var user = _applicationDbContext.Users.FirstOrDefault(x => x.Id == request.UserId);

            if (user is null)
            {
                _logger.LogError($"User needs to create a user profile first. {request.UserId}");
                throw new NotFoundException("User not found");
            }

            var userMadeRecipe = user.UserMadeRecipes.FirstOrDefault(x => x.RecipeId == request.RecipeId);

            if (userMadeRecipe is null)
            {
                _logger.LogError($"User attempted to delete a made recipe that they didnt make. {request.UserId} - {request.RecipeId}");
                throw new NotFoundException("Made recipe not found");
            }
            try
            {
                var userMadeRecipeSet = _applicationDbContext.Instance.Set<UserMadeRecipes>();
                userMadeRecipeSet.Remove(userMadeRecipe);
                await _applicationDbContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception exc)
            {
                _logger.LogError($"Unable to delete user made recipe {request.UserId} - {request.RecipeId}");
                throw exc;
            }


            return _mapper.Map<UserDto>(user);
        }
    }
}
