using Api.Data;
using Api.Models;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;

namespace Api.CustomRateLimit
{
    public class CustomIpRateLimitMiddleware : RateLimitMiddleware<IpRateLimitProcessor>
    {
        private readonly ILogger<IpRateLimitMiddleware> _logger;
        public CustomIpRateLimitMiddleware(RequestDelegate next, IProcessingStrategy processingStrategy, IOptions<IpRateLimitOptions> options, IIpPolicyStore policyStore,
            IRateLimitConfiguration config, ILogger<IpRateLimitMiddleware> logger)
            : base(next, options?.Value, new IpRateLimitProcessor(options?.Value, policyStore, processingStrategy), config)
        {
            _logger = logger;
        }
        protected override async void LogBlockedRequest(HttpContext httpContext, ClientRequestIdentity identity, RateLimitCounter counter, RateLimitRule rule)
        {
            ILogger<IpRateLimitMiddleware> logger = _logger;
            //DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(108, 9);
            //defaultInterpolatedStringHandler.AppendLiteral("Request ");
            //defaultInterpolatedStringHandler.AppendFormatted(identity.HttpVerb);
            //defaultInterpolatedStringHandler.AppendLiteral(":");
            //defaultInterpolatedStringHandler.AppendFormatted(identity.Path);
            //defaultInterpolatedStringHandler.AppendLiteral(" from IP ");
            //defaultInterpolatedStringHandler.AppendFormatted(identity.ClientIp);
            //defaultInterpolatedStringHandler.AppendLiteral(" has been blocked, quota ");
            //defaultInterpolatedStringHandler.AppendFormatted(rule.Limit);
            //defaultInterpolatedStringHandler.AppendLiteral("/");
            //defaultInterpolatedStringHandler.AppendFormatted(rule.Period);
            //defaultInterpolatedStringHandler.AppendLiteral(" exceeded by ");
            //defaultInterpolatedStringHandler.AppendFormatted(counter.Count - rule.Limit);
            //defaultInterpolatedStringHandler.AppendLiteral(". Blocked by rule ");
            //defaultInterpolatedStringHandler.AppendFormatted(rule.Endpoint);
            //defaultInterpolatedStringHandler.AppendLiteral(", TraceIdentifier ");
            //defaultInterpolatedStringHandler.AppendFormatted(httpContext.TraceIdentifier);
            //defaultInterpolatedStringHandler.AppendLiteral(". MonitorMode: ");
            //defaultInterpolatedStringHandler.AppendFormatted(rule.MonitorMode);
            //defaultInterpolatedStringHandler.AppendLiteral(" Manipulated by the devs ");
            //defaultInterpolatedStringHandler.AppendFormatted(httpContext.Request.Query["Username"]);
            //logger.LogInformation(defaultInterpolatedStringHandler.ToStringAndClear());
            await BlockUser(httpContext);
            
        }
        private async Task BlockUser(HttpContext httpContext)
        {
            using (var db = httpContext.RequestServices.GetService<IRepository>())
            {
                var accessToken = await httpContext.GetTokenAsync("access_token") ?? httpContext.Request.Headers["Authorization"];
                await db.InvalidateTokenAsync(accessToken);
            }
        }
    }
}
