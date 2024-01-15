using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.Common.Models
{
    public class ElasticModelResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Score { get; set; }
    }
}
