using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Domain.Entities
{
    public class IngredientType
    {
        public int Id { get; set; }
        public virtual IngredientGroupType IngredientGroupType { get; set; }
        public virtual Ingredient Ingredient { get; set; }
    }
}
