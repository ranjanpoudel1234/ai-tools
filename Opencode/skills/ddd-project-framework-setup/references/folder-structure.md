# DDD Clean Architecture - Folder Structure Reference

This document provides the complete folder structure template used by the framework setup skill.

## Complete Project Structure

```
{project-root}/
│
├── .editorconfig                                     # Code style configuration (EditorConfig)
├── .gitattributes                                    # Git attributes (line endings, etc.)
├── .gitignore                                        # Git ignore patterns
│
├── .github/                                          # GitHub configuration
│   └── copilot-instructions.md                      # GitHub Copilot instructions
│
├── .pipelines/                                       # Azure DevOps pipelines
│   ├── build-pipeline.yml                           # CI/CD configurations
│   └── deploy-pipeline.yml
│
├── .vscode/                                          # VS Code workspace settings
│   ├── settings.json                                # Editor settings
│   └── launch.json                                  # Debug configurations
│
├── docs/                                             # Documentation
│   ├── architecture.md                              # Architecture documentation
│   ├── adr/                                         # Architecture Decision Records
│   └── diagrams/                                    # Architecture diagrams
│
└── src/                                              # Source code
    │
    ├── {Org}.{ProjectName}.sln                      # Solution file
    ├── README.md                                    # Project documentation
    │
    ├── {Org}.{ProjectName}.Domain/                  # CORE DOMAIN LAYER
│   ├── {Org}.{ProjectName}.Domain.csproj            # No external dependencies
│   ├── InternalsVisibleTo.cs                        # Test project visibility
│   │
│   ├── Constants/                                    # Domain constants
│   │   └── Errors/                                   # Error messages/codes
│   │
│   ├── Entities/                                     # Rich domain entities
│   │   └── [YourEntity].cs                          # Aggregate roots, entities
│   │
│   ├── ValueObjects/                                 # Immutable value objects
│   │   └── [YourValueObject].cs                     # Address, Money, etc.
│   │
│   ├── Enums/                                        # Domain enumerations
│   │   └── [YourEnum].cs                            # Status types, etc.
│   │
│   ├── Interfaces/                                   # Domain interfaces
│   │   └── I[YourRepository].cs                     # Repository contracts
│   │
│   ├── Exceptions/                                   # Domain exceptions
│   │   └── [YourException].cs                       # Business rule violations
│   │
│   ├── Events/                                       # Domain events
│   │   └── [YourEvent].cs                           # INotification events
│   │
│   ├── Dtos/                                         # Domain DTOs
│   │   └── [YourDto].cs                             # Data transfer objects
│   │
│   ├── Extensions/                                   # Domain extensions
│   │   └── [YourExtensions].cs                      # Extension methods
│   │
│   ├── Helpers/                                      # Domain helpers
│   │   └── [YourHelper].cs                          # Utility classes
│   │
│   └── Validation/                                   # Domain validation
│       └── [YourValidator].cs                       # Business rule validators
│
├── {Org}.{ProjectName}.Domain.Tests/                # Domain unit tests
│   ├── {Org}.{ProjectName}.Domain.Tests.csproj
│   │
│   ├── Constants/                                    # Test constants
│   ├── Entities/                                     # Entity tests
│   │   └── [YourEntity]Tests.cs
│   ├── ValueObjects/                                 # Value object tests
│   │   └── [YourValueObject]Tests.cs
│   ├── Exceptions/                                   # Exception tests
│   ├── Extensions/                                   # Extension tests
│   ├── Helpers/                                      # Helper tests
│   └── Defaults/                                     # Test data builders
│       └── Default[YourEntity].cs
│
├── {Org}.{ProjectName}.Application/                 # APPLICATION LAYER (Use Cases)
│   ├── {Org}.{ProjectName}.Application.csproj       # Depends on: Domain
│   ├── Program.cs                                    # DI configuration
│   ├── Result.cs                                     # Result pattern
│   │
│   ├── Commands/                                     # Write operations (CQRS)
│   │   └── [YourCommand].cs                         # IRequest<Result<T>>
│   │   └── [YourCommand]Handler.cs                  # Command handler
│   │
│   ├── Queries/                                      # Read operations (CQRS)
│   │   └── [YourQuery].cs                           # IRequest<Result<T>>
│   │   └── [YourQuery]Handler.cs                    # Query handler
│   │
│   ├── Validators/                                   # FluentValidation
│   │   └── [YourCommand]Validator.cs                # AbstractValidator<T>
│   │
│   ├── Behaviors/                                    # MediatR pipeline behaviors
│   │   └── ValidationBehavior.cs                    # IPipelineBehavior
│   │
│   ├── Dtos/                                         # Application DTOs
│   │   └── [YourDto].cs                             # Request/Response DTOs
│   │
│   ├── Services/                                     # Application services
│   │   ├── [YourService].cs                         # Service interface/impl
│   │   └── Fakes/                                    # Fake implementations
│   │       └── Fake[YourService].cs                 # For local development
│   │
│   ├── Mapper/                                       # AutoMapper profiles
│   │   └── ApplicationAutoMapperProfile.cs
│   │
│   └── CloudEvents/                                  # Event handling
│       ├── [YourEventHandler].cs                    # Event handlers
│       └── Contracts/                                # Event contracts
│           └── [YourEvent]Contract.cs
│
├── {Org}.{ProjectName}.Application.Tests/           # Application unit tests
│   ├── {Org}.{ProjectName}.Application.Tests.csproj
│   │
│   ├── Commands/                                     # Command handler tests
│   │   └── [YourCommand]Tests.cs
│   ├── Queries/                                      # Query handler tests
│   │   └── [YourQuery]Tests.cs
│   ├── Validators/                                   # Validator tests
│   │   └── [YourValidator]Tests.cs
│   ├── Services/                                     # Service tests
│   ├── Behaviors/                                    # Behavior tests
│   ├── CloudEvents/                                  # Event handler tests
│   ├── Defaults/                                     # Test data builders
│   │   └── Default[YourCommand].cs
│   └── TestHelpers/                                  # Test utilities
│       └── [YourHelper].cs
│
├── {Org}.{ProjectName}.Infrastructure/              # INFRASTRUCTURE LAYER
│   ├── {Org}.{ProjectName}.Infrastructure.csproj    # Depends on: Domain
│   │
│   ├── Constants/                                    # Infrastructure constants
│   │
│   ├── Repositories/                                 # Repository implementations
│   │   └── [Your]Repository.cs                      # CosmosDB, SQL, etc.
│   │
│   ├── Gateways/                                     # External service clients
│   │   └── [Your]Gateway.cs                         # HTTP clients
│   │
│   ├── EventHandlers/                                # Infrastructure event handlers
│   │   └── [Your]EventHandler.cs
│   │
│   ├── EmsCloudEvents/                               # EMS event integration
│   │   └── [Your]CloudEvent.cs
│   │
│   ├── QueuePublishers/                              # Message queue publishers
│   │   └── [Your]Publisher.cs                       # Service Bus, Storage Queue
│   │
│   ├── OutgoingEvents/                               # Outgoing event definitions
│   │   └── [Your]Event.cs
│   │
│   ├── HealthChecks/                                 # Health check implementations
│   │   └── [Your]HealthCheck.cs
│   │
│   ├── Dtos/                                         # Infrastructure DTOs
│   │   └── [Your]Dto.cs                             # External API contracts
│   │
│   ├── Enums/                                        # Infrastructure enums
│   ├── Exceptions/                                   # Infrastructure exceptions
│   ├── Extensions/                                   # Extension methods
│   ├── Helpers/                                      # Infrastructure helpers
│   ├── Interfaces/                                   # Infrastructure interfaces
│   │
│   ├── Mapper/                                       # AutoMapper profiles
│   │   └── InfrastructureAutoMapperProfile.cs
│   │
│   ├── Startup/                                      # DI registration
│   │   └── InfrastructureStartup.cs
│   │
│   └── Mocks/                                        # Mock implementations
│       ├── ServiceClients/                           # Mock service clients
│       │   ├── Mock[Service]Client.cs
│       │   └── Dtos/                                 # Mock DTOs
│       └── DelegatingHandlers/                       # HTTP mock handlers
│           └── Mock[Service]Handler.cs
│
├── {Org}.{ProjectName}.Infrastructure.Tests/        # Infrastructure unit tests
│   ├── {Org}.{ProjectName}.Infrastructure.Tests.csproj
│   │
│   ├── Repositories/                                 # Repository tests
│   ├── Gateways/                                     # Gateway tests
│   ├── EventHandlers/                                # Event handler tests
│   ├── EmsCloudEvents/                               # Cloud event tests
│   ├── HealthChecks/                                 # Health check tests
│   ├── Extensions/                                   # Extension tests
│   ├── Helpers/                                      # Helper tests
│   └── Mocking/                                      # Mock object builders
│
├── {Org}.{ProjectName}.Service/                     # PRESENTATION LAYER (API)
│   ├── {Org}.{ProjectName}.Service.csproj           # Depends on: Application, Infrastructure
│   ├── Program.cs                                    # Application entry point
│   ├── appsettings.json                             # Configuration
│   ├── InternalsVisibleTo.cs
│   │
│   ├── Controllers/                                  # API Controllers
│   │   └── [Your]Controller.cs                      # REST endpoints
│   │
│   ├── Middleware/                                   # Custom middleware
│   │   └── [Your]Middleware.cs                      # Request pipeline
│   │
│   ├── Filters/                                      # Action/Exception filters
│   │   └── [Your]Filter.cs
│   │
│   ├── ErrorHandling/                                # Error handling
│   │   └── GlobalExceptionHandler.cs
│   │
│   ├── Extensions/                                   # Extension methods
│   │   └── ServiceCollectionExtensions.cs
│   │
│   ├── Dtos/                                         # API request/response models
│   │   └── [Your]Request.cs
│   │   └── [Your]Response.cs
│   │
│   ├── Mapper/                                       # AutoMapper profiles
│   │   └── ServiceAutoMapperProfile.cs
│   │
│   ├── Helpers/                                      # Presentation helpers
│   ├── Constants/                                    # API constants
│   │
│   ├── Startup/                                      # Startup configuration
│   │   └── ServiceStartup.cs
│   │
│   └── Properties/                                   # Launch settings
│       └── launchSettings.json
│
├── {Org}.{ProjectName}.Service.Tests/               # API unit tests
│   ├── {Org}.{ProjectName}.Service.Tests.csproj
│   │
│   ├── Controllers/                                  # Controller tests
│   │   └── [Your]ControllerTests.cs
│   ├── Middleware/                                   # Middleware tests
│   ├── Filters/                                      # Filter tests
│   ├── Mapper/                                       # Mapper tests
│   └── Helpers/                                      # Helper tests
│
├── {Org}.{ProjectName}.Service.Functions/           # AZURE FUNCTIONS (Optional)
│   ├── {Org}.{ProjectName}.Service.Functions.csproj
│   ├── host.json                                     # Functions host config
│   ├── local.settings.json                          # Local development config
│   │
│   ├── Functions/                                    # Function definitions
│   │   ├── System/                                   # System functions
│   │   │   └── HealthCheckFunction.cs
│   │   ├── ServiceBus/                               # Service Bus triggers
│   │   │   └── [Your]ServiceBusFunction.cs
│   │   └── StorageQueue/                             # Queue triggers
│   │       └── [Your]QueueFunction.cs
│   │
│   ├── EventHandlers/                                # Function event handlers
│   ├── Constants/                                    # Function constants
│   ├── Extensions/                                   # Extension methods
│   ├── Middleware/                                   # Function middleware
│   └── Wrappers/                                     # Function wrappers
│
└── {Org}.{ProjectName}.Service.Functions.Tests/     # Function tests
    ├── {Org}.{ProjectName}.Service.Functions.Tests.csproj
    │
    ├── Functions/                                    # Function tests
    ├── EventHandlers/                                # Event handler tests
    ├── Helpers/                                      # Helper tests
    └── Wrappers/                                     # Wrapper tests
```

