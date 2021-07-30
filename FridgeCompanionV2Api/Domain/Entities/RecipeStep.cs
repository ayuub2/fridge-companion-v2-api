using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Domain.Entities
{
    public class RecipeStep
    {
        public int Id { get; set; }
        public string Step { get; set; }
        public int SortOrder { get; set; }
        public Recipe Recipe { get; set; }
    }
}
