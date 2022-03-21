using AutoMapper;
using FridgeCompanionV2Api.Application.Common.Exceptions;
using FridgeCompanionV2Api.Application.Common.Interfaces;
using FridgeCompanionV2Api.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.User.Queries.GetUserProfile
{
    public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, UserDto>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public GetUserProfileQueryHandler(IApplicationDbContext applicationDbContext, IMapper mapper, ILogger<GetUserProfileQueryHandler> logger)
        {
            _applicationDbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<UserDto> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var user = _applicationDbContext.Users.Include(x => x.UserFavouriteRecipes).Include(x => x.UserMadeRecipes).Include(x => x.UserDiets).FirstOrDefault(x => x.Id == request.UserId);
            if(user is null) 
            {
                _logger.LogError($"Attempted to get user profile for non existent user - {request.UserId}");
                throw new NotFoundException("User not found, please create user profile first.");
            }
            return _mapper.Map<UserDto>(user);
        }
    }
}
