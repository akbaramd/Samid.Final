Samid.Domain:

    Entities
    ValueObjects
    Interfaces
    Domain Events
    Domain Exceptions

Samid.Application:

    Interfaces
    Services
    DTOs
    Exceptions
    UseCases

Samid.Infrastructure:

    Persistence (Repositories, DbContext)
    ExternalServices (Integrations, External APIs)
    Mappings (Entity Framework Mappings, AutoMapper Profiles)
    Configurations (Configuration files and settings)

Samid.Api:

    Controllers
    Models (API Models, ViewModels)
    Middlewares
    Configurations (Dependency Injection, API Settings)



dotnet ef migrations add InitialCreate --project Samid.Infrastructure --startup-project Samid.Api --context ApplicationDbContext

dotnet ef database update --project Samid.Infrastructure --startup-project Samid.Api --context ApplicationDbContext
