---
name: ddd-project-framework-setup
description: Scaffolds a complete DDD Clean Architecture project framework based on the Vehicle Maintenance Service architecture. Creates solution structure with Domain, Application, Infrastructure, and Presentation layers including all standard folders, project files, and configuration. Only creates the folder structure and base files - no domain-specific implementations. Follows strict DDD principles with custom Command/Query pattern (NO MediatR), manual mapping (NO AutoMapper), and entity validation patterns.
---

# DDD Project Framework Setup Skill

This skill creates a complete DDD Clean Architecture project structure based on proven patterns from the Vehicle Maintenance Service. It scaffolds all necessary projects, folders, and configuration files for a new service while maintaining the architectural integrity and conventions established in the CarMax ERO ecosystem.

## What This Skill Does

**Creates**: 
- Complete folder structure with all standard folders (docs, .pipelines, .github, .vscode)
- All .csproj files with proper dependencies and package references
- Solution file organized into folder groups
- Configuration files (.editorconfig, .gitignore, .gitattributes)
- Base Program.cs files with DI configuration
- GitHub Copilot instructions
- README with getting started guide
- Architecture documentation templates (coding-guidelines.md, target-architecture.md)

**Does NOT Create**: Domain-specific entities, commands, queries, controllers, or business logic

The skill generates a **production-ready framework** that developers can immediately start adding features to following strict DDD and Clean Architecture principles.

## Key Architectural Decisions

This framework enforces the following architectural patterns from Vehicle Maintenance Service:

1. **NO MediatR** - Uses custom Command/Query interfaces (ICreateXCommand, IUpdateXCommand, IGetXQuery)
2. **NO AutoMapper** - Uses explicit static mapping classes per layer
3. **Entity Validation Pattern** - Static private validation methods called by factory methods
4. **Result<T> vs Exceptions** - Prefer throwing exceptions with CustomExceptionFilter for global error handling
5. **Problem Details Pattern** - Standard error responses following Microsoft/CarMax guidelines
6. **Vertical Slice Architecture** - Application layer organized by feature/resource folders
7. **FluentValidation** - BasicValidator suffix for DTO validation, business validation in entities

## When to Use This Skill

Invoke this skill when:
- Starting a new microservice that follows DDD/Clean Architecture
- Creating a template for standardized service structure
- Need to quickly bootstrap a project with proper layering
- Want to ensure consistency across multiple services

## Architecture Overview

The framework follows Clean Architecture with 4 layers:

```
{ProjectName}.Service (Presentation)
    ↓ depends on
{ProjectName}.Infrastructure (Data Access & External Services)  
    ↓ depends on
{ProjectName}.Application (Use Cases & Business Logic)
    ↓ depends on  
{ProjectName}.Domain (Core Business Domain)
```

**Dependency Rule**: Dependencies only flow inward. Domain has ZERO external dependencies (except CosmosDB interfaces where required by nuget package design).

### Layer Responsibilities

**Domain Layer** - Core business domain with ZERO infrastructure dependencies
- Entities with private setters and rich factory methods
- Value Objects (immutable sub-objects without identity)
- Domain Models (rich objects with business logic, not just DTOs)
- Domain Events
- Domain Exceptions
- Interfaces for repositories and external services (implementations in Infrastructure)
- Constants, Enums
- DTOs only for inter-layer data transfer (NOT for external APIs)

**Application Layer** - Orchestrates domain logic and coordinates with infrastructure
- Custom Command interfaces (ICreateXCommand, IUpdateXCommand) - NO MediatR
- Custom Query interfaces (IGetXQuery, IListXQuery) - NO MediatR  
- Command/Query handlers
- FluentValidation validators (BasicValidator suffix for DTOs)
- Business validation orchestration (entities perform actual validation)
- Custom static mapping classes (NO AutoMapper)
- Application services for cross-aggregate logic
- Vertical Slice Architecture (organize by feature/resource folders)

**Infrastructure Layer** - External concerns and implementation details
- Repository implementations
- Gateway implementations for external HTTP services
- Request/Response DTOs for external services (isolated to specific gateway)
- Custom static mapping classes (NO AutoMapper)
- Health checks
- Event handlers
- Queue publishers
- CosmosDB configuration
- Polly retry policies

