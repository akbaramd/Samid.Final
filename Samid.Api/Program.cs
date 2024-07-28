using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Samid.Infrastructure.Persistence;
using Samid.Inrastructure.Mappings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
  options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentityCore<User>()
  .AddRoles<IdentityRole<Guid>>()
  .AddEntityFrameworkStores<ApplicationDbContext>()
  .AddDefaultTokenProviders();

var signingKey = builder.Configuration["Jwt:Key"] ?? "M5T8Qr8LsPuzhPiXE5lOAnPZ7WGrPyXPrNTpLVZ7ysQ=";
builder.Services
  .AddAuthenticationJwtBearer(s => s.SigningKey = signingKey) //add this
  .AddAuthorization()
  .AddFastEndpoints();

builder.Services.AddAutoMapper(typeof(MappingProfile));
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

app.UseMiddleware<ExceptionHandlingMiddleware>();


app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseFastEndpoints();

using (var scope = app.Services.CreateScope())
{
  var services = scope.ServiceProvider;
  var context = services.GetRequiredService<ApplicationDbContext>();
  var user = services.GetRequiredService<UserManager<User>>();
  ApplicationDbContextSeed.Seed(context,user);
}

app.Run();
