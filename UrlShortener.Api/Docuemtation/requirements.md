# Requirements

## Objective

Build a working URL Shortener prototype using AI-assisted software
engineering.

The system must demonstrate:

- Requirement understanding
- Task decomposition
- AI-assisted implementation
- Multi-step engineering execution
- Validation and testing
- Security and reliability
- Human engineering oversight
- Reviewable and maintainable output

AI assists within engineering tasks. The engineer owns correctness,
maintainability, security, validation and production readiness.

---

## Functional Requirements

### Create Short URL

**POST** `/api/urls`

Input:

```json
{
  "longUrl": "https://example.com"
}

Output:

{
  "shortCode": "aB12xZ",
  "shortUrl": "/aB12xZ"
}