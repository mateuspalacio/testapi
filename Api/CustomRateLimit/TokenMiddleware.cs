using Api.Data;
using Api.Exceptions;
using Api.Models;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;
using System.Text;

namespace Api.CustomRateLimit
{
    public class TokenMiddleware
    {
        private readonly RequestDelegate _next;
        public TokenMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext, IRepository tokenCheck)
        {
            var accessToken = await httpContext.GetTokenAsync("access_token") ?? httpContext.Request.Headers["Authorization"];
            
            var request = httpContext.Request;
            request.EnableBuffering();
            var buffer = new byte[Convert.ToInt32(request.ContentLength)];
            await request.Body.ReadAsync(buffer, 0, buffer.Length);
            var req_txt = Encoding.UTF8.GetString(buffer);

            request.Body.Position = 0;
            var body = JsonConvert.DeserializeObject<Req>(req_txt);
            if (body is null)
                throw new UserNotFoundException();

            var isValid = await tokenCheck.IsTokenValidAsync(accessToken);
            var isValidUser = await tokenCheck.IsUserValidAsync(body.username);
            if (!isValidUser)
            {
                if(isValid)
                    await tokenCheck.InvalidateTokenAsync(accessToken);
                throw new BlockedUserException();
            }
            if (!isValid)
            {
                if (isValidUser)
                {
                    await tokenCheck.InvalidateUserAsync(new User()
                    {
                        Email = body.email,
                        Username = body.username,
                        RequestLimit = 6,
                        IsValid = false
                    });
                }
                throw new InvalidTokenException();
            }

            await _next(httpContext);
        }
    }
}
