# Quick Reference - Agent Execution Guide

This guide helps AI agents execute the DDD Project Framework Setup skill correctly.

## Invocation Trigger

This skill should be invoked when the user:
- Says "create a new DDD project"
- Says "scaffold a Clean Architecture project"
- Says "use the ddd-project-framework-setup skill"
- Asks to "create a new service with DDD"
- Wants to "start a new microservice"

## Execution Flow

### Step 1: Gather Requirements (Interactive Prompts)

Ask these questions in order:

```
1. "What is the name of your new project? (e.g., OrderManagement, CustomerPortal)"
   - Validate: Must start with uppercase, valid C# identifier
   - Store as: PROJECT_NAME

2. "What organization prefix should be used? (default: Contoso)"
   - Default: "Contoso"
   - Store as: ORG_PREFIX

3. "Where should the solution be created? (default: ./src)"
   - Default: "./src"
   - Validate: Directory exists or can be created
   - Store as: TARGET_DIR

4. "Include Azure Functions project? (Y/n)"
   - Default: Y
   - Store as: INCLUDE_FUNCTIONS (boolean)

5. "Include integration test projects? (Y/n)"
   - Default: n
   - Store as: INCLUDE_INTEGRATION_TESTS (boolean)
```

### Step 2: Validation

Before proceeding, validate:

```typescript
// Project name validation
const isValidProjectName = /^[A-Z][A-Za-z0-9]*$/.test(PROJECT_NAME);
if (!isValidProjectName) {
    ERROR: "Project name must start with uppercase and contain only letters/numbers"
}

// Target directory validation
if (!directoryExists(TARGET_DIR) && !canCreateDirectory(TARGET_DIR)) {
    ERROR: "Cannot create target directory"
}
```

### Step 3: Execute Scaffolding

Use the appropriate script based on platform:

**Windows/PowerShell:**
```powershell
./.claude/skills/ddd-project-framework-setup/scripts/create-project.ps1 `
    -ProjectName "OrderManagement" `
   -OrgPrefix "Contoso" `
    -TargetDir "./src" `
    -IncludeFunctions $true `
    -IncludeIntegrationTests $false
```

**Linux/macOS/Bash:**
```bash
./.claude/skills/ddd-project-framework-setup/scripts/create-project.sh \
    -p OrderManagement \
   -o Contoso \
    -t ./src \
    -f # Include functions (omit -f to skip)
    # Omit -i for no integration tests
```

### Step 4: Post-Creation Tasks

After script execution:

1. **Navigate to solution directory**
   ```bash
   cd ./src/{ORG}.{PROJECT}/
   ```

2. **Customize .csproj files** (if needed)
   - Add organization-specific packages
   - Update package versions
   - Add project-specific configuration

3. **Create base files from templates**
   - Application/Program.cs
   - Application/Result.cs
   - Application/Behaviors/ValidationBehavior.cs
   - Service/Program.cs
   - InternalsVisibleTo.cs for each project

4. **Copy configuration files**
   - Copy .editorconfig from repository root
   - Create .gitignore
   - Create README.md from template

5. **Verify build**
   ```bash
   dotnet build {ORG}.{PROJECT}.sln
   ```

6. **Report results to user**

### Step 5: Success Response

Provide clear output to the user:

```
✓ Project framework created successfully!

Location: ./src/Contoso.OrderManagement/

Structure created:
✓ Contoso.OrderManagement.Domain
✓ Contoso.OrderManagement.Domain.Tests
✓ Contoso.OrderManagement.Application
✓ Contoso.OrderManagement.Application.Tests
✓ Contoso.OrderManagement.Infrastructure
✓ Contoso.OrderManagement.Infrastructure.Tests
✓ Contoso.OrderManagement.Service
✓ Contoso.OrderManagement.Service.Tests
✓ Contoso.OrderManagement.Service.Functions
✓ Contoso.OrderManagement.Service.Functions.Tests

Solution file: Contoso.OrderManagement.sln

Next steps:
1. cd ./src/Contoso.OrderManagement
2. Review README.md for getting started guide
3. Run 'dotnet build' to verify compilation
4. Start adding your domain entities in the Domain layer
5. Refer to AGENTS.md for coding guidelines

Build command: dotnet build Contoso.OrderManagement.sln
Test command: dotnet test Contoso.OrderManagement.sln