## Layer Responsibilities

### Domain Layer (Core)
- **No external dependencies** - pure C# and MediatR only
- Contains business entities, value objects, domain logic
- Defines repository interfaces (not implementations)
- Contains domain events and exceptions
- Encapsulates business rules and validation

### Application Layer (Use Cases)
- **Depends only on Domain**
- Implements CQRS with MediatR (Commands and Queries)
- Contains use case handlers (business workflows)
- FluentValidation for input validation
- Defines application service interfaces
- Contains DTOs for data transfer between layers

### Infrastructure Layer (External Concerns)
- **Depends on Domain** (and Application for some implementations)
- Implements repository interfaces from Domain
- External service integration (HTTP clients, gateways)
- Data access (CosmosDB, SQL, etc.)
- Message queuing (Service Bus, Storage Queue)
- Health checks, event handlers
- Mock implementations for testing

### Presentation Layer (API/Functions)
- **Depends on Application and Infrastructure**
- REST API controllers
- Middleware, filters, error handling
- Azure Functions (optional)
- Swagger/OpenAPI documentation
- Authentication/Authorization
- Request/Response mapping

## Naming Conventions

### Files
- Entities: `Cart.cs`, `Order.cs`
- Value Objects: `Money.cs`, `Address.cs`
- Commands: `CreateCartCommand.cs`
- Queries: `GetCartQuery.cs`
- Handlers: `CreateCartCommandHandler.cs`
- Validators: `CreateCartCommandValidator.cs`
- Controllers: `CartsController.cs`
- Tests: `CreateCartCommandTests.cs`

