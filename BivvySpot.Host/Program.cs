using BivvySpot.Application.Extensions;
using BivvySpot.Data;
using BivvySpot.Data.Extensions;
using BivvySpot.Presentation.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("BivvySpot");

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BivvySpot API", Version = "v1" });

    // OAuth2 definition (Auth Code + PKCE)
    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri($"https://{builder.Configuration["Auth0:Domain"]}/authorize"),
                TokenUrl         = new Uri($"https://{builder.Configuration["Auth0:Domain"]}/oauth/token"),
                Scopes = new Dictionary<string, string>
                {
                    { "openid", "OpenID" },
                    { "profile", "User profile" },
                    { "email", "Email address" }
                }
            }
        }
    });

    // Require OAuth on all operations (you can narrow later)
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
            },
            new[] { "openid", "profile", "email" }
        }
    });
});

builder.Services.AddDbContext<BivvySpotContext>(o => 
    o.UseSqlServer(connectionString, sql => sql.UseNetTopologySuite()));

builder.Services
    .AddData(connectionString)
    .AddApplication(builder.Configuration)
    .AddPresentation();

// Auth0 JWT Bearer
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.Authority = $"https://{builder.Configuration["Auth0:Domain"]}";
        o.Audience  = builder.Configuration["Auth0:Audience"];

        // IMPORTANT: keep "sub" as "sub" instead of mapping to NameIdentifier
        o.MapInboundClaims = false;

        // Optional: nicer default for User.Identity.Name if you care
        // o.TokenValidationParameters = new TokenValidationParameters { NameClaimType = "name" };
    });

builder.Services.AddAuthorization(options =>
{
    // Example permission policy (Auth0 "permissions" claim)
    options.AddPolicy("posts:read", policy =>
        policy.RequireAssertion(ctx =>
            ctx.User.Claims.Any(c => c.Type == "permissions" && c.Value.Contains("posts:read"))));
});

var app = builder.Build();

using var scope = app.Services.CreateScope();

await using var context = scope.ServiceProvider.GetRequiredService<BivvySpotContext>();
await context.Database.MigrateAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "BivvySpot API v1");
        c.OAuthClientId(builder.Configuration["Auth0:SwaggerClientId"]);
        c.OAuthUsePkce();
        c.OAuthScopes("openid", "profile", "email");
        // Tell Auth0 which API audience we want in the access token:
        c.OAuthAdditionalQueryStringParams(new Dictionary<string, string>
        {
            ["audience"] = builder.Configuration["Auth0:Audience"]!
        });
    });
}

app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();

app.MapControllers();

app.Run();