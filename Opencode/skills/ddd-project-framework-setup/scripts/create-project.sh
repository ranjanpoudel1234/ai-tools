#!/bin/bash

# DDD Clean Architecture Project Framework Setup Script
# Creates a complete project structure with Domain, Application, Infrastructure, and Presentation layers

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

# Default values
ORG_PREFIX="CarMax"
TARGET_DIR="./src"
INCLUDE_FUNCTIONS=true
INCLUDE_INTEGRATION_TESTS=false

# Display usage
usage() {
    echo "Usage: $0 -p PROJECT_NAME [-o ORG_PREFIX] [-t TARGET_DIR] [-f] [-i]"
    echo ""
    echo "Options:"
    echo "  -p PROJECT_NAME              Project name (required, e.g., OrderManagement)"
    echo "  -o ORG_PREFIX                Organization prefix (default: CarMax)"
    echo "  -t TARGET_DIR                Target directory (default: ./src)"
    echo "  -f                          Skip Azure Functions project"
    echo "  -i                          Include integration test projects"
    echo "  -h                          Display this help message"
    echo ""
    echo "Example:"
    echo "  $0 -p OrderManagement -o Acme"
    exit 1
}

# Parse arguments
while getopts "p:o:t:fih" opt; do
    case $opt in
        p) PROJECT_NAME="$OPTARG" ;;
        o) ORG_PREFIX="$OPTARG" ;;
        t) TARGET_DIR="$OPTARG" ;;
        f) INCLUDE_FUNCTIONS=false ;;
        i) INCLUDE_INTEGRATION_TESTS=true ;;
        h) usage ;;
        *) usage ;;
    esac
done

# Validate required parameters
if [ -z "$PROJECT_NAME" ]; then
    echo -e "${RED}Error: Project name is required${NC}"
    usage
fi

# Validate project name (must start with uppercase letter)
if ! [[ "$PROJECT_NAME" =~ ^[A-Z][A-Za-z0-9]*$ ]]; then
    echo -e "${RED}Error: Project name must be a valid C# identifier starting with uppercase letter${NC}"
    exit 1
fi

FULL_PROJECT_NAME="${ORG_PREFIX}.${PROJECT_NAME}"
SOLUTION_DIR="${TARGET_DIR}/${FULL_PROJECT_NAME}"
SRC_DIR="$SOLUTION_DIR"

echo -e "${GREEN}Creating DDD Clean Architecture project: ${FULL_PROJECT_NAME}${NC}"
echo -e "${CYAN}Target directory: ${SOLUTION_DIR}${NC}"

# Create solution directory
mkdir -p "$SRC_DIR"

# Function to create project
create_project() {
    local proj_suffix=$1
    local proj_type=$2
    shift 2
    local folders=("$@")
    
    local full_proj_name="${FULL_PROJECT_NAME}.${proj_suffix}"
    local proj_dir="${SRC_DIR}/${full_proj_name}"
    
    echo -e "${CYAN}Creating project: ${full_proj_name}${NC}"
    
    # Create project directory
    mkdir -p "$proj_dir"
    cd "$proj_dir"
    
    # Create project file
    if [ "$proj_type" = "function" ]; then
        dotnet new func -n "$full_proj_name" --worker-runtime "dotnet-isolated" > /dev/null 2>&1 || true
    else
        dotnet new "$proj_type" -n "$full_proj_name" > /dev/null 2>&1 || true
    fi
    
    # Create folder structure
    for folder in "${folders[@]}"; do
        mkdir -p "${proj_dir}/${folder}"
        # Create .gitkeep to preserve empty folders in git
        touch "${proj_dir}/${folder}/.gitkeep"
    done
    
    # Add project to solution
    cd "$SRC_DIR"
    dotnet sln add "$proj_dir" > /dev/null 2>&1 || true
}

# Create solution file
echo -e "\n${YELLOW}Creating solution file...${NC}"
cd "$SRC_DIR"
dotnet new sln -n "$FULL_PROJECT_NAME" > /dev/null 2>&1

# Domain Layer
create_project "Domain" "classlib" \
    "Constants/Errors" \
    "Entities" \
    "ValueObjects" \
    "Enums" \
    "Interfaces" \
    "Exceptions" \
    "Events" \
    "Dtos" \
    "Extensions" \
    "Helpers" \
    "Validation"

create_project "Domain.Tests" "xunit" \
    "Constants" \
    "Entities" \
    "ValueObjects" \
    "Exceptions" \
    "Extensions" \
    "Helpers" \
    "Defaults"

# Application Layer
create_project "Application" "classlib" \
    "Commands" \
    "Queries" \
    "Validators" \
    "Behaviors" \
    "Dtos" \
    "Services/Fakes" \
    "Mapper" \
    "CloudEvents/Contracts"

