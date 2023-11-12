using AutoMapper;
using FridgeCompanionV2Api.Application.Common.Interfaces;
using FridgeCompanionV2Api.Application.Common.Models;
using FridgeCompanionV2Api.Application.User.Commands.AddFavouriteRecipe;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.User.Commands.AddDeviceToken
{
    public class AddDeviceTokenCommandHandler : IRequestHandler<AddDeviceTokenCommand, UserDto>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public AddDeviceTokenCommandHandler(IApplicationDbContext applicationDbContext, IMapper mapper, ILogger<AddDeviceTokenCommandHandler> logger)
        {
            _applicationDbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<UserDto> Handle(AddDeviceTokenCommand request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            _logger.LogInformation("Updating device token for user {userId} to {deviceToken}", request.UserId, request.DeviceToken);
            
            var user = _applicationDbContext.Users.Include(x => x.UserDiets).Include(x => x.UserMadeRecipes).Include(x => x.UserFavouriteRecipes).FirstOrDefault(x => x.Id == request.UserId);
            user.DeviceToken = request.DeviceToken;
            await _applicationDbContext.SaveChangesAsync(cancellationToken);
           
            _logger.LogInformation("Successfully updated device token for user {userId} to {deviceToken}", request.UserId, request.DeviceToken);
            
            return _mapper.Map<UserDto>(user);
        }
    }
}
