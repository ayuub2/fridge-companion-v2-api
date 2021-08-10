using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Domain.Entities
{
    public class IngredientDiet
    {
        public int Id { get; set; }
        public virtual Ingredient Ingredient { get; set; }
        public virtual DietType Diet { get; set; }
    }
}
