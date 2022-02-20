// See https://aka.ms/new-console-template for more information
// discover endpoints from metadata
using IdentityModel.Client;
using Newtonsoft.Json.Linq;

var client = new HttpClient();
var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5001");
Console.WriteLine(disco.AuthorizeEndpoint);
Console.WriteLine(disco.TokenEndpoint);
if (disco.IsError)
{
    Console.WriteLine(disco.Error);
    return;
}

// request token
var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
{
    Address = disco.TokenEndpoint,

    ClientId = "admin",
    ClientSecret = "admin",
    Scope = "Api"
});

if (tokenResponse.IsError)
{
    Console.WriteLine(tokenResponse.Error);
    return;
}

Console.WriteLine(tokenResponse.Json);

// call api
var apiClient = new HttpClient();
apiClient.SetBearerToken(tokenResponse.AccessToken);

var response = await apiClient.GetAsync("https://localhost:7075/Ancient");
if (!response.IsSuccessStatusCode)
{
    Console.WriteLine(response.StatusCode);
}
else
{
    var content = await response.Content.ReadAsStringAsync();
    Console.WriteLine(JObject.Parse(content));
}