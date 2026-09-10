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

The engineer remains responsible for reviewing and approving AI-assisted outputs, validating correctness, and making final engineering decisions.
---
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

## 3. Architecture

The application follows a layered architecture:

```text
Browser / API Client
        |
        v
Controllers
        |
        v
Application Services
        |
        v
EF Core / AppDbContext
        |
        v
SQLite Database
