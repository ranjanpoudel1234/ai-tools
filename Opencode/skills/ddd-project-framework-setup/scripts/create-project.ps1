#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Creates a DDD Clean Architecture project framework.

.DESCRIPTION
    Scaffolds a complete project structure with Domain, Application, Infrastructure, 
    and Presentation layers following Clean Architecture principles.

.PARAMETER ProjectName
    The name of the project (e.g., OrderManagement, CustomerPortal)

.PARAMETER OrgPrefix
    Organization prefix for namespaces (default: Contoso)

.PARAMETER TargetDir
    Directory where solution will be created (default: ./src)

.PARAMETER IncludeFunctions
    Include Azure Functions project (default: true)

.PARAMETER IncludeIntegrationTests
    Include integration test projects (default: false)

.EXAMPLE
    .\create-project.ps1 -ProjectName "OrderManagement" -OrgPrefix "Acme"

.EXAMPLE
    .\create-project.ps1 -ProjectName "CustomerPortal" -IncludeFunctions $false
#>

param(
    [Parameter(Mandatory=$true)]
    [string]$ProjectName,
    
    [Parameter(Mandatory=$false)]
    [string]$OrgPrefix = "Contoso",
    
    [Parameter(Mandatory=$false)]
    [string]$TargetDir = "./src",
    
    [Parameter(Mandatory=$false)]
    [bool]$IncludeFunctions = $true,
    
    [Parameter(Mandatory=$false)]
    [bool]$IncludeIntegrationTests = $false
)

$ErrorActionPreference = "Stop"

# Validate project name (must be valid C# identifier)
if ($ProjectName -notmatch '^[A-Z][A-Za-z0-9]*$') {
    Write-Error "Project name must be a valid C# identifier starting with uppercase letter"
    exit 1
}

$FullProjectName = "$OrgPrefix.$ProjectName"
$SolutionDir = Join-Path $TargetDir $FullProjectName
$SrcDir = $SolutionDir

Write-Host "Creating DDD Clean Architecture project: $FullProjectName" -ForegroundColor Green
Write-Host "Target directory: $SolutionDir" -ForegroundColor Cyan

# Create solution directory
New-Item -ItemType Directory -Path $SrcDir -Force | Out-Null

# Define project structure
$projects = @{
    "Domain" = @{
        Type = "classlib"
        Folders = @(
            "Constants/Errors",
            "Entities",
            "ValueObjects",
            "Enums",
            "Interfaces",
            "Exceptions",
            "Events",
            "Dtos",
            "Extensions",
            "Helpers",
            "Validation"
        )
    }
    "Domain.Tests" = @{
        Type = "xunit"
        Folders = @(
            "Constants",
            "Entities",
            "ValueObjects",
            "Exceptions",
            "Extensions",
            "Helpers",
            "Defaults"
        )
    }
    "Application" = @{
        Type = "classlib"
        Folders = @(
            "Commands",
            "Queries",
            "Validators",
            "Behaviors",
            "Dtos",
            "Services/Fakes",
            "Mapper",
            "CloudEvents/Contracts"
        )
    }
    "Application.Tests" = @{
        Type = "xunit"
        Folders = @(
            "Commands",
            "Queries",
            "Validators",
            "Services",
            "Behaviors",
            "CloudEvents",
            "Defaults",
            "TestHelpers"
        )
    }
    "Infrastructure" = @{
        Type = "classlib"
        Folders = @(
            "Constants",
            "Repositories",
            "Gateways",
            "EventHandlers",
            "EmsCloudEvents",
            "QueuePublishers",
            "OutgoingEvents",
            "HealthChecks",
            "Dtos",
            "Enums",
            "Exceptions",
            "Extensions",
            "Helpers",
            "Interfaces",
            "Mapper",
            "Startup",
            "Mocks/ServiceClients/Dtos",
            "Mocks/DelegatingHandlers"
        )
    }
    "Infrastructure.Tests" = @{
        Type = "xunit"
        Folders = @(
            "Repositories",
            "Gateways",
            "EventHandlers",
            "EmsCloudEvents",
            "HealthChecks",
            "Extensions",
            "Helpers",
            "Mocking"
        )
    }
    "Service" = @{
        Type = "web"
        Folders = @(
            "Controllers",
            "Middleware",
            "Filters",
            "ErrorHandling",
            "Extensions",
            "Dtos",
            "Mapper",
            "Helpers",
            "Constants",
            "Startup",
            "Properties"
        )
    }
    "Service.Tests" = @{
        Type = "xunit"
        Folders = @(
            "Controllers",
            "Middleware",
            "Filters",
            "Mapper",
            "Helpers"
        )
    }
}

if ($IncludeFunctions) {
    $projects["Service.Functions"] = @{
        Type = "function"
        Folders = @(
            "Functions/System",
            "Functions/ServiceBus",
            "Functions/StorageQueue",
            "EventHandlers",
            "Constants",
            "Extensions",
            "Middleware",
            "Wrappers"
        )
    }
    $projects["Service.Functions.Tests"] = @{
        Type = "xunit"
        Folders = @(
            "Functions",
            "EventHandlers",
            "Helpers",
            "Wrappers"
        )
    }
}

if ($IncludeIntegrationTests) {
    $projects["Service.Tests.Integration"] = @{
        Type = "xunit"
        Folders = @()
    }
    $projects["Infrastructure.Tests.Integration"] = @{
        Type = "xunit"
        Folders = @()
    }
}

# Create solution
Write-Host "`nCreating solution file..." -ForegroundColor Yellow
Set-Location $SrcDir
dotnet new sln -n $FullProjectName | Out-Null

# Create each project
foreach ($projName in $projects.Keys) {
    $fullProjName = "$FullProjectName.$projName"
    $projDir = Join-Path $SrcDir $fullProjName
    $projType = $projects[$projName].Type
    
    Write-Host "Creating project: $fullProjName" -ForegroundColor Cyan
    
    # Create project
    New-Item -ItemType Directory -Path $projDir -Force | Out-Null
    Set-Location $projDir
    
    if ($projType -eq "function") {
        dotnet new func -n $fullProjName --worker-runtime "dotnet-isolated" | Out-Null
    } else {
        dotnet new $projType -n $fullProjName | Out-Null
    }
    
    # Create folder structure
    foreach ($folder in $projects[$projName].Folders) {
        $folderPath = Join-Path $projDir $folder
        New-Item -ItemType Directory -Path $folderPath -Force | Out-Null
        
        # Create .gitkeep to preserve empty folders
        New-Item -ItemType File -Path (Join-Path $folderPath ".gitkeep") -Force | Out-Null
    }
    
    # Add project to solution
    Set-Location $SrcDir
    dotnet sln add $projDir | Out-Null
}

Write-Host "`nProject structure created successfully!" -ForegroundColor Green
Write-Host "`nNext steps:" -ForegroundColor Yellow
Write-Host "1. cd $SolutionDir"
Write-Host "2. Review and customize .csproj files with required packages"
Write-Host "3. Run 'dotnet build' to verify"
Write-Host "4. Start implementing your domain models in the Domain layer"
Write-Host "`nRefer to AGENTS.md for coding guidelines" -ForegroundColor Cyan
