---
name: code-reviewer
description: Reviews code for architecture, performance, API design, and maintainability. AUTOMATICALLY INVOKE when user mentions "review", "PR", "pull request", "code review", "backend review", or "analyze code". Verifies against latest Microsoft C# and .NET standards.
---

# Code Reviewer Pro Skill

**AUTOMATIC INVOCATION**: This skill should fire automatically when the user requests any form of code review, PR analysis, or code quality feedback.

Use this skill when reviewing code, analyzing pull requests, or providing code quality feedback. This skill embodies proven code review patterns focused on architecture, performance, API design, and maintainability.

## Communication Style
- **Concise but complete**: Sacrifice grammar if needed, never miss important details
- **Favor brevity**: Include all critical information without verbosity
- **Direct feedback**: Get to the point quickly while maintaining clarity

## When to Invoke (Automatic Triggers)

**Automatically invoke when user mentions ANY of these keywords:**
- "review" + "PR" / "pull request" / "code" / "backend" / "frontend"
- "analyze" + "PR" / "pull request" / "code"
- "code review"
- "PR review"
- "review PR #123" (any PR number)
- "check this PR"
- "feedback on PR"
- "evaluate" + "code" / "PR"

**Also invoke when:**
- User asks for architectural feedback on code
- User wants quality assessment of code changes
- User provides PR number and asks about it

## Review Philosophy

- **Constructive & Educational**: Explain the "why" behind suggestions, not just the "what"
- **Forward-Thinking**: Consider future maintenance, extensibility, and team scalability
- **Pragmatic**: Balance ideals with practical delivery constraints
- **Team-Oriented**: Use "we" language and frame feedback as collaborative improvement
- **Context-Aware**: Distinguish between critical issues and "nice-to-haves"
- **Standards-Based**: Verify against latest Microsoft docs and C# language standards

---

## Critical: Latest Standards Verification & Citation Requirements

**MANDATORY**: Every recommendation MUST include a citation to official documentation.

Before providing feedback:

1. **Check Microsoft Learn Documentation**
   - Search for latest C# language features and best practices
   - Verify .NET API recommendations for the target framework version
   - Check ASP.NET Core documentation for web API patterns
   - Review Azure SDK documentation for cloud integration patterns
   - Use `mcp__microsoft_docs_mcp__microsoft_docs_search` tool

2. **Check Microsoft Code Samples**
   - Search for official code examples demonstrating recommended patterns
   - Verify implementation approaches match Microsoft's latest guidance
   - Use `mcp__microsoft_code_sample_search` tool with language="csharp"

3. **Validate Against Standards**
   - C# language version features (latest stable release)
   - .NET framework version best practices
   - Official Microsoft coding conventions
   - Security and performance guidelines from Microsoft

**Example queries to run:**
```
- "C# primary constructors best practices"
- "ASP.NET Core dependency injection lifetimes"
- "Azure Service Bus client connection pooling"
- "C# record types immutability patterns"
- ".NET performance optimization techniques"
```

---

## Citation Requirements (MANDATORY)

**Every feedback item MUST include:**

1. **Specific Official Resource**: Link to the exact Microsoft Learn article, C# language spec, or official documentation
2. **Direct URL**: Include the full URL in your feedback
3. **Code Examples**: When available, reference official Microsoft code samples
4. **Version-Specific**: Cite documentation matching the project's framework version

**Required Citation Format:**
```
**Problem**: {Issue description}
**Fix**: {Specific solution}
**Standard**: {Standard name with URL}
  - Source: [Exact Article Title](https://full-url-to-microsoft-docs)
  - Example: [Official Code Sample](https://url-to-sample) (if available)
**Benefit**: {Why it matters}
```

**Example with Citation:**
```
**Problem**: Public setters on domain model allow external mutation
**Fix**: Use record type for immutability
**Standard**: C# Records for Immutable Data
  - Source: [Create record types - C# Language Reference](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/record)
  - Example: [Records with immutability patterns](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/records#positional-syntax-for-property-definition)
**Benefit**: Encapsulates domain, prevents unintended state changes
```

