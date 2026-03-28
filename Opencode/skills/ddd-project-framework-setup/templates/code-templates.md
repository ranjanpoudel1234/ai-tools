# Base Code Templates

These templates provide starter code for key files that should be created in the framework.

## Application/Program.cs

```csharp
using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;

namespace {{Org}}.{{ProjectName}}.Application;

public static class Program
{
    public static IServiceCollection ConfigureApplication(
        this IServiceCollection services, 
        bool isLocal, 
        IConfiguration configuration, 
        Assembly[] mediatrAssemblies)
    {
        var fluentValidatorAssembly = Assembly.Load("{{Org}}.{{ProjectName}}.Application");
        services.AddValidatorsFromAssembly(fluentValidatorAssembly);
        ConfigureFluentValidation();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(mediatrAssemblies));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        // Register application services here
        // Example:
        // services.AddTransient<IMyService, MyService>();

        return services;
    }

    private static void ConfigureFluentValidation() =>
        ValidatorOptions.Global.PropertyNameResolver = (_, member, _) => 
            member != null ? new CamelCasePropertyNamesContractResolver().GetResolvedPropertyName(member.Name) : null;
}
```

## Application/Result.cs

```csharp
namespace {{Org}}.{{ProjectName}}.Application;

/// <summary>
/// Represents the result of an operation that can either succeed with a value or fail with an exception.
/// </summary>
/// <typeparam name="T">The type of the value when the operation succeeds.</typeparam>
public class Result<T>
{
    /// <summary>
    /// Gets the value if the operation was successful.
    /// </summary>
    public T? Value { get; }

    /// <summary>
    /// Gets the exception if the operation failed.
    /// </summary>
    public Exception? Exception { get; }

    /// <summary>
    /// Gets a value indicating whether the operation was successful.
    /// </summary>
    public bool IsSuccess => Exception == null;

    /// <summary>
    /// Gets a value indicating whether the operation failed.
    /// </summary>
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

    /// <summary>
    /// Implicitly converts a value to a successful result.
    /// </summary>
    public static implicit operator Result<T>(T value) => new(value);

    /// <summary>
    /// Implicitly converts an exception to a failed result.
    /// </summary>
    public static implicit operator Result<T>(Exception exception) => new(exception);
}
```

## Application/Behaviors/ValidationBehavior.cs

```csharp
using FluentValidation;
using MediatR;

namespace {{Org}}.{{ProjectName}}.Application.Behaviors;

/// <summary>
/// Pipeline behavior that validates requests before they are handled.
/// </summary>
public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) 
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators = validators;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count != 0)
        {
            throw new ValidationException(failures);
        }

        return await next();
    }
}
```

## InternalsVisibleTo.cs (for each project)

```csharp
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("{{Org}}.{{ProjectName}}.{{Layer}}.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")] // For NSubstitute mocking
```

## Service/appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database={{ProjectName}};Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

## Service/Program.cs (Minimal Starter)

```csharp
using {{Org}}.{{ProjectName}}.Application;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Application layer
var mediatrAssemblies = new[]
{
    typeof(Program).Assembly,
    typeof({{Org}}.{{ProjectName}}.Application.Program).Assembly
};

builder.Services.ConfigureApplication(
    builder.Environment.IsDevelopment(),
    builder.Configuration,
    mediatrAssemblies);

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

## Functions/host.json

```json
{
  "version": "2.0",
  "logging": {
    "applicationInsights": {
      "samplingSettings": {
        "isEnabled": true,
        "maxTelemetryItemsPerSecond": 20,
        "excludedTypes": "Request"
      }
    },
    "logLevel": {
      "default": "Information",
      "Host.Results": "Error",
      "Function": "Information",
      "Host.Aggregator": "Trace"
    }
  },
  "extensions": {
    "http": {
      "routePrefix": "api"
    }
  },
  "functionTimeout": "00:05:00",
  "healthMonitor": {
    "enabled": true,
    "healthCheckInterval": "00:00:10",
    "healthCheckWindow": "00:02:00",
    "healthCheckThreshold": 6,
    "counterThreshold": 0.80
  }
}
```

## Functions/local.settings.json

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "ASPNETCORE_ENVIRONMENT": "Development"
  }
}
```

## Domain Constants Example

```csharp
namespace {{Org}}.{{ProjectName}}.Domain.Constants.Errors;

/// <summary>
/// General error messages used across the domain.
/// </summary>
public static class GeneralErrors
{
    public const string VALUE_CANNOT_BE_NULL_OR_EMPTY = "Value cannot be null or empty";
    public const string VALUE_MUST_BE_A_VALID_ENUM = "Value must be a valid enum value";
    public const string VALUE_MUST_BE_POSITIVE = "Value must be positive";
    public const string VALUE_MUST_BE_GREATER_THAN_ZERO = "Value must be greater than zero";
}
```