### Folders
- Use PascalCase for folders
- Plural for collections: `Entities/`, `Commands/`, `Controllers/`
- Nested folders for subcategories: `Constants/Errors/`

## Key Files to Create After Scaffolding

### Domain Layer
1. First Entity with factory method
2. Value Objects as needed
3. Domain exceptions
4. Repository interfaces

### Application Layer
1. First Command and Handler
2. Command Validator
3. Result pattern usage
4. Program.cs DI configuration
5. ValidationBehavior pipeline

### Infrastructure Layer
1. Repository implementations
2. Startup.cs for DI registration
3. Health checks

### Presentation Layer
1. First Controller
2. Program.cs startup configuration
3. appsettings.json
4. Swagger configuration

## Testing Structure

- Each layer has a corresponding `.Tests` project
- Use xUnit, FluentAssertions, NSubstitute
- Follow AAA pattern (Arrange-Act-Assert)
- Test naming: `MethodName_WithStateUnderTest_ExpectedBehavior`
- Create `Defaults/` folder with test data builders

## Configuration Files

### .editorconfig
Copy from repository root - defines code style

### .gitignore
Standard .NET gitignore plus custom exclusions

### Directory.Build.props (optional)
Shared MSBuild properties across projects

## Best Practices

1. **Maintain layer boundaries** - never skip layers
2. **Keep Domain pure** - no infrastructure concerns
3. **Use dependency injection** - interface-based dependencies
4. **Write tests first** - TDD approach recommended
5. **Follow SOLID principles** - especially SRP and DIP
6. **Use primary constructors** - for DI in handlers (.NET 8+)
7. **File-scoped namespaces** - required by .editorconfig

## References

- Clean Architecture by Robert C. Martin
- Domain-Driven Design by Eric Evans
- Implementing DDD by Vaughn Vernon
- Microsoft .NET Architecture Guides