Happy coding! 🚀
```

## Token Replacement Map

When generating files from templates, replace:

| Token | Description | Example |
|-------|-------------|---------|
| `{{Org}}` | Organization prefix | Contoso, Acme |
| `{{ProjectName}}` | Project name | OrderManagement |
| `{{Layer}}` | Layer name | Domain, Application |
| `{{Folder}}` | Folder within layer | Commands, Entities |
| `{{FullProjectName}}` | Org.Project | Contoso.OrderManagement |

## Directory Structure to Create

Core projects (always created):
```
src/{Org}.{Project}/
├── {Org}.{Project}.Domain/
├── {Org}.{Project}.Domain.Tests/
├── {Org}.{Project}.Application/
├── {Org}.{Project}.Application.Tests/
├── {Org}.{Project}.Infrastructure/
├── {Org}.{Project}.Infrastructure.Tests/
├── {Org}.{Project}.Service/
└── {Org}.{Project}.Service.Tests/
```

Optional projects:
```
├── {Org}.{Project}.Service.Functions/ (if INCLUDE_FUNCTIONS)
├── {Org}.{Project}.Service.Functions.Tests/ (if INCLUDE_FUNCTIONS)
├── {Org}.{Project}.Service.Tests.Integration/ (if INCLUDE_INTEGRATION_TESTS)
└── {Org}.{Project}.Infrastructure.Tests.Integration/ (if INCLUDE_INTEGRATION_TESTS)
```

## Files to Generate

### Must Generate

1. **Solution file**: `{Org}.{Project}.sln`
2. **Project files**: `.csproj` for each project
3. **README.md**: Project documentation
4. **.gitignore**: Standard .NET gitignore
5. **InternalsVisibleTo.cs**: For each project

### Should Generate

1. **Application/Program.cs**: DI configuration
2. **Application/Result.cs**: Result pattern
3. **Application/Behaviors/ValidationBehavior.cs**: Validation pipeline
4. **Service/Program.cs**: API entry point
5. **Service/appsettings.json**: Configuration
6. **Functions/host.json**: Functions config (if applicable)
7. **Functions/local.settings.json**: Local settings (if applicable)

### Can Generate (Optional)

1. **Domain/Constants/Errors/GeneralErrors.cs**: Common errors
2. **.editorconfig**: Copy from repository
3. **Directory.Build.props**: Shared MSBuild props

## Error Handling

Handle these common errors:

| Error | Resolution |
|-------|-----------|
| Invalid project name | Prompt user for valid name (uppercase start, alphanumeric) |
| Directory already exists | Ask to overwrite or choose different location |
| Script execution fails | Check permissions, .NET SDK availability |
| Build fails after creation | Report specific errors, offer to fix |

## Validation Checklist

Before reporting success, verify:

- [ ] All project folders created
- [ ] All .csproj files valid XML
- [ ] Solution file includes all projects
- [ ] Projects reference correct dependencies
- [ ] `dotnet build` succeeds
- [ ] `dotnet test` discovers test projects
- [ ] README.md created with correct content
- [ ] .gitignore present

## Common Issues

### Issue: "dotnet: command not found"
**Solution**: Verify .NET SDK is installed and in PATH

### Issue: Script permission denied
**Solution**: 
```bash
chmod +x ./.claude/skills/ddd-project-framework-setup/scripts/create-project.sh
```

### Issue: Projects won't build
**Solution**: Check package references, ensure NuGet restore completed

### Issue: Empty folders not in git
**Solution**: .gitkeep files should be created in each empty folder

## References

- Full skill documentation: `SKILL.md`
- Folder structure reference: `references/folder-structure.md`
- Project templates: `templates/project-files.md`
- Code templates: `templates/code-templates.md`
- PowerShell script: `scripts/create-project.ps1`
- Bash script: `scripts/create-project.sh`

## Example Complete Session

```
User: "Create a new DDD project for me"

Agent: "I'll help you create a new DDD Clean Architecture project using the framework setup skill.

What is the name of your new project? (e.g., OrderManagement, CustomerPortal)"

User: "CustomerPortal"

Agent: "What organization prefix should be used? (default: Contoso)"

User: "Acme"

Agent: "Where should the solution be created? (default: ./src)"

User: "./services"

Agent: "Include Azure Functions project? (Y/n)"

User: "Y"

Agent: "Include integration test projects? (Y/n)"

User: "n"

Agent: [Executes create-project script]

Agent: "✓ Project framework created successfully!

Location: ./services/Acme.CustomerPortal/

[... full output as shown in Step 5 above ...]

Would you like me to help you create your first domain entity?"
```

## Tips for Agents

1. **Be conversational**: Don't just dump information, guide the user
2. **Validate early**: Check inputs before running scripts
3. **Provide context**: Explain what's being created and why
4. **Offer next steps**: Guide users on what to do after scaffolding
5. **Handle errors gracefully**: If something fails, help troubleshoot
6. **Use emojis sparingly**: Only for success/completion messages
7. **Be proactive**: Offer to help with next steps like creating first entity
