using Safemeet.Services;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using dotenv.net;

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
//google authentication
builder.Services.Authentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDEfaults.AuthenticationScheme;
    options.DefaultChallangeScheme = "Google";
}).AddGoogle(
{
    options.ClientId = builder.configuration["Authentication:Google:ClientId"];
    options.ClientSecret = builder.configuration["Authentication:Google:ClientSecret"];
    options.SignInScheme = "Cookies";
}).AddCookie("Cookies").AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters {
        ValidateIssuer = false,
        ValidateAudiance = false,
        ValidateLifetime = true,
        ValidateIssuerSigninKey = true,
        IssuerSigninKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("${JWT_SECRET}"))
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

app.MapControllers();


var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

// app.MapGet("/", () => "Welcome to SafeMeet API! Use /weatherforecast to get weather data.");

app.MapGet("/", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
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