**Presentation Layer (Service)** - API controllers and HTTP concerns
- Controllers per sub-resource (NOT one controller for everything)
- Request/Response DTOs (specific to API layer, never passed to Application)
- CustomExceptionFilter for global error handling
- Problem Details pattern for error responses
- Middleware, Filters
- Custom static mapping classes (NO AutoMapper)
- Dependency injection configuration

## Coding Guidelines and Patterns

The framework enforces these critical patterns from the Vehicle Maintenance Service coding guidelines:

### Entity Validation Pattern (Required)

Entities must validate business rules using **static private validation methods** called by factory methods:

```csharp
// Static private validation - not exposed outside entity
private static void CanUpdateSchedule(
    Schedule existingSchedule,
    bool isActive,
    string scheduleType)
{
    var errors = new List<DomainValidationFailure>();
    
    if (!isActive && scheduleType == ScheduleTypes.Required)
    {
        errors.Add(new DomainValidationFailure(
            propertyName: nameof(IsActive),
            errorCode: nameof(ScheduleErrors.REQUIRED_MUST_BE_ACTIVE),
            errorMessage: ScheduleErrors.REQUIRED_MUST_BE_ACTIVE
        ));
    }
    
    ThrowOnDomainErrors(errors);
}

// Factory method calls validation at the beginning
public static Schedule UpdateSchedule(Schedule existing, UpdateCommand cmd)
{
    CanUpdateSchedule(existing, cmd.IsActive, cmd.ScheduleType);
    
    var schedule = new Schedule()
    {
        // ... property mappings ...
    };
    
    return schedule;
}
```

**Key Rules:**
- Validation is static and private
- Called at beginning of factory method before creating entity
- Accepts individual parameters, NOT entire command object
- Command handler just calls factory - validation is automatic
- Test validation through factory method, not directly

### Controller to Command/Query Flow (NO MediatR)

**Pattern:** Request DTO → Command Interface → Domain Model/Entity → Response DTO

```csharp
// Controller
[HttpPost]
public async Task<ActionResult<ScheduleResponse>> Create(
    [FromBody] CreateScheduleRequest request)
{
    // Map request DTO to command properties
    var schedule = await _createCommand.ExecuteAsync(
        request.VehicleId,
        request.ServiceType);
    
    // Map domain model to response DTO
    return _mapper.ToResponse(schedule);
}

// Application Layer - Custom interface (NO MediatR)
public interface ICreateScheduleCommand
{
    Task<Schedule> ExecuteAsync(string vehicleId, string serviceType);
}

// Application Layer - Handler
public class CreateScheduleCommand(IRepository repo) : ICreateScheduleCommand
{
    private readonly IRepository _repo = repo.ValidateArgNotNull(nameof(repo));
    
    public async Task<Schedule> ExecuteAsync(string vehicleId, string serviceType)
    {
        // Orchestrate business logic
        var schedule = Schedule.Create(vehicleId, serviceType);
        await _repo.AddAsync(schedule);
        return schedule;
    }
}
```

### Validation Layers

**BasicValidation (FluentValidation)** - In Application layer
- Suffix: `XBasicValidator`
- Validates DTOs with simple property checks
- No external data or business logic required
- Called explicitly from controllers
- Examples: null checks, required fields, ranges

**Business Validation (Entity Methods)** - In Domain layer
- Static private methods in entities
- Validates domain invariants with business rules
- May require data from parent aggregate
- Called automatically by factory methods
- Throws DomainValidationException

**When to use BusinessValidator classes:**
- Complex validations requiring external service calls
- Cross-service validations
- Validations requiring complex data aggregation beyond parent context

### Mapping Pattern (NO AutoMapper)

Create explicit static mapping classes per layer:

```csharp
// Service layer - API DTOs to/from Domain Models
public static class ScheduleMapper
{
    public static ScheduleResponse ToResponse(Schedule schedule)
    {
        return new ScheduleResponse
        {
            Id = schedule.Id,
            VehicleId = schedule.VehicleId,
            ServiceType = schedule.ServiceType
        };
    }
}

// Application layer - Domain Models to Entities
public static class ScheduleEntityMapper
{
    public static Schedule ToEntity(ScheduleModel model)
    {
        return Schedule.Create(model.VehicleId, model.ServiceType);
    }
}
```

