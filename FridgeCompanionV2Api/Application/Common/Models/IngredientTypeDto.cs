using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.Common.Models
{
    public class IngredientTypeDto
    {
        public int Id { get; set; }
        public IngredientGroupTypeDto Type { get; set; }
    }
}
