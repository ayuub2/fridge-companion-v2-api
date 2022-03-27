using AutoMapper;
using FridgeCompanionV2Api.Application.Common.Exceptions;
using FridgeCompanionV2Api.Application.Common.Interfaces;
using FridgeCompanionV2Api.Application.Common.Models;
using FridgeCompanionV2Api.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.User.Commands.DeleteFavouriteRecipe
{
    public class DeleteFavouriteRecipeCommandHandler : IRequestHandler<DeleteFavouriteRecipeCommand, UserDto>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public DeleteFavouriteRecipeCommandHandler(IApplicationDbContext applicationDbContext, IMapper mapper, ILogger<DeleteFavouriteRecipeCommandHandler> logger)
        {
            _applicationDbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<UserDto> Handle(DeleteFavouriteRecipeCommand request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var user = _applicationDbContext.Users.Include(x => x.UserFavouriteRecipes).FirstOrDefault(x => x.Id == request.UserId);

            if (user is null)
            {
                _logger.LogError($"User needs to create a user profile first. {request.UserId}");
                throw new NotFoundException("User not found");
            }

            var favouriteRecipe = user.UserFavouriteRecipes.FirstOrDefault(x => x.RecipeId == request.RecipeId);

            if (favouriteRecipe is null)
            {
                _logger.LogError($"User attempted to delete a favourite recipe that they didnt favourite. {request.UserId} - {request.RecipeId}");
                throw new NotFoundException("Favourite recipe not found");
            }
            try
            {
                var favouriteRecipesSet = _applicationDbContext.Instance.Set<UserFavouriteRecipes>();
                favouriteRecipesSet.Remove(favouriteRecipe);
                await _applicationDbContext.SaveChangesAsync(cancellationToken);
            } catch (Exception exc) 
            {
                _logger.LogError($"Unable to delete user favourite recipe {request.UserId} - {request.RecipeId}");
                throw exc; 
            }
            

            return _mapper.Map<UserDto>(user);
        }
    }
}