### Gateway Pattern (Infrastructure Layer)

Gateways isolate external service request/response objects:

```csharp
// Infrastructure/Gateways/UserService/
//   - IUserGateway.cs (interface in Domain)
//   - UserGateway.cs (implementation)
//   - Dtos/
//       - UserRequest.cs (NOT exposed to other layers)
//       - UserResponse.cs (NOT exposed to other layers)
//   - Mappers/
//       - UserMapper.cs (maps Response to Domain Model)

public interface IUserGateway
{
    Task<UserModel> GetUserAsync(string userId); // Returns Domain Model
}

public class UserGateway(HttpClient client) : IUserGateway
{
    public async Task<UserModel> GetUserAsync(string userId)
    {
        var request = new UserRequest { Id = userId };
        var response = await client.PostAsync(...);
        
        // Map response to Domain Model before returning
        return UserMapper.ToDomainModel(response);
    }
}
```

### Error Handling Pattern

**Use CustomExceptionFilter for global error handling:**
- Most commands/queries should throw exceptions directly
- CustomExceptionFilter catches and converts to Problem Details
- Only use Result<T> when you need custom handling of different result states

```csharp
// Preferred: Throw exceptions
public async Task<Schedule> ExecuteAsync(string id)
{
    var schedule = await _repo.GetByIdAsync(id)
        ?? throw new NotFoundException($"Schedule {id} not found");
    return schedule;
}

// Only when needed: Use Result<T>
public async Task<Result<Schedule>> ExecuteAsync(string id)
{
    // When controller needs different HTTP codes based on business logic
}
```

### C# Code Style

**Required conventions:**
- File-scoped namespaces
- Primary constructors with explicit dependency assignment:
  ```csharp
  public class Handler(IRepo repo, ILogger logger)
  {
      private readonly IRepo _repo = repo.ValidateArgNotNull(nameof(repo));
      private readonly ILogger _logger = logger.ValidateArgNotNull(nameof(logger));
}
```

## Documentation Templates

The skill should create these documentation files in the `docs/architecture/` folder:

### coding-guidelines.md Template

This file should contain the comprehensive coding guidelines covering:
- Controller patterns
- Error handling with CustomExceptionFilter
- Validation patterns (BasicValidator vs Business Validation)
- Entity Validation Pattern with examples
- DTO, Models, and Responses
- Gateway pattern with isolated DTOs
- Domain Models vs DTOs vs Entities
- Controller to Command/Query flow (NO MediatR)
- Mapping patterns (NO AutoMapper)
- Database saving patterns
- Result<T> vs Exception guidelines
- C# recommendations (primary constructors, file-scoped namespaces, etc.)
- Unit testing standards (xUnit, FluentAssertions, NSubstitute, AutoFixture)
- Test data management patterns
- Test design principles

### target-architecture.md Template

This file should contain:
- Architecture overview with DDD layer diagram
- Explanation of Domain, Application, Infrastructure, and Presentation layers
- What each layer should contain
- Rules for each layer
- Project dependency rules
- Links to DDD learning resources
- References to coding-guidelines.md

Both templates should be based on the Vehicle Maintenance Service documentation structure.
- Prefer record types unless just bag of properties
- Never introduce new warnings (TreatWarningsAsErrors=true)
- Use explicit descriptive names over comments
- No unused using statements (alphabetically sorted)

### Unit Testing Standards

**Required frameworks:**
- xUnit (NOT NUnit or MSTest)
- FluentAssertions (for assertions)
- NSubstitute (for mocking - NO Moq)
- AutoFixture (for test data generation)

**Patterns:**
- AAA pattern (Arrange-Act-Assert) with comments
- One assertion per test (one logical outcome)
- Test naming: `MethodName_WithGivenScenario_ExpectedBehavior`
- DTOs don't need unit tests
- DO NOT test private methods - only public interfaces
- Test entity validation through factory methods

**Test data management:**
- Use `Guid.NewGuid().ToString()` for IDs
- Use concrete readable values for business data
- Use AutoFixture for complex objects where values don't matter
- Create helper methods with default parameters
- Extract shared test data as readonly fields

