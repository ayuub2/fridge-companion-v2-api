using FridgeCompanionV2Api.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.User.Commands.AddFavouriteRecipe
{
    public class AddFavouriteRecipeCommand : IRequest<UserDto>
    {
        public string UserId { get; set; }
        public int RecipeId { get; set; }
    }
}
