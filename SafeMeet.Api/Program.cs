using SafeMeet.Api.Services;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using dotenv.net;
using Microsoft.AspNetCore.Authentication;

//For Authentication
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

// Load environment variables from .env file
DotEnv.Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSingleton<MongoDbService>(); //database service
builder.Services.AddSingleton<UserService>(); //user service registration
builder.Services.AddSingleton<AvailabilityService>(); //availability service registration

//google authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = "Google";
})
.AddGoogle(options =>
{
    var clientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID") ?? builder.Configuration["Authentication:Google:ClientId"];
    var clientSecret = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET") ?? builder.Configuration["Authentication:Google:ClientSecret"];

    if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
    {
        throw new InvalidOperationException("Google OAuth credentials not configured properly");
    }

    options.ClientId = clientId;
    options.ClientSecret = clientSecret;
    options.SignInScheme = "Cookies";
    options.CallbackPath = "/signin-google";
    options.SaveTokens = true;
})
.AddCookie("Cookies", options =>
{
    options.LoginPath = "/api/auth/login";
    options.LogoutPath = "/api/auth/logout";
})
.AddJwtBearer(options =>
{
    var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? builder.Configuration["Authentication:Google:JwtSecret"];
    if (string.IsNullOrEmpty(jwtSecret))
    {
        throw new InvalidOperationException("JWT secret not configured");
    }
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Add a route to handle the Google OAuth callback and redirect to our controller
app.MapGet("/signin-google", async (HttpContext context) =>
{
    // This will trigger the cookie authentication scheme to process the Google auth
    var result = await context.AuthenticateAsync("Cookies");
    if (result.Succeeded)
    {
        // Redirect to our callback endpoint for JWT generation
        return Results.Redirect("/api/auth/callback");
    }
    return Results.BadRequest("Authentication failed");
});

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

// app.MapGet("/", () => "Welcome to SafeMeet API! Use /weatherforecast to get weather data.");

app.MapGet("/", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}