**Example:**
```csharp
[Fact]
public void UpdateSchedule_WithInvalidState_ThrowsDomainValidationException()
{
    // Arrange
    var existingSchedule = GetSchedule();
    var command = CreateCommand(isActive: false);

    // Act & Assert
    var exception = Assert.Throws<DomainValidationException>(() =>
        Schedule.UpdateSchedule(existingSchedule, command, ScheduleTypes.Required));

    exception.Errors.Should().ContainSingle();
    exception.Errors[0].PropertyName.Should().Be(nameof(IsActive));
}
```

### Project Dependency Rules

**STRICTLY ENFORCED:**
1. Service project depends ONLY on Application + Infrastructure
2. Application project depends ONLY on Domain
3. Infrastructure project depends ONLY on Domain
4. Domain has NO project dependencies (only nuget packages)

**Violation of these rules will break the architecture!**

## Interactive Workflow

When this skill is invoked, the agent will:

1. **Prompt for Project Name** (required)
   - Example: "OrderManagement", "InventoryService", "CustomerPortal"
   - Agent will ask: "What is the name of your new project?"
   
2. **Prompt for Organization Prefix** (optional, default: "CarMax")
   - Example: "CarMax", "Acme", "CompanyName"
   - Agent will ask: "What organization prefix should be used? (default: CarMax)"

3. **Prompt for Target Directory** (optional, default: "./src")
   - Where to create the solution
   - Agent will ask: "Where should the solution be created? (default: ./src)"

4. **Prompt for Optional Components** (optional)
   - Azure Functions project
   - Integration test projects
   - Agent will ask: "Include Azure Functions project? (Y/n)" and "Include integration test projects? (Y/n)"

5. **Execute Scaffolding**
   - Create all folders
   - Generate all project files
   - Create solution file
   - Generate README with next steps

## Project Structure Created

