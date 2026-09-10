```markdown
# Brownfield Scenario

## Scenario

Assume the URL Shortener already exists and requires incremental
improvements without breaking existing behavior.

---

## Initial Analysis

Before modifying the system, inspect:

- Project structure
- Controllers
- Services
- Database model
- Existing tests
- Configuration
- Dependencies
- API contracts
- Authentication
- Error handling

The objective is to understand current behavior before making changes.

---

## Decomposition

1. Understand existing behavior
2. Review existing tests
3. Identify gaps
4. Define change boundaries
5. Implement one change at a time
6. Run regression tests
7. Review API compatibility
8. Validate security
9. Validate performance
10. Approve changes

---

## Example Improvement

Assume the existing application supports:

- URL creation
- Redirects

but lacks:

- URL validation
- Rate limiting
- Request-size protection
- Strong short-code generation

The improvement sequence would be:

```text
Existing API
     |
     v
URL Validation
     |
     v
Secure Short-Code Generation
     |
     v
Request-Size Protection
     |
     v
Rate Limiting
     |
     v
Regression Tests

