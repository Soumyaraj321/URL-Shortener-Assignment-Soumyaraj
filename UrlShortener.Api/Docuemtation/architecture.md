# Architecture Overview

## System Architecture

The application uses a layered ASP.NET Core architecture.

```text
                    Browser / API Client
                            |
                            v
                    ASP.NET Core API
                            |
             +--------------+--------------+
             |                             |
             v                             v
        Controllers                 Middleware
             |
             v
      Application Services
             |
             v
      Entity Framework Core
             |
             v
           SQLite
```