create_project "Application.Tests" "xunit" \
    "Commands" \
    "Queries" \
    "Validators" \
    "Services" \
    "Behaviors" \
    "CloudEvents" \
    "Defaults" \
    "TestHelpers"

# Infrastructure Layer
create_project "Infrastructure" "classlib" \
    "Constants" \
    "Repositories" \
    "Gateways" \
    "EventHandlers" \
    "EmsCloudEvents" \
    "QueuePublishers" \
    "OutgoingEvents" \
    "HealthChecks" \
    "Dtos" \
    "Enums" \
    "Exceptions" \
    "Extensions" \
    "Helpers" \
    "Interfaces" \
    "Mapper" \
    "Startup" \
    "Mocks/ServiceClients/Dtos" \
    "Mocks/DelegatingHandlers"

create_project "Infrastructure.Tests" "xunit" \
    "Repositories" \
    "Gateways" \
    "EventHandlers" \
    "EmsCloudEvents" \
    "HealthChecks" \
    "Extensions" \
    "Helpers" \
    "Mocking"

# Presentation Layer
create_project "Service" "web" \
    "Controllers" \
    "Middleware" \
    "Filters" \
    "ErrorHandling" \
    "Extensions" \
    "Dtos" \
    "Mapper" \
    "Helpers" \
    "Constants" \
    "Startup" \
    "Properties"

create_project "Service.Tests" "xunit" \
    "Controllers" \
    "Middleware" \
    "Filters" \
    "Mapper" \
    "Helpers"

# Optional: Azure Functions
if [ "$INCLUDE_FUNCTIONS" = true ]; then
    create_project "Service.Functions" "function" \
        "Functions/System" \
        "Functions/ServiceBus" \
        "Functions/StorageQueue" \
        "EventHandlers" \
        "Constants" \
        "Extensions" \
        "Middleware" \
        "Wrappers"
    
    create_project "Service.Functions.Tests" "xunit" \
        "Functions" \
        "EventHandlers" \
        "Helpers" \
        "Wrappers"
fi

# Optional: Integration Tests
if [ "$INCLUDE_INTEGRATION_TESTS" = true ]; then
    create_project "Service.Tests.Integration" "xunit"
    create_project "Infrastructure.Tests.Integration" "xunit"
fi

# Create standard root-level folders and files
echo -e "${CYAN}Creating standard project structure...${NC}"

# Go to project root (parent of src)
PROJECT_ROOT="$(dirname "$SOLUTION_DIR")"
cd "$PROJECT_ROOT"

# Create standard folders
mkdir -p .github .pipelines .vscode docs

# Create .gitignore
cat > ".gitignore" << 'GITIGNORE'
## Ignore Visual Studio temporary files, build results, and
## files generated by popular Visual Studio add-ons.

# User-specific files
*.suo
*.user
*.userosscache
*.sln.docstates

# Build results
[Dd]ebug/
[Dd]ebugPublic/
[Rr]elease/
[Rr]eleases/
x64/
x86/
[Bb]in/
[Oo]bj/

# Visual Studio cache/options directory
.vs/

# MSTest test Results
[Tt]est[Rr]esult*/
[Bb]uild[Ll]og.*

# NuGet Packages
*.nupkg
**/packages/*
!**/packages/build/

# Visual Studio cache files
*.cache
project.lock.json
project.fragment.lock.json
artifacts/

# Rider
.idea/