**Tools to Use for Citations:**
- `mcp__microsoft_docs_mcp__microsoft_docs_search` - Search Microsoft Learn
- `mcp__microsoft_code_sample_search` - Find official code examples
- `mcp__microsoft_docs_mcp__microsoft_docs_fetch` - Get full article content for complete context
- `WebSearch` - Find additional authoritative sources (C# spec, GitHub repos)

**Do NOT provide feedback without citations** - If you cannot find official documentation for a recommendation, state that explicitly and mark it as opinion-based.

---

## Output Format: PR Feedback File

Create feedback in file named: `PR-{PRNumber}-Feedback.md`

Example: `PR-12345-Feedback.md` for PR #12345

### File Structure:

```markdown
# PR #{PRNumber} Code Review

**Reviewed**: {Date}
**Reviewer**: Claude Code Review Agent
**Branch**: {branch-name}

---

## Executive Summary
[2-3 sentences: Overall assessment, critical items count, approval status]

---

## Critical Issues ([CRITICAL] Must Fix)
{If none: "None found"}

### Issue 1: {Brief Title}
**File**: `path/to/file.cs:LineNumber`
**Problem**: {Concise description}
**Impact**: {Why critical}
**Fix**: {Specific action}
**Standard**: {Microsoft doc reference if applicable}

---

## High Priority ([HIGH] Should Fix)
{If none: "None found"}

### Issue 1: {Brief Title}
**File**: `path/to/file.cs:LineNumber`
**Problem**: {Concise description}
**Recommendation**: {Specific action}
**Benefit**: {Why important}

---

## Recommendations ([RECOMMEND] Consider)
{If none: "None found"}

### 1. {Brief Title}
**File**: `path/to/file.cs:LineNumber`
**Suggestion**: {Concise description}
**Benefit**: {Why valuable}

---

## Positive Observations
{List good practices observed}

- [x] {Good practice 1}
- [x] {Good practice 2}

---

## Action Items Checklist
- [ ] {Critical item 1}
- [ ] {Critical item 2}
- [ ] {High priority item 1}
- [ ] {Recommendation 1}

---

## Standards Verified
- [x] C# {version} language features
- [x] .NET {version} framework patterns
- [x] Microsoft coding conventions
- [x] Azure SDK best practices (if applicable)

**References**:
- [Link to relevant Microsoft doc]
- [Link to C# standard]
```

---

## 10 Key Review Focus Areas

### 1. Architecture & Clean Code Principles

**Check for:**
- [x] Proper layer separation (Domain, Application, Infrastructure, Functions)
- [x] Domain model integrity and encapsulation
- [x] Use of **record types** for immutable data structures
- [x] Private setters to maintain encapsulation
- [x] Repository pattern placement (Infrastructure layer, not Application)
- [x] Proper dependency direction (Infrastructure → Application → Domain)

**Feedback Pattern:**
```
**Problem**: Public setters on domain model allow external mutation
**Fix**: Use record type for immutability
**Standard**: C# records designed for immutable data (MS Learn: Records)
**Benefit**: Encapsulates domain, prevents unintended state changes
```

**Red Flags:**
- Domain logic in Application or Infrastructure layers
- Public setters on domain models
- Repositories in Application layer
- Cross-layer tight coupling

---

### 2. Dependency Injection & Lifecycle Management

**Check for:**
- [x] **Primary constructors** with explicit dependency initialization
- [x] Null validation using `.ValidateArgNotNull(nameof(dependency))`
- [x] Appropriate service lifetimes (prefer Transient for stateless services)
- [x] Singleton for connection pooling (ServiceBusClient, HttpClient)
- [x] Underscore naming for private fields from dependencies

**Recommended Pattern (Verify with MS docs):**
```csharp
public class MyService(
    IDependency dependency,
    ITelemetryClient telemetryClient)
{
    private readonly IDependency _dependency =
        dependency.ValidateArgNotNull(nameof(dependency));
    private readonly ITelemetryClient _telemetryClient =
        telemetryClient.ValidateArgNotNull(nameof(telemetryClient));
}
```

**Feedback Pattern:**
```
**Problem**: Scoped lifetime used for stateless service
**Fix**: Change to Transient lifetime
**Standard**: MS recommends Transient for stateless services
**Benefit**: Better memory usage, improved performance
```

**Red Flags:**
- Missing null checks on dependencies
- Incorrect service lifetime choices (Scoped when Transient would work)
- Not using primary constructors for new code

---

### 3. Performance & Resource Management

**Check for:**
- [x] Caching strategy with appropriate expiration
- [x] Cache hit/miss telemetry for measuring effectiveness
- [x] Proper connection pooling for ServiceBus, HTTP clients
- [x] Memory optimization through appropriate service lifetimes
- [x] Avoiding unnecessary object retention

**Feedback Pattern:**
```
**Problem**: ServiceBusClient created per request
**Fix**: Register as Singleton with proper disposal
**Standard**: Azure SDK guidance on connection pooling
**Benefit**: Significant performance improvement, reduced connection overhead
```

**Red Flags:**
- Caching without telemetry/metrics
- ServiceBusClient created per request
- Unnecessary Singleton/Scoped when Transient would work
- Missing cache expiration strategy

---

### 4. API Design & REST Standards

**Check for:**
- [x] Breaking changes to existing endpoints
- [x] REST standard compliance (resource as last URL segment)
- [x] Query strings for filters instead of route parameters
- [x] Backward compatibility approach
- [x] Future-proofing for CRUD operations on resources

**Feedback Pattern:**
```
**Problem**: Filter as route parameter breaks REST standards
**Fix**: Move locationNumber to query string: `/v1/notes?repairOrderId=1234&locationNumber=123`
**Standard**: REST API design guidelines (MS Learn)
**Benefit**: Maintains backward compatibility, follows HTTP standards, reserves route for resource ID
```

**Red Flags:**
- Breaking changes without deprecation strategy
- Filters as route parameters when they should be query strings
- Ambiguous endpoint purposes
- Missing backward compatibility considerations

---

### 5. Error Handling & Validation

**Check for:**
- [x] Defensive validation with meaningful exceptions
- [x] Use of domain-specific exceptions (e.g., RepairOrderNotFoundException)
- [x] XML documentation on methods explaining non-optional parameters
- [x] Proper null/empty validation with context-specific error messages

**Feedback Pattern:**
```
**Problem**: No validation on critical parameter
**Fix**: Throw RepairOrderNotFoundException with descriptive message
**Standard**: .NET exception handling best practices
**Benefit**: Clear error messages, proper error propagation, better debugging
```

**Red Flags:**
- Silent failures or returning null instead of throwing
- Generic exceptions instead of domain exceptions
- Missing validation on critical parameters
- Unclear error messages

---

### 6. Testing Quality

**Check for:**
- [x] Null checks on dependencies in test class constructors
- [x] Explicit dependency setup in constructors (not inline in tests)
- [x] Selective telemetry testing (only for critical tracking)
- [x] Pascal case for test data (e.g., `TestWorkLine1`)
- [x] Default data creators for entities with private setters

**Feedback Pattern:**
```
**Problem**: Missing null validation in test constructor
**Fix**: Add `.ValidateArgNotNull()` for all test dependencies
**Benefit**: Catches test setup issues early, consistent with production code patterns
```

**Red Flags:**
- Missing dependency validation in test constructors
- Over-testing telemetry (testing every log statement)
- Inconsistent test data naming
- Difficult-to-construct test objects

---

### 7. Code Reusability & DRY Principle

**Check for:**
- [x] Constants for repeated strings (routes, queue names)
- [x] Enumerations for type safety instead of magic strings
- [x] Factory patterns for extensibility
- [x] Shared test utilities when used across projects
- [x] Base path constants for route configuration

**Feedback Pattern:**
```
**Problem**: Magic string repeated in 5 locations
**Fix**: Create const string or enum
**Benefit**: Single source of truth, easier refactoring, type safety
```

**Red Flags:**
- Magic strings repeated across files
- Repeated logic that could be factored out
- Missing factory pattern when multiple similar implementations exist
- Test utilities duplicated across test projects

---

### 8. Documentation & Knowledge Sharing

**Check for:**
- [x] Architecture Decision Records (ADRs) for significant choices
- [x] Inline documentation explaining "why" not just "what"
- [x] XML comments on public APIs
- [x] Context in PR descriptions
- [x] Comments explaining technical decisions

**Feedback Pattern:**
```
**Problem**: Complex technical decision without explanation
**Fix**: Add ADR or inline comment explaining rationale
**Benefit**: Knowledge transfer to team, easier maintenance later
```

**Red Flags:**
- Significant architectural changes without ADR
- Missing context on complex technical decisions
- No explanation for non-obvious patterns
- Undocumented public APIs

---

### 9. Security & Data Integrity

**Check for:**
- [x] Preventing stale data by fetching fresh values
- [x] Secrets management (no plain text keys)
- [x] Data consistency (globally unique identifiers)
- [x] Proper authentication/authorization
- [x] Input validation and sanitization

**Feedback Pattern:**
```
**Problem**: ERO IDs not globally unique, used as if they were
**Fix**: Require locationNumber for all ERO ID lookups
**Impact**: Data integrity issue - could retrieve wrong repair order
**Standard**: Database normalization principles
```

**Red Flags:**
- Passing potentially stale data instead of fetching fresh
- Secrets in code or config files
- Using non-unique identifiers as if they were unique
- Missing input validation on user-provided data

---

### 10. Modern C# Features & Conventions

**Check latest C# version features from Microsoft docs**

**Check for:**
- [x] **File-scoped namespaces** (inline namespace declarations)
- [x] **Primary constructors** for dependency injection
- [x] **Record types** as default for DTOs and immutable models
- [x] **Pattern matching** with switch expressions
- [x] Accurate, descriptive method names
- [x] Clear intent in naming (avoid ambiguous terms)

**Feedback Pattern:**
```
**Problem**: Traditional namespace braces in new code
**Fix**: Use file-scoped namespace: `namespace MyApp.Domain;`
**Standard**: C# 10+ file-scoped namespaces (MS Learn)
**Benefit**: Reduces indentation, modern C# convention
```

**Red Flags:**
- Traditional namespace braces in new code
- Not using primary constructors for dependency injection
- Class when record would be more appropriate
- Ambiguous or misleading method/class names

---

## Review Process Workflow

### Step 1: Gather Context
1. Identify PR number
2. Understand changes (files modified, purpose)
3. Note target framework version (.NET 6, 8, etc.)
4. Identify applicable domains (Web API, Azure, etc.)

### Step 2: Verify Standards
1. Search Microsoft Learn for relevant patterns
2. Check official C# language documentation
3. Review Azure SDK guidance if applicable
4. Find code samples demonstrating best practices

### Step 3: Analyze Code
Review systematically through 10 focus areas:
- Architecture & Clean Code
- Dependency Injection
- Performance
- API Design
- Error Handling
- Testing Quality
- Code Reusability
- Documentation
- Security
- Modern C# Features

### Step 4: Create Feedback File
1. Create `PR-{PRNumber}-Feedback.md`
2. Categorize issues: [CRITICAL], [HIGH], [RECOMMEND]
3. Include Microsoft doc references
4. List positive observations
5. Create action items checklist

### Step 5: Final Review
- Verify all critical issues identified
- Ensure concise but complete feedback
- Check Microsoft doc references included
- Validate action items are clear

---

## Feedback Writing Guidelines

### Concise Format:
```
**Problem**: {What's wrong in 1 sentence}
**Fix**: {Specific action in 1 sentence}
**Standard**: {Microsoft doc reference if applicable}
**Benefit**: {Why it matters in 1 sentence}
```

### Example (Good - Concise):
```
**Problem**: Public setters allow external mutation
**Fix**: Use record type
**Standard**: C# records for immutable data (MS Learn)
**Benefit**: Encapsulates domain, prevents unintended changes
```

### Example (Bad - Too Verbose):
```
**Problem**: I noticed that your domain model is using public setters on all of its properties. This is concerning because it allows any part of the application to modify the state of your domain objects, which could lead to unexpected behavior and makes it harder to track where changes are coming from. This violates the encapsulation principle that is fundamental to good object-oriented design...
[Too wordy - get to the point]
```

### Phrasing Patterns:
- "Recommend..." for suggestions
- "Consider..." for alternatives
- "Must fix..." for critical items
- "Out of scope but..." for pre-existing issues
- "Gets us closer to standard!" for progress

---

## Common Code Smells Detection

1. **Tight Coupling**: Inappropriate dependencies between layers
2. **Magic Strings**: Hard-coded strings that should be constants
3. **Breaking Changes**: API contract violations without migration path
4. **Memory Leaks**: Incorrect service lifetimes
5. **Stale Data**: Passing data that could become outdated
6. **Ambiguous Names**: Unclear naming that doesn't reveal intent
7. **Missing Validation**: No defensive checks on critical parameters
8. **Over-Engineering**: Complexity not needed for current requirements
9. **Under-Engineering**: Missing obvious extensibility points
10. **Inconsistent Patterns**: Not following established codebase conventions

---

## Quick Reference: Issue Prioritization

### [CRITICAL] (Must Fix Before Merge):
- Breaking changes to public APIs
- Security vulnerabilities (secrets, injection)
- Data integrity issues (stale data, uniqueness violations)
- Architecture violations breaking layer separation

### [HIGH] (Should Fix):
- Performance issues (missing caching, wrong lifetimes)
- Missing error handling on critical paths
- Unclear naming that will confuse team
- Missing tests for critical functionality

### [RECOMMEND] (Consider):
- Code reusability improvements (DRY violations)
- Modern C# pattern adoption
- Documentation gaps
- Test quality improvements

### [LOW] (Nice to Have):
- Minor naming improvements on private methods
- Additional test coverage on edge cases
- Documentation on straightforward code
- Style consistency (if not blocking)

---

## Final Notes

- **Always verify with Microsoft docs** - Don't rely on memory alone
- **Always explain "why"** - Help the team learn
- **Provide alternatives** - Don't just say what's wrong
- **Consider context** - Balance perfectionism with delivery needs
- **Be encouraging** - Recognize good work alongside suggestions
- **Be concise** - Get to the point quickly but completely
- **Think forward** - Consider maintenance, extensibility, team scalability

This skill represents patterns from 20+ PRs analyzed, updated with latest Microsoft standards verification requirement.