```
{project-root}/
├── .editorconfig
├── .gitattributes
├── .gitignore
├── .github/
│   └── copilot-instructions.md
├── .pipelines/
│   └── (Azure DevOps pipeline configurations)
├── .vscode/
│   └── (VS Code workspace settings)
├── docs/
│   └── (Architecture documentation, ADRs, diagrams)
└── src/
    ├── {Org}.{ProjectName}.sln
├── {Org}.{ProjectName}.Domain/
│   ├── {Org}.{ProjectName}.Domain.csproj
│   ├── InternalsVisibleTo.cs
│   ├── Constants/
│   │   └── Errors/
│   │       └── (Domain error messages and codes)
│   ├── Entities/
│   │   └── (Domain entities with private setters and factory methods)
│   │       └── MaintenanceSchedule.cs
│   ├── ValueObjects/
│   │   └── (Immutable sub-objects without identity)
│   │       └── ServiceInterval.cs
│   ├── Models/
│   │   └── (Rich domain objects with business logic, NOT DTOs)
│   │       └── Vehicle.cs
│   ├── Enums/
│   ├── Interfaces/
│   │   └── (Repository and service interfaces - implementations in Infrastructure)
│   │       ├── IMaintenanceScheduleRepository.cs
│   │       └── IUserGateway.cs
│   ├── Exceptions/
│   │   ├── DomainValidationException.cs
│   │   └── (Domain-specific exceptions)
│   ├── Events/
│   │   └── (Domain events for state changes)
│   ├── Dtos/
│   │   └── (ONLY for inter-layer data transfer, NOT external APIs)
│   ├── Extensions/
│   ├── Helpers/
│   └── Validation/
│       └── DomainValidationFailure.cs
├── {Org}.{ProjectName}.Domain.Tests/
│   ├── {Org}.{ProjectName}.Domain.Tests.csproj
│   ├── Constants/
│   ├── Entities/
│   ├── ValueObjects/
│   ├── Exceptions/
│   ├── Extensions/
│   ├── Helpers/
│   └── Defaults/
├── {Org}.{ProjectName}.Application/
│   ├── {Org}.{ProjectName}.Application.csproj
│   ├── Program.cs (DI configuration)
│   ├── Result.cs
│   ├── Commands/
│   │   └── (Organized by feature/resource - Vertical Slice)
│   │       └── MaintenanceSchedule/
│   │           ├── ICreateMaintenanceScheduleCommand.cs
│   │           ├── CreateMaintenanceScheduleCommand.cs
│   │           ├── IUpdateMaintenanceScheduleCommand.cs
│   │           ├── UpdateMaintenanceScheduleCommand.cs
│   │           ├── Validators/
│   │           │   ├── CreateMaintenanceScheduleBasicValidator.cs
│   │           │   └── UpdateMaintenanceScheduleBasicValidator.cs
│   │           └── Mappers/
│   │               └── MaintenanceScheduleMapper.cs
│   ├── Queries/
│   │   └── (Organized by feature/resource - Vertical Slice)
│   │       └── MaintenanceSchedule/
│   │           ├── IGetMaintenanceScheduleQuery.cs
│   │           ├── GetMaintenanceScheduleQuery.cs
│   │           └── Mappers/
│   ├── Common/
│   │   └── (Shared code between multiple slices)
│   ├── Validators/
│   ├── Behaviors/
│   ├── Dtos/
│   ├── Services/
│   │   └── Fakes/
│   ├── Mapper/
│   └── CloudEvents/
│       └── Contracts/
├── {Org}.{ProjectName}.Application.Tests/
│   ├── {Org}.{ProjectName}.Application.Tests.csproj
│   ├── Commands/
│   ├── Queries/
│   ├── Validators/
│   ├── Services/
│   ├── Behaviors/
│   ├── CloudEvents/
│   ├── Defaults/
│   └── TestHelpers/
├── {Org}.{ProjectName}.Infrastructure/
│   ├── {Org}.{ProjectName}.Infrastructure.csproj
│   ├── Constants/
│   ├── Repositories/
│   ├── Gateways/
│   │   └── (One folder per gateway with isolated DTOs)
│   │       └── UserGateway/
│   │           ├── IUserGateway.cs (interface defined in Domain)
│   │           ├── UserGateway.cs
│   │           ├── Dtos/
│   │           │   ├── UserRequest.cs (NOT exposed to other layers)
│   │           │   └── UserResponse.cs (NOT exposed to other layers)
│   │           └── Mappers/
│   │               └── UserMapper.cs (maps to Domain Models)
│   ├── EventHandlers/
│   ├── EmsCloudEvents/
│   ├── QueuePublishers/
│   ├── OutgoingEvents/
│   ├── HealthChecks/
│   ├── Dtos/
│   ├── Enums/
│   ├── Exceptions/
│   ├── Extensions/
│   ├── Helpers/
│   ├── Interfaces/
│   ├── Mapper/
│   ├── Startup/
│   └── Mocks/
│       ├── ServiceClients/
│       │   └── Dtos/
│       └── DelegatingHandlers/
├── {Org}.{ProjectName}.Infrastructure.Tests/
│   ├── {Org}.{ProjectName}.Infrastructure.Tests.csproj
│   ├── Repositories/
│   ├── Gateways/
│   ├── EventHandlers/
│   ├── EmsCloudEvents/
│   ├── HealthChecks/
│   ├── Extensions/
│   ├── Helpers/
│   └── Mocking/
├── {Org}.{ProjectName}.Service/
│   ├── {Org}.{ProjectName}.Service.csproj
│   ├── Program.cs
│   ├── appsettings.json
│   ├── InternalsVisibleTo.cs
│   ├── Controllers/
│   │   └── (One controller per sub-resource)
│   │       └── MaintenanceSchedulesController.cs
│   ├── Middleware/
│   ├── Filters/
│   │   └── CustomExceptionFilter.cs (global error handling)
│   ├── ErrorHandling/
│   ├── Extensions/
│   ├── Dtos/
│   │   └── (Request/Response DTOs specific to API layer)
│   │       ├── CreateMaintenanceScheduleRequest.cs
│   │       └── MaintenanceScheduleResponse.cs
│   ├── Mapper/
│   │   └── (Static mapping classes - NO AutoMapper)
│   ├── Helpers/
│   ├── Constants/
│   ├── Startup/
│   └── Properties/
├── {Org}.{ProjectName}.Service.Tests/
│   ├── {Org}.{ProjectName}.Service.Tests.csproj
│   ├── Controllers/
│   ├── Middleware/
│   ├── Filters/
│   ├── Mapper/
│   └── Helpers/
└── {Org}.{ProjectName}.Service.Functions/ (if selected)
    ├── {Org}.{ProjectName}.Service.Functions.csproj
    ├── host.json
    ├── local.settings.json
    ├── Functions/
    │   ├── System/
    │   ├── ServiceBus/
    │   └── StorageQueue/
    ├── EventHandlers/
    ├── Constants/
    ├── Extensions/
    ├── Middleware/
    └── Wrappers/
```

