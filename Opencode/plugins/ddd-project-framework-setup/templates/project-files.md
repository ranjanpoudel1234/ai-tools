# Project File Templates

These templates are used when creating .csproj files for each layer.

## Domain.csproj Template

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
    <PackageReference Include="MediatR" Version="12.4.0" />
    <!-- Add domain-specific packages as needed -->
  </ItemGroup>
</Project>
```

## Application.csproj Template

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
    <PackageReference Include="AutoMapper" Version="13.0.1" />
    <PackageReference Include="FluentValidation.AspNetCore" Version="11.3.0" />
    <PackageReference Include="MediatR" Version="12.4.0" />
  </ItemGroup>
  
  <ItemGroup>
    <ProjectReference Include="..\{{Org}}.{{ProjectName}}.Domain\{{Org}}.{{ProjectName}}.Domain.csproj" />
  </ItemGroup>
</Project>
```

## Infrastructure.csproj Template

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
    <PackageReference Include="AutoMapper" Version="13.0.1" />
    <PackageReference Include="FluentValidation.AspNetCore" Version="11.3.0" />
    <PackageReference Include="Polly" Version="8.4.1" />
    <PackageReference Include="Polly.Contrib.WaitAndRetry" Version="1.1.1" />
    <PackageReference Include="Microsoft.Extensions.Http.Polly" Version="8.0.7" />
    <!-- Add infrastructure packages: Azure SDKs, database clients, etc. -->
  </ItemGroup>
  
  <ItemGroup>
    <ProjectReference Include="..\{{Org}}.{{ProjectName}}.Domain\{{Org}}.{{ProjectName}}.Domain.csproj" />
  </ItemGroup>
</Project>
```

## Service.csproj Template (Web API)

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
    <UserSecretsId>{{ProjectName}}-secrets</UserSecretsId>
  </PropertyGroup>
  
  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.Mvc.NewtonsoftJson" Version="8.0.7" />
    <PackageReference Include="Swashbuckle.AspNetCore" Version="6.7.0" />
    <PackageReference Include="Swashbuckle.AspNetCore.Newtonsoft" Version="6.7.0" />
    <!-- Add presentation layer packages as needed -->
  </ItemGroup>
  
  <ItemGroup>
    <ProjectReference Include="..\{{Org}}.{{ProjectName}}.Application\{{Org}}.{{ProjectName}}.Application.csproj" />
    <ProjectReference Include="..\{{Org}}.{{ProjectName}}.Infrastructure\{{Org}}.{{ProjectName}}.Infrastructure.csproj" />
  </ItemGroup>
</Project>
```

## Test.csproj Template

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
    <PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="8.0.0" />
  </ItemGroup>
  
  <ItemGroup>
    <ProjectReference Include="..\{{Org}}.{{ProjectName}}.{{Layer}}\{{Org}}.{{ProjectName}}.{{Layer}}.csproj" />
  </ItemGroup>
  
  <ItemGroup>
    <AssemblyAttribute Include="System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute" />
  </ItemGroup>
</Project>
```

## Token Replacement

When generating files, replace these tokens:
- `{{Org}}` - Organization prefix (e.g., "Contoso", "Acme")
- `{{ProjectName}}` - Project name (e.g., "OrderManagement", "CustomerPortal")
- `{{Layer}}` - Layer name (e.g., "Domain", "Application", "Infrastructure", "Service")

Example:
- Template: `{{Org}}.{{ProjectName}}.{{Layer}}`
- Result: `Contoso.OrderManagement.Domain`