# VS Code
.vscode/*
!.vscode/settings.json
!.vscode/tasks.json
!.vscode/launch.json
!.vscode/extensions.json

# Azure Functions
local.settings.json
GITIGNORE

# Create .gitattributes
cat > ".gitattributes" << 'GITATTRIBUTES'
###############################################################################
# Set default behavior to automatically normalize line endings.
###############################################################################
* text=auto

###############################################################################
# Set default behavior for command prompt diff.
###############################################################################
*.cs     diff=csharp
*.vb     diff=vbnet

###############################################################################
# Set the merge driver for project and solution files
###############################################################################
*.sln        merge=union
*.csproj     merge=union
*.vbproj     merge=union
GITATTRIBUTES

# Create basic .editorconfig (simplified version)
cat > ".editorconfig" << 'EDITORCONFIG'
root = true

# All files
[*]
charset = utf-8
insert_final_newline = true
trim_trailing_whitespace = true

# Code files
[*.{cs,csx,vb,vbx}]
indent_size = 4
indent_style = space

# C# files
[*.cs]
# Use file-scoped namespaces
csharp_style_namespace_declarations = file_scoped:warning

# Naming conventions
dotnet_naming_rule.private_fields_with_underscore.symbols = private_fields
dotnet_naming_rule.private_fields_with_underscore.style = prefix_underscore
dotnet_naming_rule.private_fields_with_underscore.severity = warning

dotnet_naming_symbols.private_fields.applicable_kinds = field
dotnet_naming_symbols.private_fields.applicable_accessibilities = private

dotnet_naming_style.prefix_underscore.capitalization = camel_case
dotnet_naming_style.prefix_underscore.required_prefix = _
EDITORCONFIG

# Create .github/copilot-instructions.md
cat > ".github/copilot-instructions.md" << 'COPILOT'
# GitHub Copilot Instructions

## Code Style
- Use file-scoped namespaces
- Private fields with underscore prefix (_fieldName)
- Follow SOLID principles
- Prefer var for local variables

## Architecture
- Follow Clean Architecture layering
- Domain has NO external dependencies
- Use CQRS pattern with MediatR
- Use Result pattern for operations

## Testing
- Use xUnit, FluentAssertions, NSubstitute
- Follow Arrange-Act-Assert pattern
- One assertion per test
- Test naming: MethodName_StateUnderTest_ExpectedBehavior
COPILOT

# Create docs/README.md
cat > "docs/README.md" << 'DOCSREADME'
# Documentation

This folder contains project documentation including:

- Architecture documentation
- Architecture Decision Records (ADRs)
- Diagrams and design documents
- API documentation

## Recommended Structure

```
docs/
├── architecture.md           # High-level architecture
├── adr/                      # Architecture Decision Records
│   └── 001-use-clean-architecture.md
└── diagrams/                 # Architecture diagrams
    └── context-diagram.puml
```
DOCSREADME

echo -e "${GREEN}✓ Standard project structure created${NC}"

# Create README
cat > "${SOLUTION_DIR}/README.md" << EOF
# ${FULL_PROJECT_NAME}

Clean Architecture project following DDD principles.

## Architecture

This project follows Clean Architecture with clear separation of concerns:

\`\`\`
${FULL_PROJECT_NAME}.Service (Presentation)
    ↓ depends on
${FULL_PROJECT_NAME}.Infrastructure (Data Access & External Services)
    ↓ depends on
${FULL_PROJECT_NAME}.Application (Use Cases & Business Logic)
    ↓ depends on
${FULL_PROJECT_NAME}.Domain (Core Business Domain)
\`\`\`

## Project Structure

- **Domain**: Core business entities, value objects, enums, interfaces
- **Application**: Use cases, commands, queries, validators (CQRS with MediatR)
- **Infrastructure**: Repositories, gateways, external services, data access
- **Service**: API controllers, middleware, filters, presentation logic

## Build

\`\`\`bash
dotnet build ${FULL_PROJECT_NAME}.sln
\`\`\`

## Test

\`\`\`bash
dotnet test ${FULL_PROJECT_NAME}.sln
\`\`\`

## Next Steps

1. Define your domain entities in the Domain layer
2. Create commands and queries in the Application layer
3. Implement repositories in the Infrastructure layer
4. Add controllers in the Service layer
5. Refer to AGENTS.md for coding guidelines and conventions

## Documentation

- See AGENTS.md for development guidelines
- Follow .editorconfig for code style
- Use file-scoped namespaces
- All projects have TreatWarningsAsErrors enabled
EOF

echo -e "\n${GREEN}✓ Project structure created successfully!${NC}"
echo -e "\n${YELLOW}Location:${NC} ${SOLUTION_DIR}"
echo -e "\n${YELLOW}Projects created:${NC}"
echo "  ✓ ${FULL_PROJECT_NAME}.Domain"
echo "  ✓ ${FULL_PROJECT_NAME}.Domain.Tests"
echo "  ✓ ${FULL_PROJECT_NAME}.Application"
echo "  ✓ ${FULL_PROJECT_NAME}.Application.Tests"
echo "  ✓ ${FULL_PROJECT_NAME}.Infrastructure"
echo "  ✓ ${FULL_PROJECT_NAME}.Infrastructure.Tests"
echo "  ✓ ${FULL_PROJECT_NAME}.Service"
echo "  ✓ ${FULL_PROJECT_NAME}.Service.Tests"

if [ "$INCLUDE_FUNCTIONS" = true ]; then
    echo "  ✓ ${FULL_PROJECT_NAME}.Service.Functions"
    echo "  ✓ ${FULL_PROJECT_NAME}.Service.Functions.Tests"
fi

if [ "$INCLUDE_INTEGRATION_TESTS" = true ]; then
    echo "  ✓ ${FULL_PROJECT_NAME}.Service.Tests.Integration"
    echo "  ✓ ${FULL_PROJECT_NAME}.Infrastructure.Tests.Integration"
fi

echo -e "\n${YELLOW}Next steps:${NC}"
echo "1. cd ${SOLUTION_DIR}"
echo "2. Review and customize .csproj files with required packages"
echo "3. Run 'dotnet build' to verify"
echo "4. Start implementing your domain models in the Domain layer"
echo -e "\n${CYAN}Refer to AGENTS.md for coding guidelines${NC}"
