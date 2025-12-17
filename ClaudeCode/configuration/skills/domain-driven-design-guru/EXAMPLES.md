# Example Use Cases

This skill should be activated for requests such as:

## Code Review Requests

**Example 1: Command Review**
```
User: "Review this command for DDD compliance"
```
Skill analyzes:
- Layer placement (is it in Application layer?)
- CQRS compliance (does it modify state?)
- Command/Handler naming conventions
- Single responsibility
- Proper mapping patterns

**Example 2: Query Review**
```
User: "Is this query following best practices?"
```
Skill checks:
- Read-only operations
- No side effects
- Proper layer placement
- Efficient data retrieval

## Design Questions

**Example 3: Entity vs Value Object**
```
User: "Should this be an entity or a value object?"
```
Skill evaluates:
- Does it have identity?
- Does it need to be tracked?
- Is it immutable?
- Provides recommendation with rationale

**Example 4: Gateway Structure**
```
User: "How should I structure a new gateway?"
```
Skill provides:
- Interface placement (Domain layer)
- Implementation placement (Infrastructure layer)
- DTO structure for requests/responses
- Example from existing codebase

## Validation Placement

**Example 5: Validator Layer**
```
User: "Is my validator in the correct layer?"
```
Skill determines:
- Is it BasicValidator? → Should be in Service project
- Is it business logic? → Should be in Domain layer
- Migration path if incorrectly placed

**Example 6: Business Rules**
```
User: "Where should this validation logic go?"
```
Skill analyzes the rule type:
- Input validation → BasicValidator
- Business constraint → Domain layer
- Cross-aggregate rule → Domain Service

## Feature Design

**Example 7: New Feature Design**
```
User: "Design a new feature for tracking repair order history using DDD patterns"
```
Skill provides:
- Recommended entities/value objects
- Command and query structure
- Repository interfaces
- Layer breakdown
- Migration considerations

**Example 8: Refactoring**
```
User: "Help me refactor this code to align with DDD principles"
```
Skill offers:
- Current state assessment
- Target architecture alignment
- Incremental refactoring steps
- Code examples

## Mapping Questions

**Example 9: DTO Mapping**
```
User: "How should I map this DTO to a command?"
```
Skill explains:
- MapToCommand vs MapFrom patterns
- When to use each
- Example implementation

**Example 10: Response Mapping**
```
User: "Should I map from the entity or create a separate DTO?"
```
Skill considers:
- Layer separation
- Data exposure concerns
- Performance implications
- Best practice recommendation

## Architecture Questions

**Example 11: Project Structure**
```
User: "Which project should this code go in?"
```
Skill reviews:
- Code responsibility
- Layer boundaries
- Dependency flow
- Target architecture

**Example 12: Dependency Direction**
```
User: "Can my Domain layer reference Infrastructure?"
```
Skill explains:
- Dependency inversion principle
- Why Domain should not depend on Infrastructure
- How to fix with interfaces

## Naming and Organization

**Example 13: Naming Conventions**
```
User: "What should I name my command handler?"
```
Skill provides:
- Naming patterns (CommandName + Handler)
- Co-location requirements
- Examples from codebase

**Example 14: Vertical Slices**
```
User: "How should I organize files for a new feature?"
```
Skill recommends:
- Vertical slice organization by feature
- Folder structure
- File naming conventions
- Examples of good vertical slices in the codebase

## Migration Questions

**Example 15: Legacy Code**
```
User: "This code doesn't follow DDD. How do I migrate it?"
```
Skill provides:
- Current state assessment
- Pragmatic migration path
- Incremental steps
- Avoid big-bang rewrites

**Example 16: Generic Command Usage**
```
User: "Should I use the generic command pattern here?"
```
Skill advises:
- Prefer specific commands
- When generic might be acceptable
- Migration from generic to specific

## Common Scenarios

### Scenario: Creating a New Work Line

**User asks:** "I need to add a new type of work line. What's the DDD approach?"

**Skill provides:**
1. Command structure: `CreateWorkLineCommand`
2. Handler: `CreateWorkLineCommandHandler`
3. Domain model: WorkLine entity or value object?
4. Validation: Where business rules belong
5. Repository: Interface in Domain, implementation in Infrastructure
6. Vertical slice organization

### Scenario: Adding Validation

**User asks:** "I need to validate that a repair order can't be closed if it has open work lines"

**Skill analyzes:**
- This is a business rule, not input validation
- Belongs in Domain layer (target) or Application layer (current acceptable)
- Should be in RepairOrder entity or a domain service
- Provides code example

### Scenario: Querying Data

**User asks:** "I need to get all repair orders for a customer"

**Skill recommends:**
1. Create a Query: `GetRepairOrdersByCustomerQuery`
2. Query handler implementation
3. Repository method interface
4. Response DTO structure
5. Ensures read-only operation

### Scenario: External API Integration

**User asks:** "How do I integrate with an external parts catalog API?"

**Skill guides:**
1. Gateway interface in Domain
2. Gateway implementation in Infrastructure
3. Request/Response DTOs in Infrastructure
4. Dependency injection setup
5. Error handling patterns
