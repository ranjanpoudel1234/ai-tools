# References and Resources

## Repository Architecture Documents

### Primary References (Read These First)

- **Target Architecture**: `docs/architecture/target-architecture.md`
  - Definitive architectural guidance for the Rome Repair Order Service
  - Details the desired DDD/Clean Architecture state
  - Contains migration schedule and strategy

- **Coding Guidelines**: `docs/architecture/coding-guidelines.md`
  - Current coding standards and guidelines
  - Recommended flow through each layer
  - Validation patterns and best practices

- **Repository Overview**: `Claude.md` (repository root)
  - Present layout and project responsibilities
  - Current architecture overview
  - Project structure

### CQRS Pattern Examples

**Commands** (Alter State):
- Location: Application layer
- Define transactional boundaries
- Should NOT call other Commands
- Examples in the codebase:
  - CreateRepairOrderCommand
  - UpdateWorkLineCommand
  - DeleteSubletCommand

**Queries** (Retrieve State):
- Location: Application layer
- No transactions
- May be consumed by Commands or other Queries
- Examples in the codebase:
  - GetRepairOrderByIdQuery
  - ListWorkLinesQuery

## DDD Learning Resources

### Microsoft Documentation
- [Microservices Architecture DDD/CQRS Patterns](https://docs.microsoft.com/en-us/dotnet/standard/microservices-architecture/microservice-ddd-cqrs-patterns/)
- Official Microsoft guide to implementing DDD in .NET

### Core DDD Concepts

**Entities**:
- Objects with identity
- Track across their lifecycle
- Example: RepairOrder, WorkLine

**Value Objects**:
- Objects without identity
- Immutable
- Defined by their attributes
- Example: Address, Money, DateRange

**Aggregates**:
- Cluster of entities and value objects
- Have a root entity
- Enforce consistency boundaries
- Example: RepairOrder (root) with WorkLines

**Domain Services**:
- Operations that don't naturally fit in entities or value objects
- Stateless operations
- Coordinate between aggregates

**Repositories**:
- Abstraction for data persistence
- Domain defines interface
- Infrastructure implements
- Example: IRepairOrderRepository

## Target Architecture Diagram

```
┌─────────────────────────────────────┐
│  Presentation (Controllers/Functions)│
│  - Controllers, DTOs (Request/Response)
└────────────┬────────────────────────┘
             │
┌────────────▼────────────────────────┐
│  Application (Processor)            │
│  - Commands (modify data)           │
│  - Queries (read data)              │
│  - Business Validators              │
│  - Orchestration                    │
└────────────┬────────────────────────┘
             │
┌────────────▼────────────────────────┐
│  Domain (Core)                      │
│  - Entities (Order, etc)            │
│  - Value Objects (no identity)      │
│  - Domain Models                    │
│  - Domain Exceptions                │
│  - Gateway Interfaces               │
└─────────────────────────────────────┘

┌─────────────────────────────────────┐
│  Infrastructure                     │
│  - Gateways (External APIs)         │
│  - Repositories (Cosmos, Blob)      │
│  - Request/Response DTOs            │
└─────────────────────────────────────┘
```

## Validation Pattern

**BasicValidator**:
- Location: Service Project and Controllers (Presentation layer)
- Purpose: Input validation, format checking, null checks
- Examples: Required fields, string length, regex patterns

**Business Validators**:
- Location: Domain layer (Target) or Application layer (Current)
- Purpose: Business rule enforcement
- Examples: Order status transitions, date range validation, business constraints

**Value Objects** (Target):
- Encapsulate business rules
- Self-validating
- Throw domain exceptions on invalid state

## Common Patterns

### Mapping Patterns

**MapFrom** (Infrastructure → Application):
- Used when data flows FROM infrastructure/data layer TO application/domain
- Example: `MapFrom<CosmosRepairOrderDto>`

**MapToCommand** (Presentation → Application):
- Used when data flows FROM presentation TO command
- Example: `MapToCommand<CreateRepairOrderCommand>`

### Vertical Slices

Organize code by feature/capability rather than technical layer:
```
Application/
  WorkLine/
    Commands/
      CreateWorkLineCommand.cs
      CreateWorkLineCommandHandler.cs
      UpdateWorkLineCommand.cs
      UpdateWorkLineCommandHandler.cs
    Queries/
      GetWorkLineQuery.cs
      GetWorkLineQueryHandler.cs
```

## Migration Strategy

**Pragmatic Approach**:
1. Accept the repository's current state
2. Recognize migration is ongoing
3. Recommend incremental changes
4. Avoid large one-off rewrites
5. Provide stepwise migration plans
6. Reference good examples already in codebase

**When Assessing Code**:
- New code: Apply target patterns fully
- Existing code: Propose pragmatic migration path
- Don't criticize excessively
- Show clear migration steps
