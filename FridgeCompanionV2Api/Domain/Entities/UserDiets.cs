using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Domain.Entities
{
    public class UserDiets
    {
        public int Id { get; set; }
        public virtual User User { get; set; }
        public virtual DietType DietType { get; set; }
    }
}
