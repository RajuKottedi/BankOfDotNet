using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using IdentityModel.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BankOfDotNet.ConsoleClient
{
    class Program
    {
        public static void Main(string[] args) => MainAsync().GetAwaiter().GetResult();

        private static async Task MainAsync()
        {
            var client = new HttpClient();
            // discover all the endpoints using metadata of identity server
            var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5000");
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }

            // Grab a bearer token
            //var tokenClient = new HttpClient();
            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "client",
                ClientSecret = "secret"
            });

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
            }

            Console.WriteLine(tokenResponse.Json);
            Console.WriteLine("\n\n");

            //Consume our Customer API
            //var custClient = new HttpClient();
            client.SetBearerToken(tokenResponse.AccessToken);

            var customerInfo = new StringContent(
                JsonConvert.SerializeObject(
                    new { id = 10, FirstName = "Manish", LastName = "Narayan"}),
                Encoding.UTF8, "application/json");

            var createCustomerResponse = await client.PostAsync("http://localhost:13600/api/customers", customerInfo);

            if (!createCustomerResponse.IsSuccessStatusCode)
            {
                Console.WriteLine(createCustomerResponse.StatusCode);
            }

            var getCustomerResponse = await client.GetAsync("http://localhost:13600/api/customers");
            if (!getCustomerResponse.IsSuccessStatusCode)
            {
                Console.WriteLine(getCustomerResponse.StatusCode);
            }
            else
            {
                var content = await getCustomerResponse.Content.ReadAsStringAsync();
                Console.WriteLine(JArray.Parse(content));
            }

            Console.Read();
        }
    }
}
