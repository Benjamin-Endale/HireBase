using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Routing;
using System.Text;
using HireBase_Backend.Data;
using HireBase_Backend.Services;
//using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ===== CORS (single registration) =====
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins("http://localhost:3000", "http://localhost:3001") // your frontend
              .AllowAnyHeader()
              .AllowAnyMethod()
    // .AllowCredentials() // uncomment only if you actually use cookies
    );
});

// ===== Controllers =====
builder.Services.AddControllers();

// ===== Swagger/OpenAPI =====
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen(c =>
//{
//    c.SwaggerDoc("v1", new()
//    {
//        Title = "HireBase API",
//        Version = "v1",
//        Description = "API for HireBase Applicant Management"
//    });
//});


// ===== Email Service =====
builder.Services.AddSingleton<EmailService>();

// ===== DbContext =====
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ===== JWT Authentication =====
var jwtKey = builder.Configuration["JwtSettings:Key"];
if (string.IsNullOrEmpty(jwtKey))
    throw new Exception("JWT Key not found in configuration.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),

        // Claims mapping
        RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
        NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"
    };
});

// ===== Scoped Services =====
builder.Services.AddScoped<IPasswordHasher, Pbkdf2PasswordHasher>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

builder.Services.AddAuthorization();

var app = builder.Build();

//// ===== Development Middleware =====
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI(c =>
//    {
//        c.SwaggerEndpoint("/swagger/v1/swagger.json", "HireBase API V1");
//        c.RoutePrefix = string.Empty; // Swagger at root
//    });
//}

// ===== Pipeline =====
app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

//// Custom middleware
//app.UseMiddleware<HRMS.Backend.Middleware.OrganizationMiddleware>();

// Map controllers
app.MapControllers();

// Optional: debug route listing in development
#if DEBUG
app.MapGet("/_routes", (IEnumerable<EndpointDataSource> sources) =>
{
    var list = new List<object>();

    foreach (var src in sources)
    {
        foreach (var ep in src.Endpoints.OfType<RouteEndpoint>())
        {
            var httpMeta = ep.Metadata.OfType<IHttpMethodMetadata>().FirstOrDefault();
            var methods = httpMeta?.HttpMethods ?? Array.Empty<string>();

            list.Add(new
            {
                Template = ep.RoutePattern.RawText,
                Methods = string.Join(",", methods)
            });
        }
    }

    list = list.OrderBy(x => x.GetType().GetProperty("Template")!.GetValue(x)).ToList();
    return Results.Json(list);
});
#endif

// Sample endpoint
var summaries = new[]
{
    "Freezing","Bracing","Chilly","Cool","Mild","Warm","Balmy","Hot","Sweltering","Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast(
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

// Run the app
app.Run();

// ===== Record for sample endpoint =====
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
