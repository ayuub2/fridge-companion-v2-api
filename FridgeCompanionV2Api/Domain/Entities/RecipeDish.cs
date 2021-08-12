using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Domain.Entities
{
    public class RecipeDish
    {
        public int Id { get; set; }
        public virtual Recipe Recipe { get; set; }
        public virtual DishType Dish { get; set; }
    }
}
