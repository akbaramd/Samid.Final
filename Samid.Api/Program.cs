using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Samid.Api;
using Samid.Api.Middlewares;
using Samid.Domain.Entities;
using Samid.Infrastructure.Mappings;
using Samid.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Configure services
ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

// Configure middleware
ConfigureMiddleware(app);

// Run database migrations and seed data
ApplyMigrationsAndSeedData(app);

app.Run();

void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    // Database context configuration
    services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

    // Identity configuration
    services.AddIdentityCore<User>()
        .AddRoles<IdentityRole<Guid>>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

    // Authentication and Authorization
    var signingKey = configuration["Jwt:Key"] ?? ApplicationConstants.DefaultJwtSigningKey;
    services.AddAuthenticationJwtBearer(s => s.SigningKey = signingKey);
    services.AddAuthorization();

    // FastEndpoints and AutoMapper
    services.AddFastEndpoints();
    services.AddAutoMapper(typeof(MappingProfile));

    // Swagger/OpenAPI documentation
    services.SwaggerDocument(o =>
    {
        o.DocumentSettings = s =>
        {
            s.Title = "Samid API";
            s.Version = "v1";
            s.Description = "Samid is a study platform designed for tracking study activities and progress.";
            s.EnableJWTBearerAuth();
        };
    });
}

void ConfigureMiddleware(WebApplication app)
{
    app.UseMiddleware<ExceptionHandlingMiddleware>();
    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseFastEndpoints();
    app.UseSwaggerGen();
}

void ApplyMigrationsAndSeedData(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();

        var userManager = services.GetRequiredService<UserManager<User>>();
        ApplicationDbContextSeed.Seed(context, userManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
        throw;
    }
}
