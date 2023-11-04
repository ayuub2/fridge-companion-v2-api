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

namespace FridgeCompanionV2Api.Application.User.Commands.AddMadeRecipe
{
    public class AddUserMadeRecipeCommandHandler : IRequestHandler<AddUserMadeRecipeCommand, UserDto>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public AddUserMadeRecipeCommandHandler(IApplicationDbContext applicationDbContext, IMapper mapper, ILogger<AddUserMadeRecipeCommandHandler> logger)
        {
            _applicationDbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<UserDto> Handle(AddUserMadeRecipeCommand request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var recipe = _applicationDbContext.Recipes.FirstOrDefault(x => x.Id == request.RecipeId && !x.IsDeleted);

            if (recipe is null)
            {
                _logger.LogError($"User attempted to add a recipe made that does not exist. {request.UserId} - {request.RecipeId}");
                throw new NotFoundException("Recipe not found");
            }

            var user = _applicationDbContext.Users.Include(x => x.UserMadeRecipes).FirstOrDefault(x => x.Id == request.UserId);

            if (user is null)
            {
                _logger.LogError($"Unknown user attempted to add a previously made recipe. {request.UserId}");
                throw new NotFoundException("User profile not found, please create user profile first.");
            }

            user.UserMadeRecipes.Add(new UserMadeRecipes()
            {
                RecipeId = recipe.Id,
                UserId = user.Id,
                CreatedDate = DateTime.UtcNow
            });

            await _applicationDbContext.SaveChangesAsync(cancellationToken);

            return _mapper.Map<UserDto>(user);
        }
    }
}
