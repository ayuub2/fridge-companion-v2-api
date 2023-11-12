using FluentValidation;

namespace FridgeCompanionV2Api.Application.User.Commands.AddDeviceToken
{
    public class AddDeviceTokenCommandValidator : AbstractValidator<AddDeviceTokenCommand>
    {
        public AddDeviceTokenCommandValidator()
        {
            RuleFor(x => x.DeviceToken).NotEmpty().WithMessage("Device token must be supplied.");
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User is not authorized");
        }
    }
}
