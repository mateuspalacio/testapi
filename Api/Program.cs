using Api.CustomRateLimit;
using Api.Data;
using AspNetCoreRateLimit;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOptions();


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<TesteDbContext>(opt => opt.UseMySql(builder.Configuration.GetConnectionString("TesteContext"), ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("TesteContext"))));
builder.Services.AddDbContext<Api.Data.TokenContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("SqliteContext")));

builder.Services.AddMemoryCache();
builder.Services.AddControllers();

string tokenUrl = "http://localhost:5001";

//ATENÇÃO: O auth no swagger não esta funcionando. ERROR: failed to fetch.

// Registre o gerador Swagger, definindo 1 ou mais documentos Swagger
 builder.Services.AddSwaggerGen(c =>
{
    // Definindo uso de Client Credentials para o Swagger
    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            ClientCredentials = new OpenApiOAuthFlow
            {
                // TODO Receber infos do ambiente e ajustar portas 
                TokenUrl = new Uri($"{tokenUrl}/connect/token", UriKind.Absolute),
                Scopes = new Dictionary<string, string>
                            {
                                { "Api", "Ancients API"}
                            },
                AuthorizationUrl = new Uri($"{tokenUrl}/connect/authorize", UriKind.Absolute)
            }
        }
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
                        },
                        new[] { "Api" }
                    }
                });
});

builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
//builder.Services.Configure<IpRateLimitPolicies>(builder.Configuration.GetSection("IpRateLimitPolicies"));

builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
//builder.Services.AddDistributedRateLimiting<AsyncKeyLockProcessingStrategy>();
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, CustomRateLimitConfiguration>();
builder.Services.AddTransient<IRepository, Repository>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApiScope", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "Api");
    });
});

builder.Services.AddAuthentication("Bearer").AddJwtBearer("Bearer", options =>
{
    options.Authority = "https://localhost:5001";

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = false
    };
});

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseHttpsRedirection();
//app.UseIpRateLimiting();
app.UseMiddleware<TokenMiddleware>();

app.UseMiddleware<CustomIpRateLimitMiddleware>(Array.Empty<object>());
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers()
        .RequireAuthorization("ApiScope")
        ;
});


app.MapControllers();

app.Run();