## Generated Files

### 1. Domain Layer (.csproj)
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <RestorePackagesWithLockFile>true</RestorePackagesWithLockFile>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <TreatWarningsAsErrors>True</TreatWarningsAsErrors>
    <NoWarn>1701;1702;1591;CA1014</NoWarn>
    <WarningsNotAsErrors>NU1901;NU1902;NU1903;NU1904</WarningsNotAsErrors>
  </PropertyGroup>
  <!-- Domain has NO dependencies except CosmosDB interfaces if needed -->
  <ItemGroup>
    <!-- Add CosmosDB packages only if needed for IContainerItem -->
  </ItemGroup>
</Project>
```

### 2. Application Layer (.csproj)
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <RestorePackagesWithLockFile>true</RestorePackagesWithLockFile>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <TreatWarningsAsErrors>True</TreatWarningsAsErrors>
    <NoWarn>1701;1702;1591;CA1014</NoWarn>
    <WarningsNotAsErrors>NU1901;NU1902;NU1903;NU1904</WarningsNotAsErrors>
  </PropertyGroup>
  <ItemGroup>
    <!-- NO MediatR - using custom Command/Query interfaces -->
    <!-- NO AutoMapper - using custom static mapping classes -->
    <PackageReference Include="FluentValidation.AspNetCore" Version="11.3.0" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\{Org}.{ProjectName}.Domain\{Org}.{ProjectName}.Domain.csproj" />
  </ItemGroup>
</Project>
```

### 3. Infrastructure Layer (.csproj)
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <RestorePackagesWithLockFile>true</RestorePackagesWithLockFile>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <TreatWarningsAsErrors>True</TreatWarningsAsErrors>
    <NoWarn>1701;1702;1591;CA1014</NoWarn>
    <WarningsNotAsErrors>NU1901;NU1902;NU1903;NU1904</WarningsNotAsErrors>
  </PropertyGroup>
  <ItemGroup>
    <!-- NO AutoMapper - using custom static mapping classes -->
    <PackageReference Include="FluentValidation.AspNetCore" Version="11.3.0" />
    <PackageReference Include="Polly" Version="8.4.1" />
    <PackageReference Include="Polly.Contrib.WaitAndRetry" Version="1.1.1" />
    <PackageReference Include="Microsoft.Extensions.Http.Polly" Version="8.0.7" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\{Org}.{ProjectName}.Domain\{Org}.{ProjectName}.Domain.csproj" />
  </ItemGroup>
</Project>
```

### 4. Service/Presentation Layer (.csproj)
```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Configurations>Debug;Release;dev;qa;prod</Configurations>
    <RestorePackagesWithLockFile>true</RestorePackagesWithLockFile>
    <DisableImplicitNuGetFallbackFolder>true</DisableImplicitNuGetFallbackFolder>
    <EnableNETAnalyzers>true</EnableNETAnalyzers>
    <AnalysisMode>AllEnabledByDefault</AnalysisMode>
    <AnalysisLevel>latest</AnalysisLevel>
    <TreatWarningsAsErrors>True</TreatWarningsAsErrors>
    <NoWarn>1701;1702;1591;CA1014</NoWarn>
    <WarningsNotAsErrors>NU1901;NU1902;NU1903;NU1904</WarningsNotAsErrors>
    <DocumentationFile>bin\$(Configuration)\$(TargetFramework)\$(MSBuildThisFileName).xml</DocumentationFile>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.Mvc.NewtonsoftJson" Version="8.0.7" />
    <PackageReference Include="Swashbuckle.AspNetCore" Version="6.7.0" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\{Org}.{ProjectName}.Application\{Org}.{ProjectName}.Application.csproj" />
    <ProjectReference Include="..\{Org}.{ProjectName}.Infrastructure\{Org}.{ProjectName}.Infrastructure.csproj" />
  </ItemGroup>