## Example Test Class Structure

```csharp
using FluentAssertions;
using Xunit;

namespace {{Org}}.{{ProjectName}}.{{Layer}}.Tests.{{Folder}};

public class ExampleTests
{
    [Fact]
    public void MethodName_WithValidInput_ReturnsExpectedResult()
    {
        // Arrange
        var input = "test";
        
        // Act
        var result = MethodUnderTest(input);
        
        // Assert
        result.Should().Be("expected");
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void MethodName_WithInvalidInput_ThrowsException(string? input)
    {
        // Arrange
        // (setup if needed)
        
        // Act
        var act = () => MethodUnderTest(input);
        
        // Assert
        act.Should().Throw<ArgumentException>();
    }
    
    private static string MethodUnderTest(string? input)
    {
        if (string.IsNullOrEmpty(input))
            throw new ArgumentException("Input cannot be null or empty");
        
        return input;
    }
}
```

## README.md Template

```markdown
# {{Org}}.{{ProjectName}}

[Brief description of what this service does]

## Architecture

This project follows Clean Architecture with Domain-Driven Design principles:

\`\`\`
{{Org}}.{{ProjectName}}.Service (Presentation)
    ↓ depends on
{{Org}}.{{ProjectName}}.Infrastructure (Data Access & External Services)
    ↓ depends on
{{Org}}.{{ProjectName}}.Application (Use Cases & Business Logic)
    ↓ depends on
{{Org}}.{{ProjectName}}.Domain (Core Business Domain)
\`\`\`

**Dependency Rule**: Dependencies flow inward. Domain has ZERO external dependencies.

## Project Structure

- **Domain**: Core business entities, value objects, enums, domain interfaces
- **Application**: CQRS commands/queries, handlers, validators, application services
- **Infrastructure**: Repository implementations, external service clients, data access
- **Service**: REST API controllers, middleware, filters, presentation logic

## Getting Started

### Prerequisites

- .NET 8.0 SDK or later
- Visual Studio 2022, VS Code, or Rider

### Build

\`\`\`bash
dotnet build {{Org}}.{{ProjectName}}.sln
\`\`\`

### Test

\`\`\`bash
# Run all tests
dotnet test {{Org}}.{{ProjectName}}.sln

# Run tests with coverage
dotnet test {{Org}}.{{ProjectName}}.sln --collect:"XPlat Code Coverage"
\`\`\`

### Run Locally

\`\`\`bash
dotnet run --project src/{{Org}}.{{ProjectName}}.Service/{{Org}}.{{ProjectName}}.Service.csproj
\`\`\`

The API will be available at `https://localhost:5001` (HTTPS) or `http://localhost:5000` (HTTP).

Swagger UI: `https://localhost:5001/swagger`

## Development Guidelines

### Code Style

- Follow the conventions in `.editorconfig`
- Use file-scoped namespaces
- TreatWarningsAsErrors is enabled - all warnings must be resolved
- Refer to `AGENTS.md` for detailed coding guidelines

### Testing

- Use xUnit for test framework
- Use FluentAssertions for assertions
- Use NSubstitute for mocking (DO NOT use Moq)
- Follow AAA pattern (Arrange-Act-Assert)
- Test naming: `MethodName_WithStateUnderTest_ExpectedBehavior`

### Branching Strategy

- `main` - production-ready code
- `develop` - integration branch
- Feature branches: `feature/your-feature-name`
- Bug fixes: `bugfix/issue-description`

## Architecture Patterns

### CQRS (Command Query Responsibility Segregation)

- **Commands**: Write operations that change state
- **Queries**: Read operations that return data
- Handled by MediatR

### Repository Pattern

- Domain defines interfaces
- Infrastructure provides implementations
- Supports unit testing with mocks

### Result Pattern

- Operations return `Result<T>` instead of throwing exceptions
- Allows graceful error handling
- Makes success/failure explicit

## Next Steps

1. Define your core domain entities in the Domain layer
2. Create your first command in the Application layer
3. Implement repository in the Infrastructure layer
4. Add controller endpoints in the Service layer
5. Write tests for each component

## Documentation

- See `AGENTS.md` for AI agent development guidelines
- See `docs/` folder for additional documentation
- Check `docs/adr/` for Architecture Decision Records

## Support

[Add contact information or links to issue tracker]

## License

[Add license information]
\`\`\`

## Token Replacement

Replace these tokens when generating files:
- `{{Org}}` - Organization prefix
- `{{ProjectName}}` - Project name
- `{{Layer}}` - Layer name (Domain, Application, Infrastructure, Service)
- `{{Folder}}` - Folder name within layer
