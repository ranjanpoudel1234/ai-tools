# DDD Analysis Output Template

Use this exact structure for all architecture analyses:

## Architecture Analysis

[Current state assessment, alignment with target DDD, layer responsibilities]

## DDD Principle Checklist

- [x]/[ ] **Layer Separation**: [explanation]
- [x]/[ ] **CQRS Pattern**: [explanation]
- [x]/[ ] **Rich Domain Models**: [explanation]
- [x]/[ ]/[~] **Validation Placement**:
  - BasicValidator in Service Project and controllers
  - Business rules in Domain
- [x]/[ ] **Mapping Patterns**: [explanation]
- [x]/[ ] **Dependencies**: [explanation]
- [x]/[ ] **Naming Conventions**: [explanation]
- [x]/[ ] **Single Responsibility**: [explanation]
- [x]/[ ] **Value Objects**: [explanation]
- [x]/[ ] **Vertical Slices**: [explanation]
- [x]/[ ] **Domain Logic Location**: [Business rules in Core vs Processor?]

## Concrete Recommendations

1. **[Priority]**: [Recommendation]
   - Current: [code example]
   - Target: [code example]
   - Migration path: [steps]

2. **[Priority]**: [Recommendation]
   - Current: [code example]
   - Target: [code example]
   - Migration path: [steps]

## References

- Architecture Doc: docs/architecture/target-architecture.md
- Coding Guidelines: docs/architecture/coding-guidelines.md

---

## Checklist Legend

- [x] = Compliant with DDD principles
- [ ] = Not compliant / Needs improvement
- [~] = Partially compliant / In progress
