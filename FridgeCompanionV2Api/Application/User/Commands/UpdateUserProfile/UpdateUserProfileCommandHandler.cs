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

namespace FridgeCompanionV2Api.Application.User.Commands.UpdateUserProfile
{
    public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, UserDto>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public UpdateUserProfileCommandHandler(IApplicationDbContext applicationDbContext, IMapper mapper, ILogger<UpdateUserProfileCommandHandler> logger)
        {
            _applicationDbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<UserDto> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            var user = _applicationDbContext.Users.FirstOrDefault(x => x.Id == request.UserId);
            
            if(user is null)
            {
                _logger.LogError($"Attempted to update a profile for a user profile that doesnt exist, userId - {request.UserId}.");
                throw new NotFoundException("User not found");
            }

            var userDiets = _applicationDbContext.UserDiets.Where(x => x.User.Id == request.UserId);

            if (!userDiets.Any())
            {
                _logger.LogError($"Attempted to update userDiet that does not exist, userId - {request.UserId}.");
                throw new NotFoundException("User not found");
            }
            try
            {
                user.IsAllergicNuts = request.IsAllergicNuts;
                user.IsGlutenFree = request.IsGlutenFree;
                var diets = request.Diets.Select(x => x.Id).Select(x => _applicationDbContext.DietTypes.FirstOrDefault(d => d.Id == x)).ToList();
                _applicationDbContext.UserDiets.RemoveRange(userDiets);
                _applicationDbContext.UserDiets.AddRange(diets.Select(x => new UserDiets() { User = user, DietType = x }));
                await _applicationDbContext.SaveChangesAsync(cancellationToken);
                return _mapper.Map<UserDto>(user);
            }
            catch (Exception exc)
            {
                _logger.LogError($"Unable to update profile for user - {request.UserId}", exc);
                throw exc;
            }
        }
    }
}
