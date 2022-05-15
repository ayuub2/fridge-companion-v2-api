using FridgeCompanionV2Api.Application.Common.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.Common.HttpClients
{
    public class BarcodeClient : IBarcodeClient
    {
        private readonly HttpClient _client;

        public BarcodeClient(HttpClient client)
        {
            _client = client;
        }

        public async Task<string> GetItemName(string ean)
        {
            try
            {
                var response = await _client.GetAsync($"api/v2/product/{ean}");
                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var model = JsonConvert.DeserializeObject<BarcodeApiResponse>(responseString);
                    return model.product.product_name;
                }
            }
            catch
            {
            }
            return string.Empty;
        }
    }
}