</Project>
```

### 5. Test Project (.csproj)
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
    <TreatWarningsAsErrors>True</TreatWarningsAsErrors>
    <NoWarn>1701;1702;1591;CA1014</NoWarn>
    <WarningsNotAsErrors>NU1901;NU1902;NU1903;NU1904</WarningsNotAsErrors>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="AutoFixture" Version="4.18.1" />
    <PackageReference Include="AutoFixture.AutoNSubstitute" Version="4.18.1" />
    <PackageReference Include="FluentAssertions" Version="6.12.0" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.10.0" />
    <PackageReference Include="NSubstitute" Version="5.1.0" />
    <PackageReference Include="NSubstitute.Analyzers.CSharp" Version="1.0.17">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="xunit" Version="2.9.0" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.8.2">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="coverlet.collector" Version="6.0.2">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\{Org}.{ProjectName}.{Layer}\{Org}.{ProjectName}.{Layer}.csproj" />
  </ItemGroup>
  <ItemGroup>
    <AssemblyAttribute Include="System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute" />
  </ItemGroup>
</Project>
```

### 6. Application/Program.cs (DI Configuration)
```csharp
using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace {Org}.{ProjectName}.Application;

public static class Program
{
    public static IServiceCollection ConfigureApplication(
        this IServiceCollection services, 
        bool isLocal, 
        IConfiguration configuration)
    {
        // Register FluentValidation validators
        var fluentValidatorAssembly = Assembly.Load("{Org}.{ProjectName}.Application");
        services.AddValidatorsFromAssembly(fluentValidatorAssembly);

        // NO MediatR - Register custom Command/Query interfaces manually
        // Example:
        // services.AddScoped<ICreateScheduleCommand, CreateScheduleCommand>();
        // services.AddScoped<IUpdateScheduleCommand, UpdateScheduleCommand>();
        // services.AddScoped<IGetScheduleQuery, GetScheduleQuery>();

        // Add your service registrations here

        return services;
    }
}
```

### 7. Result Pattern (Application/Result.cs)
```csharp
namespace {Org}.{ProjectName}.Application;

public class Result<T>
{
    public T? Value { get; }
    public Exception? Exception { get; }
    public bool IsSuccess => Exception == null;
    public bool IsFailure => Exception != null;

    private Result(T value)
    {
        Value = value;
        Exception = null;
    }

    private Result(Exception exception)
    {
        Value = default;
        Exception = exception;
    }

    public static implicit operator Result<T>(T value) => new(value);
    public static implicit operator Result<T>(Exception exception) => new(exception);
}
```

### 8. InternalsVisibleTo.cs
```csharp
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("{Org}.{ProjectName}.{Layer}.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")] // For NSubstitute
```

### 9. Service/appsettings.json
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### 10. Functions/host.json (if Azure Functions selected)
```json
{
  "version": "2.0",
  "logging": {
    "applicationInsights": {
      "samplingSettings": {
        "isEnabled": true,
        "maxTelemetryItemsPerSecond": 20
      }
    }
  },
  "extensions": {
    "http": {
      "routePrefix": "api"
    }
  }
}
```

### 11. Functions/local.settings.json
```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated"
  }
}
```

## Solution File Structure

The .sln file organizes projects into solution folders:
- **Domain** (Domain + Domain.Tests)
- **Application** (Application + Application.Tests)
- **Infrastructure** (Infrastructure + Infrastructure.Tests)
- **Presentation** (Service + Service.Tests + Functions + Functions.Tests)
- **Tests** (All test projects grouped)

## Post-Scaffolding README

The skill generates a README.md in the solution root with:
- Architecture overview
- Project descriptions
- Build commands
- Next steps for developers
- Links to AGENTS.md and other documentation

## Execution Steps

The agent will execute these steps in order:

1. **Validate input parameters**
   - Project name is valid C# identifier
   - Target directory exists or can be created
   
2. **Create directory structure**
   - Use scripts/create-folders.ps1 or bash equivalent
   
