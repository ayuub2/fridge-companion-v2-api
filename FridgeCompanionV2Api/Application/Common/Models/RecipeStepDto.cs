using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.Common.Models
{
    public class RecipeStepDto
    {
        public int Id { get; set; }
        public string Step { get; set; }
        public int SortOrder { get; set; }
    }
}
