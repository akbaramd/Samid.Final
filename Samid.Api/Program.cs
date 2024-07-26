using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Samid.Application.Interfaces;
using Samid.Domain.Interface;
using Samid.Inrastructure.Persistence;
using Samid.Inrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
  options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<User, IdentityRole<Guid>>()
  .AddEntityFrameworkStores<ApplicationDbContext>()
  .AddDefaultTokenProviders();
var signingKey = builder.Configuration["Jwt:Key"] ?? "M5T8Qr8LsPuzhPiXE5lOAnPZ7WGrPyXPrNTpLVZ7ysQ=";
builder.Services.AddAuthenticationJwtBearer(s => s.SigningKey =signingKey) //add this
  .AddAuthorization().AddFastEndpoints();
builder.Services.SwaggerDocument(o =>
{
  o.DocumentSettings = s =>
  {
    s.Title = "Your API";
    s.Version = "v1";
    s.Description = "API documentation";
    s.EnableJWTBearerAuth();
  };
});
  

builder.Services.AddScoped(typeof(IRepository<>),typeof(Repository<>));
builder.Services.AddScoped<IAuthService,AuthService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();
  app.UseSwaggerGen();
  app.UseSwaggerUi(s =>
  {
    s.ConfigureDefaults();
  });
}
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseFastEndpoints();

var summaries = new[]
{
  "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
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
