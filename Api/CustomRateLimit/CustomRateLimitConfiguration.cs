using AspNetCoreRateLimit;
using Microsoft.Extensions.Options;

namespace Api.CustomRateLimit
{
    public class CustomRateLimitConfiguration : RateLimitConfiguration
    {
        public CustomRateLimitConfiguration(IOptions<IpRateLimitOptions> ipOptions, IOptions<ClientRateLimitOptions> clientOptions) : base(ipOptions, clientOptions)
        {
        }
        public override void RegisterResolvers()
        {
            base.RegisterResolvers();
            ClientResolvers.Add(new QueryStringClientIdResolveContributor());
        }
    }
    public class QueryStringClientIdResolveContributor : IClientResolveContributor
    {
        public Task<string> ResolveClientAsync(HttpContext httpContext)
        {
            Console.WriteLine(httpContext.Request.Query["Username"]);
            return Task.FromResult<string>(httpContext.Request.Query["APIKey"]);
        }
    }
}
