# Testing Approach, Limitations, and Trade-offs

## 1. Testing Objective

The testing strategy validates the URL shortener at multiple levels:

1. Unit testing
2. API integration testing
3. Negative-path testing
4. Manual UI validation
5. Security and reliability validation

The goal is to verify both individual components and the behavior of the complete API.

---

# 2. Unit Testing

Unit tests focus on business logic and individual services without requiring the application to run as a complete HTTP application.

## Components Covered

### URL Validator

Tests verify:

- Valid HTTP URLs are accepted.
- Valid HTTPS URLs are accepted.
- Invalid URLs are rejected.
- Unsupported schemes such as FTP are rejected.
- JavaScript URLs are rejected.

# 3. Test Structure

```text
Unit Tests
├── URL validation
├── Short-code generation
└── URL shortener service
    ├── Create
    ├── Invalid URL
    ├── Collision handling
    ├── Lookup
    ├── Redirect
    ├── Unknown URL
    ├── Inactive URL
    ├── Analytics
    └── Deactivation

Integration Tests
├── Create API
├── Lookup API
├── Redirect + analytics
├── Deactivation
├── Unknown URL
└── Invalid URL
```