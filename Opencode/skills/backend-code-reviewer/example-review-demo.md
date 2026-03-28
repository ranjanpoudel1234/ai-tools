# Code Reviewer Pro - Example Review Demo

## Sample Code (Before Review)

```csharp
using System;
using Microsoft.Azure.Functions.Worker;

namespace RepairOrder.Functions
{
    public class RepairOrderFunction
    {
        private IRepairOrderService _service;

        public RepairOrderFunction(IRepairOrderService service)
        {
            _service = service;
        }

        [Function("GetRepairOrder")]
        public async Task<IActionResult> GetRepairOrder(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "v1/repair-orders/{eroId}")]
            HttpRequest req,
            string eroId)
        {
            var result = await _service.GetByEroId(eroId);
            return new OkObjectResult(result);
        }
    }

    public class RepairOrderService : IRepairOrderService
    {
        private readonly IRepository _repo;

        public RepairOrderService(IRepository repo)
        {
            _repo = repo;
        }

        public async Task<RepairOrderModel> GetByEroId(string eroId)
        {
            var data = await _repo.GetAsync(eroId);
            return data;
        }
    }

    public class RepairOrderModel
    {
        public string EroId { get; set; }
        public string LocationNumber { get; set; }
        public string Status { get; set; }
        public DateTime LastModified { get; set; }
    }
}
```

---

## Code Reviewer Pro Feedback

### Architecture & Design

❌ **Critical: Layer Separation Violation**
The `IRepository` appears to be in the Application layer (same namespace as Function).
**Recommendation**: Move repository interface to Domain layer and implementation to Infrastructure layer for proper Clean Architecture layering.

❌ **Domain Model Encapsulation**
`RepairOrderModel` has public setters, allowing mutation from anywhere.
**Recommendation**: Use record type for immutability:
```csharp
public record RepairOrderModel(
    string EroId,
    string LocationNumber,
    string Status,
    DateTime LastModified);
```
This prevents mutation and helps encapsulate the domain.

---

### Dependency Injection

❌ **Missing Null Validation**
Constructor in `RepairOrderFunction` doesn't validate the injected dependency.
**Recommendation**: Use primary constructor with explicit null validation:
```csharp
public class RepairOrderFunction(IRepairOrderService service)
{
    private readonly IRepairOrderService _service =
        service.ValidateArgNotNull(nameof(service));
```

❌ **Service Lifetime Not Specified**
No indication of service registration lifetime in the code.
**Recommendation**: Register as Transient if stateless (most performant): "Transient is more performant and better for memory when dependencies are stateless."

---

### API Design

❌ **Critical: Potential Breaking Change**
Using only `eroId` in the route, but ERO IDs are not globally unique (they can be reused across locations).
**Recommendation**: Include locationNumber as required parameter:
```csharp
Route = "v1/repair-orders"
// Query string: ?eroId={eroId}&locationNumber={locationNumber}
```
This follows REST standards (resource as last URL segment) and prevents data consistency issues.

⚠️ **Future-Proofing**
Current route pattern doesn't allow for future CRUD operations on individual repair orders.
**Recommendation**: Reserve `/v1/repair-orders/{id}` pattern for when you transition to modern GUIDs. Use query strings for current lookups.

---

### Error Handling

❌ **Missing Validation**
No validation if `eroId` is null/empty.
**Recommendation**: Throw exception with meaningful message: "Recommend throwing exception if locationNumber is empty or null. The exception message should explain that the same eroId can be used by multiple locations."

❌ **Generic Error Handling**
No domain-specific exceptions.
**Recommendation**: Use `RepairOrderNotFoundException` from domain when not found instead of returning null.

❌ **Missing XML Documentation**
No XML comments explaining that eroId is non-optional and why locationNumber is critical.

---

### Code Quality

⚠️ **Namespace Structure**
Using traditional namespace with braces.
**Recommendation**: Use file-scoped namespace declaration:
```csharp
namespace RepairOrder.Functions;
```

⚠️ **Method Naming Ambiguity**
`GetByEroId` doesn't indicate that eroId alone may not be unique.
**Recommendation**: Rename to `GetByEroIdAndLocation` or similar to make the requirement explicit. "This method is getting repair order ID by eroId, but eroId isn't unique. Make that explicit in the name."

---

### Testing

⚠️ **Testability Concerns**
Without seeing tests, ensure:
- Test constructors validate all dependencies with null checks
- Use pascal case for test data (e.g., `TestRepairOrder1`)
- Setup dependencies in test constructor, not inline

---

### Security & Data Integrity

❌ **Critical: Data Uniqueness Issue**
Using non-unique identifier (eroId) as if it were unique.
**Recommendation**: "ERO IDs are not globally unique. Recommend requiring locationNumber for lookups to ensure correct data retrieval."

---

## Summary

- **3 Critical Issues**: Breaking change risk, data uniqueness, layer separation
- **5 High Priority**: Null validation, encapsulation, error handling, naming, XML docs
- **3 Medium Priority**: Modern C# patterns, testability, future-proofing

## Action Items

### Priority 1 (Before Merge):
1. Add locationNumber as required parameter to prevent data consistency issues
2. Fix layer separation (move repository to Infrastructure)
3. Add proper validation and domain exceptions

### Priority 2 (Should Fix):
4. Convert RepairOrderModel to record type for immutability
5. Add primary constructor with null validation
6. Improve method naming to reflect eroId+location requirement
7. Add XML documentation on critical parameters

### Priority 3 (Nice to Have):
8. Use file-scoped namespace
9. Plan for future CRUD operations on individual resources
10. Ensure test coverage includes dependency validation

---

## Positive Observations

✅ Clean separation of concerns between Function and Service
✅ Async/await pattern used correctly
✅ Constructor-based dependency injection

---

## The Benefit

By addressing these issues, you'll:
- Prevent production data retrieval bugs (wrong repair order returned)
- Maintain Clean Architecture principles for long-term maintainability
- Ensure API backward compatibility
- Make code intent clearer for the team
- Improve testability and reliability

**This gets us closer to the future!**

---

*Review generated using Code Reviewer Pro skill based on proven patterns from 20+ PRs*
