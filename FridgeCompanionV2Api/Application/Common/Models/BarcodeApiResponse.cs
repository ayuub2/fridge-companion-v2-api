using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.Common.Models
{
    public class BarcodeApiResponse
    {
        public string code { get; set; }
        public Product product { get; set; }
        public int status { get; set; }
        public string status_verbose { get; set; }
    }

    public class Product
    {
        public string product_name { get; set; }
    }
}