3. **Generate project files**
   - Use templates with token replacement ({Org}, {ProjectName})
   
4. **Create solution file**
   - Use dotnet CLI: `dotnet new sln -n {Org}.{ProjectName}`
   - Add all projects to solution
   - Organize into solution folders
   
5. **Generate documentation**
   - Create README.md
   - Copy AGENTS.md if it exists
   - Create .editorconfig
   
6. **Verify structure**
   - Run `dotnet build` to ensure everything compiles
   - Report any issues

## Success Criteria

The framework is successfully created when:
- All projects compile without errors
- Solution loads in Visual Studio/Rider
- All test projects are discovered
- Folder structure matches specification
- All configuration files are valid

## References

See the `references/` folder for:
- Architecture diagrams
- Folder structure reference
- Naming conventions guide
- Best practices document

## Scripts

The `scripts/` folder contains:
- `create-project.ps1` - PowerShell scaffolding script
- `create-project.sh` - Bash scaffolding script
- `validate-structure.ps1` - Validation script

## Example Usage

```
User: "Use the ddd-project-framework-setup skill to create a new project"

Agent: "I'll help you create a new DDD Clean Architecture project. 

What is the name of your new project? (e.g., OrderManagement, InventoryService)"

User: "CustomerPortal"

Agent: "What organization prefix should be used? (default: CarMax)"

User: "Acme"

Agent: "Where should the solution be created? (default: ./src)"

User: "./src"

Agent: "Include Azure Functions project? (Y/n)"

User: "Y"

Agent: "Include integration test projects? (Y/n)"

User: "n"

Agent: [Creates complete project structure]

"Project framework created successfully!

Location: ./src/Acme.CustomerPortal.sln

Structure created:
✓ Acme.CustomerPortal.Domain
✓ Acme.CustomerPortal.Domain.Tests  
✓ Acme.CustomerPortal.Application
✓ Acme.CustomerPortal.Application.Tests
✓ Acme.CustomerPortal.Infrastructure
✓ Acme.CustomerPortal.Infrastructure.Tests
✓ Acme.CustomerPortal.Service
✓ Acme.CustomerPortal.Service.Tests
✓ Acme.CustomerPortal.Service.Functions
✓ Acme.CustomerPortal.Service.Functions.Tests

Next steps:
1. Review the generated README.md
2. Run 'dotnet build' to verify compilation
3. Start adding your domain entities in the Domain layer
4. Refer to AGENTS.md for coding guidelines

Happy coding!"
```

## Important Notes

**Critical Architecture Decisions:**
- **NO MediatR** - Uses custom Command/Query interfaces for CQRS
- **NO AutoMapper** - Uses explicit static mapping classes per layer
- **Entity Validation Pattern** - Static private validation in entities called by factory methods
- **CustomExceptionFilter** - Global error handling, throw exceptions (not Result<T> everywhere)
- **Problem Details Pattern** - Standard error responses
- **Vertical Slice Architecture** - Application layer organized by feature/resource folders

**Framework Characteristics:**
- **No business logic** is generated - only structure and configuration
- **Dependencies can be customized** after generation
- **Follows .editorconfig** conventions from Vehicle Maintenance Service
- **Compatible with .NET 8.0** - update TargetFramework if needed
- **Test projects** use xUnit + FluentAssertions + NSubstitute + AutoFixture
- **All projects** have TreatWarningsAsErrors enabled
- **Uses file-scoped namespaces** as required by editorconfig
- **Primary constructors** with explicit dependency assignment pattern
- **Controllers per sub-resource** - not one mega-controller

**Strict Dependency Rules (ENFORCED):**
1. Service depends ONLY on Application + Infrastructure
2. Application depends ONLY on Domain
3. Infrastructure depends ONLY on Domain
4. Domain has NO dependencies (except CosmosDB interfaces if needed)

**Documentation Generated:**
- README.md with getting started guide
- Architecture documentation (coding-guidelines.md, target-architecture.md)
- GitHub Copilot instructions
- .editorconfig with code style rules

## Maintenance

This skill should be updated when:
- New architectural patterns are established
- Package versions need updating
- New folder conventions are introduced
- Additional optional components are needed
