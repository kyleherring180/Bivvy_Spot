using BivvySpot.Application.Extensions;
using BivvySpot.Data;
using BivvySpot.Data.Extensions;
using BivvySpot.Presentation.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("BivvySpot");

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
        // If you’ll use RBAC roles/permissions:
        o.TokenValidationParameters.RoleClaimType = "roles";
        // Auth0 puts fine-grained permissions in "permissions"
        // You can read those directly in policies if you like.
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
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();

app.MapControllers();

app.Run();