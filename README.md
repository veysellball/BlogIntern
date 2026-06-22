# BlogIntern API

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=flat&logo=csharp&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=flat&logo=microsoftsqlserver&logoColor=white)
![Redis](https://img.shields.io/badge/Redis-DC382D?style=flat&logo=redis&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-2496ED?style=flat&logo=docker&logoColor=white)
![Kubernetes](https://img.shields.io/badge/Kubernetes-326CE5?style=flat&logo=kubernetes&logoColor=white)

> A RESTful backend API built with ASP.NET Core 8, featuring JWT authentication, Redis caching, role-based authorization, and Kubernetes deployment support.

BlogIntern is a backend internship project demonstrating production-ready patterns: JWT access/refresh token flow, BCrypt password hashing, global exception handling, structured request/response logging with Serilog, and Redis-backed caching. The API is containerized with Docker and deployable to Kubernetes with a 3-replica setup.

## Table of Contents

- [About](#about)
- [Features](#features)
- [Tech Stack](#tech-stack)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
- [Usage](#usage)
- [API Endpoints](#api-endpoints)
- [Contributing](#contributing)
- [Authors](#authors)

## About

BlogIntern is designed as a learning-oriented backend project covering the full lifecycle of a secure ASP.NET Core Web API. It implements a user management system with role-based access control (Admin / User), a JWT authentication flow with refresh token rotation, and Redis integration for performance benchmarking. The project also includes Dockerfile and Kubernetes manifests for container-based deployments.

## Features

- **JWT Authentication** — Access tokens with configurable expiry and secure refresh token rotation (7-day TTL, single-use).
- **Role-Based Authorization** — Admin-only endpoints guarded via `[Authorize(Roles = "Admin")]`.
- **Redis Caching** — Side-by-side DB vs. cache endpoints to measure latency improvements.
- **Strategy Pattern Login** — Extensible `ILoginStrategy` interface; `EmailLoginStrategy` ships out of the box.
- **Global Exception Middleware** — Catches all unhandled exceptions and returns a consistent JSON error response.
- **Request/Response Logging** — Every HTTP request and response is logged via a custom middleware with Serilog.
- **Serilog Structured Logging** — Daily rolling log files retained for 7 days, plus console sink.
- **AutoMapper** — Clean DTO ↔ model mapping via `MappingProfile`.
- **Soft Delete & Reactivate** — Users can be deactivated without removing their database record.
- **Stored Procedure Support** — `GetUsersWithRoles` and `GetUsersByRole` via raw SQL / SP calls.
- **Docker & Kubernetes** — Multi-stage Dockerfile and `api-deployment.yaml` with 3 replicas.

## Tech Stack

- **Framework:** ASP.NET Core 8 (Web API)
- **Language:** C# (.NET 8)
- **Database:** SQL Server (EF Core 9, Code-First migrations)
- **Cache:** Redis (StackExchange.Redis)
- **Auth:** JWT Bearer (`Microsoft.AspNetCore.Authentication.JwtBearer`)
- **Logging:** Serilog (Console + File sinks)
- **Mapping:** AutoMapper 12
- **Password Hashing:** BCrypt.Net-Next
- **Documentation:** Swagger / Swashbuckle (Bearer token support)
- **Containerization:** Docker, Kubernetes

## Project Structure

```
BlogIntern/
├── Controllers/
│   ├── AuthController.cs          # Login, refresh, decode, whoami
│   ├── UserController.cs          # CRUD + soft-delete, role queries
│   ├── RedisController.cs         # DB vs. cache benchmark endpoints
│   ├── AsyncDemoController.cs     # Async pattern demos
│   └── TestController.cs          # Scratch / test endpoints
├── Data/
│   ├── AppDbContext.cs            # Primary EF Core context
│   └── AdventureContext.cs        # AdventureWorks read-only context
├── Dtos/                          # Request / response data transfer objects
├── Middlewares/
│   ├── GlobalExceptionMiddleware.cs
│   └── RequestResponseLoggingMiddleware.cs
├── Migrations/                    # EF Core migrations
├── Models/                        # Domain entities (User, Role, RefreshToken, …)
├── Services/
│   ├── Implements/                # LoginService, JwtTokenService, UserService, …
│   └── Interfaces/                # ILoginService, IUserService, ILoginStrategy
├── Mapping/MappingProfile.cs
├── Dockerfile
├── api-deployment.yaml            # Kubernetes Deployment (3 replicas)
├── api-service.yaml               # Kubernetes Service
└── appsettings.json
```

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- SQL Server (local or Docker)
- Redis (local or Docker)
- Docker + kubectl (optional, for container deployment)

### Installation

```bash
# 1. Clone the repository
git clone https://github.com/veyselbal/BlogIntern.git
cd BlogIntern

# 2. Configure connection strings and JWT secret
#    Edit BlogIntern/appsettings.json:
#    - ConnectionStrings:DefaultConnection  → your SQL Server instance
#    - Jwt:Secret                           → a strong random key (min 32 chars)

# 3. Apply database migrations
dotnet ef database update --project BlogIntern

# 4. Run the API
dotnet run --project BlogIntern
```

Swagger UI will be available at `http://localhost:<port>/swagger`.

### Running with Docker

```bash
cd BlogIntern

# Build the image
docker build -t blogintern-api:latest .

# Run with environment overrides
docker run -p 8080:8080 \
  -e ConnectionStrings__DefaultConnection="Server=<host>;Database=BlogDb;..." \
  -e Jwt__Secret="<your-secret>" \
  blogintern-api:latest
```

### Running with Kubernetes

```bash
kubectl apply -f BlogIntern/api-deployment.yaml
kubectl apply -f BlogIntern/api-service.yaml
```

The deployment starts 3 replicas. Use `GET /api/auth/whoami` to verify which pod handled a request.

## Usage

**1. Register a user** (requires an existing Admin account or direct DB insert for the first Admin).

**2. Login and obtain tokens:**

```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "yourpassword",
  "loginType": "email"
}
```

Response includes `accessToken` and `refreshToken`.

**3. Authenticate subsequent requests:**

```http
GET /api/user/profile
Authorization: Bearer <accessToken>
```

**4. Refresh an expired access token:**

```http
POST /api/auth/refresh
Content-Type: application/json

{ "refreshToken": "<refreshToken>" }
```

## API Endpoints

| Method | Path | Auth | Description |
|--------|------|------|-------------|
| POST | `/api/auth/login` | Public | Login, receive JWT + refresh token |
| POST | `/api/auth/refresh` | Public | Rotate refresh token, get new access token |
| POST | `/api/auth/decode` | Bearer | Decode and inspect the current JWT |
| GET | `/api/auth/whoami` | Public | Returns the pod/machine name |
| POST | `/api/user` | Admin | Create a new user |
| GET | `/api/user/profile` | Bearer | Get the authenticated user's profile |
| GET | `/api/user/all` | Admin | List all users |
| GET | `/api/user/by-id/{id}` | Admin | Get user by ID |
| GET | `/api/user/order-by-date` | Admin | Users ordered by creation date |
| DELETE | `/api/user/{id}` | Admin | Hard-delete a user |
| PATCH | `/api/user/soft-delete/{id}` | Admin | Deactivate a user |
| PATCH | `/api/user/reactivate/{id}` | Admin | Reactivate a deactivated user |
| GET | `/api/user/with-roles` | Public | Users with roles (stored procedure) |
| GET | `/api/user/users-by-role/{role}` | Public | Users filtered by role (stored procedure) |
| GET | `/api/redis/db` | Public | Fetch data directly from DB (with timing) |
| GET | `/api/redis/redis` | Public | Fetch data from Redis cache (with timing) |
| GET | `/api/redis/clear` | Public | Clear the Redis cache |

## Contributing

Contributions are welcome. Please open an issue first to discuss any significant change before submitting a pull request.

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/your-feature`)
3. Commit your changes (`git commit -m 'Add your feature'`)
4. Push to the branch (`git push origin feature/your-feature`)
5. Open a Pull Request

## Authors

- **veyselbal** — [GitHub](https://github.com/veyselbal)
