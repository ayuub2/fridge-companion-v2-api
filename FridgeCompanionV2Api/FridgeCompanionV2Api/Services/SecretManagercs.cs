using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Services
{
    public class SecretManager : ISecretManager
    {
        public string Get(string secretName)
        {
            var config = new AmazonSecretsManagerConfig { RegionEndpoint = RegionEndpoint.EUWest1 };
            var client = new AmazonSecretsManagerClient(config);

            var request = new GetSecretValueRequest
            {
                SecretId = secretName
            };

            GetSecretValueResponse response = null;
            try
            {
                response = Task.Run(async () => await client.GetSecretValueAsync(request)).Result;
            }
            catch (ResourceNotFoundException)
            {
                Console.WriteLine("The requested secret " + secretName + " was not found");
            }
            catch (InvalidRequestException e)
            {
                Console.WriteLine("The request was invalid due to: " + e.Message);
            }
            catch (InvalidParameterException e)
            {
                Console.WriteLine("The request had invalid params: " + e.Message);
            }

            return response?.SecretString;
        }
    }
}
