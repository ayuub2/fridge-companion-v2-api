using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Domain.Entities
{
    public class User
    {
        public string Id { get; set; }
        public virtual List<UserDiets> UserDiets { get; set; }
        public virtual List<UserFavouriteRecipes> UserFavouriteRecipes { get; set; }
        public virtual List<UserMadeRecipes> UserMadeRecipes { get; set; }
    }
}
