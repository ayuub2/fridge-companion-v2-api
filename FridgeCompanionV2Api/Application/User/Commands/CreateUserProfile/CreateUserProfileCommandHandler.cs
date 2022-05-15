using AutoMapper;
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

namespace FridgeCompanionV2Api.Application.User.Commands.CreateUserProfile
{
    public class CreateUserProfileCommandHandler : IRequestHandler<CreateUserProfileCommand, UserDto>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public CreateUserProfileCommandHandler(IApplicationDbContext applicationDbContext, IMapper mapper, ILogger<CreateUserProfileCommandHandler> logger)
        {
            _applicationDbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<UserDto> Handle(CreateUserProfileCommand request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if(_applicationDbContext.Users.Any(x => x.Id == request.UserId)) 
            {
                _logger.LogError($"Attempted to create a second user profile for user {request.UserId}");
                throw new InvalidOperationException("Cannot create user profile more than once for the same user");
            }
            try 
            {
                var userEntity = _applicationDbContext.Users.Add(new Domain.Entities.User() 
                {
                    Id = request.UserId,
                    IsGlutenFree = request.IsGlutenFree,
                    IsAllergicNuts = request.IsAllergicNuts
                });
                await _applicationDbContext.SaveChangesAsync(cancellationToken);
                var diets = request.Diets.Select(x => x.Id).Distinct().Select(x => _applicationDbContext.DietTypes.FirstOrDefault(d => d.Id == x)).ToList();
                if(diets.Any()) 
                {
                    _applicationDbContext.UserDiets.AddRange(diets.Select(x => new UserDiets() { User = userEntity.Entity, DietType = x }));
                    await _applicationDbContext.SaveChangesAsync(cancellationToken);
                }
                return _mapper.Map<UserDto>(userEntity.Entity);
            }
            catch (Exception exc) 
            {
                _logger.LogError($"Unable to add profile for user - {request.UserId}", exc);
                throw exc;
            } 
        }
    }
}
