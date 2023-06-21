using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Domain.Entities
{
    public class SuggestionRecipe
    {
        public int Id { get; set; }
        public int RecipeId { get; set; }
        public int SuggestionId { get; set; }
        public virtual Suggestion Suggestion { get; set; }
    }
}
