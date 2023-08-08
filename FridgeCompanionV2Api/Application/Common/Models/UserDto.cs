using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.Common.Models
{
    public class UserDto
    {
        public string Id { get; set; }
        public List<UserDietsDto> UserDiets { get; set; }
        public List<UserFavouriteRecipesDto> UserFavouriteRecipes { get; set; }
        public List<UserMadeRecipesDto> UserMadeRecipes { get; set; }
    }
}
