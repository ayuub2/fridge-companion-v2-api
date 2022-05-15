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

namespace FridgeCompanionV2Api.Application.User.Commands.AddFavouriteRecipe
{
    public class AddFavouriteRecipeCommandHandler : IRequestHandler<AddFavouriteRecipeCommand, UserDto>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public AddFavouriteRecipeCommandHandler(IApplicationDbContext applicationDbContext, IMapper mapper, ILogger<AddFavouriteRecipeCommandHandler> logger)
        {
            _applicationDbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<UserDto> Handle(AddFavouriteRecipeCommand request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var recipe = _applicationDbContext.Recipes.FirstOrDefault(x => x.Id == request.RecipeId && !x.IsDeleted);

            if(recipe is null)
            {
                _logger.LogError($"User attempted to add a recipe that does not exist. {request.UserId} - {request.RecipeId}");
                throw new NotFoundException("Recipe not found");
            }

            var user = _applicationDbContext.Users.Include(x => x.UserFavouriteRecipes).FirstOrDefault(x => x.Id == request.UserId);

            if (user is null) 
            {
                _logger.LogError($"Unknown user attempted to add a favourite recipe. {request.UserId}");
                throw new NotFoundException("User profile not found, please create user profile first.");
            }

            if(!user.UserFavouriteRecipes.Any(x => x.RecipeId == recipe.Id && x.UserId == user.Id)) 
            {
                user.UserFavouriteRecipes.Add(new UserFavouriteRecipes()
                {
                    RecipeId = recipe.Id,
                    UserId = user.Id
                });

                await _applicationDbContext.SaveChangesAsync(cancellationToken);
            }

            return _mapper.Map<UserDto>(user);
        }
    }
}
