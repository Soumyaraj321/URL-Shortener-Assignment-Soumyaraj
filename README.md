# URL-Shortener-Assignment-Soumyaraj
Developed an AI-Assisted Software Engineering System - URL Shortener

# AI-Assisted URL Shortener
A production-oriented URL shortener prototype built with **ASP.NET Core .NET 10**, **Entity Framework Core**, and **SQLite**.

The project demonstrates AI-assisted software engineering across requirement analysis, task decomposition, implementation, testing, security, validation, documentation, and engineering review.

---

## 1. Project Overview

The system provides:

- URL shortening
- Short-code generation
- URL redirection
- Click analytics
- Short URL lookup
- URL deactivation
- JWT authentication
- Role-based authorization
- Rate limiting
- Request-size protection
- Global exception handling
- Security response headers
- Swagger/OpenAPI documentation
- Unit and integration testing
- Simple browser-based UI

## 2. Technology Stack

| Area | Technology | 
|---|---|  
| Backend | ASP.NET Core .NET 10 |
| Language | C# |
| ORM | Entity Framework Core 10 |
| Database | SQLite |
| API Documentation | Swagger / OpenAPI |
| Authentication | JWT Bearer |
| Authorization | Role-based authorization |
| Testing | xUnit |
| Mocking | Moq |
| Integration Testing | ASP.NET Core WebApplicationFactory |
| Frontend | HTML, CSS, JavaScript |
| Version Control | Git / GitHub |

---

The application provides the following capabilities:

- Create a shortened URL
- Generate unique short codes
- Redirect users to the original URL
- Look up short URL information
- Track click analytics
- Deactivate short URLs
- JWT authentication
- Role-based authorization
- Rate limiting
- Request-size protection
- Global exception handling
- Security response headers
- Swagger/OpenAPI documentation
- Browser-based UI
- Unit tests
- Integration tests

### High-Level Flow

```text
User / Browser / API Client
          |
          v
     ASP.NET Core API
          |
          v
      Controllers
          |
          v
     Application Service
          |
          v
       EF Core
          |
          v
        SQLite
