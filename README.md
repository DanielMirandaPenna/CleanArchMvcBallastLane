# CleanArchMvcBallastLane

CleanArchMvcBallastLane is an ASP.NET Core Web API for managing assignment tasks. The solution demonstrates Clean Architecture, CQRS with MediatR, Entity Framework Core with PostgreSQL, ASP.NET Core Identity, JWT authentication, Swagger/OpenAPI, and automated tests.

## Technology stack

- .NET 10
- ASP.NET Core Web API
- PostgreSQL 16
- Entity Framework Core and Npgsql
- ASP.NET Core Identity
- JWT Bearer authentication
- MediatR
- Swagger/OpenAPI with Swashbuckle
- xUnit, FluentAssertions, Moq, and Testcontainers

## Prerequisites

Install the following tools:

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/), Docker Engine with the Compose plugin, or an equivalent Docker installation
- Git

The API runs locally. Docker Compose is used only to provide the PostgreSQL database.

## Getting started

### 1. Clone the repository

```bash
git clone https://github.com/DanielMirandaPenna/CleanArchMvcBallastLane.git
cd CleanArchMvcBallastLane
```

### 2. Start PostgreSQL

From the repository root, start the database defined in `eng/docker-compose.yaml`:

```bash
docker compose -f eng/docker-compose.yaml up -d
```

The container uses the following development settings:

| Setting | Value |
| --- | --- |
| Container | `clean-arch-postgres` |
| Host | `localhost` |
| Port | `15433` |
| Database | `CleanArchMvcBallastLane` |
| Username | `postgres` |
| Password | `postgres` |

Check the container status with:

```bash
docker compose -f eng/docker-compose.yaml ps
```

To stop the container while preserving the database volume:

```bash
docker compose -f eng/docker-compose.yaml down
```

To stop the container and delete the database volume and its data:

```bash
docker compose -f eng/docker-compose.yaml down -v
```

### 3. Run the API

Restore dependencies and run the API project:

```bash
dotnet restore
dotnet run --project src/CleanArchMvcBallastLane.API/CleanArchMvcBallastLane.API.csproj
```

When running in the Development environment, Swagger is available at:

- [Swagger UI](https://localhost:7114/swagger)

The API applies all pending Entity Framework Core migrations automatically during startup. Make sure PostgreSQL is running before launching the API.

## Configuration

The default development connection string is stored in `src/CleanArchMvcBallastLane.API/appsettings.json` and matches the Docker Compose service:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "User ID=postgres;Password=postgres;Host=localhost;Port=15433;Database=CleanArchMvcBallastLane;Pooling=true;"
  }
}
```

For another database, override `ConnectionStrings:DefaultConnection` using `appsettings.Development.json`, environment variables, or another supported ASP.NET Core configuration provider. Do not commit production credentials, database passwords, or JWT signing keys to source control.

The JWT configuration contains the issuer, audience, signing key, and token lifetime settings used by the API. The values currently committed are development values only and must be replaced in a production environment.

## Architecture

The solution is divided into independent layers:

```text
src/
├── CleanArchMvcBallastLane.API          HTTP endpoints, middleware, models, and Swagger
├── CleanArchMvcBallastLane.Application  Commands, queries, handlers, and application contracts
├── CleanArchMvcBallastLane.Domain       Entities, enums, validation, and domain interfaces
├── CleanArchMvcBallastLane.Infra.Data   EF Core context, migrations, Identity, and repositories
└── CleanArchMvcBallastLane.Infra.IoC    Dependency injection, JWT, and Swagger registration
```

The API depends on the infrastructure composition root. Application code uses domain abstractions, while infrastructure supplies persistence and external framework implementations. Assignment task operations are dispatched through MediatR commands and queries.

## Authentication

All assignment task endpoints require a valid JWT. Create a user first:

```http
POST https://localhost:7114/api/Token/CreateUser
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "password123"
}
```

The password must be between 10 and 20 characters for the request model. Then authenticate:

```http
POST https://localhost:7114/api/Token/LoginUser
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "password123"
}
```

The login response contains a `token` and its `expiration`. In Swagger UI, select **Authorize** and enter the token using this format:

```text
Bearer <token>
```

The token lifetime is currently 10 minutes. Failed login attempts are subject to the configured Identity lockout policy.

## Assignment task API

Base URL: `https://localhost:7114/api/AssignmentTask`

Every endpoint in this section requires the `Authorization: Bearer <token>` header.

| Method | Route | Description |
| --- | --- | --- |
| `GET` | `/api/AssignmentTask?pageNumber=1&pageSize=10` | Return paginated assignment tasks |
| `GET` | `/api/AssignmentTask/{id}` | Return one task by ID |
| `GET` | `/api/AssignmentTask/{userId}/GetTaskByUser` | Return tasks created by a user |
| `GET` | `/api/AssignmentTask/GetTaskByStatus?status={statusId}` | Return tasks filtered by status |
| `POST` | `/api/AssignmentTask` | Create a task |
| `PUT` | `/api/AssignmentTask/{id}` | Update a task |
| `DELETE` | `/api/AssignmentTask/{id}` | Delete a task |

### Create a task

Every new task always starts with status `Pending` (1). The `status` field is not accepted in the create request body.

```http
POST https://localhost:7114/api/AssignmentTask
Content-Type: application/json

{
  "title": "Prepare release notes",
  "description": "Document the changes for the next release"
}
```

### Filter tasks by status

Pass the numeric status value as a query string parameter:

```http
GET https://localhost:7114/api/AssignmentTask/GetTaskByStatus?status=2
```

### Update a task

```http
PUT https://localhost:7114/api/AssignmentTask/1
Content-Type: application/json

{
  "title": "Prepare final release notes",
  "description": "Document and review the changes for the next release",
  "status": 2
}
```

Titles and descriptions must contain between 3 and 100 characters and between 3 and 200 characters, respectively. The authenticated user's identifier is assigned to `CreatedBy` by the API.

### Status values

| Numeric value | Name | Meaning |
| --- | --- | --- |
| `1` | `Pending` | Task has not started (initial state, set automatically on creation) |
| `2` | `InProgress` | Task is being worked on |
| `3` | `Completed` | Task is finished |

Status transitions follow a one-way state machine: `Pending → InProgress → Completed`. Use the `PUT` endpoint to advance the status.

### Error responses

| Scenario | HTTP status | Detail |
| --- | --- | --- |
| Task not found (GET, PUT, DELETE) | `400` | `Entity could not be found.` |
| Domain validation failure | `400` | Validation message |
| Unexpected error | `500` | Generic message |

A task response contains `id`, `title`, `description`, `status`, and `createdBy`. Authentication responses contain `token` and `expiration`. Invalid requests and domain validation failures are returned as structured error responses with HTTP 400; unexpected failures return HTTP 500.

## Database and migrations

The API uses `ApplicationDbContext`, PostgreSQL, and migrations stored in `src/CleanArchMvcBallastLane.Infra.Data/Migrations`. On startup, the API calls `Database.MigrateAsync()` and creates or updates the schema automatically.

To inspect or manage migrations with the EF Core CLI:

```bash
dotnet ef migrations list --project src/CleanArchMvcBallastLane.Infra.Data/CleanArchMvcBallastLane.Infra.Data.csproj --startup-project src/CleanArchMvcBallastLane.API/CleanArchMvcBallastLane.API.csproj
dotnet ef database update --project src/CleanArchMvcBallastLane.Infra.Data/CleanArchMvcBallastLane.Infra.Data.csproj --startup-project src/CleanArchMvcBallastLane.API/CleanArchMvcBallastLane.API.csproj
```

## Testing

Run all tests from the repository root:

```bash
dotnet test --no-restore
```

Run a specific test project:

```bash
dotnet test tests/CleanArchMvcBallastLane.Domain.Tests/CleanArchMvcBallastLane.Domain.Tests.csproj
dotnet test tests/CleanArchMvcBallastLane.Application.Tests/CleanArchMvcBallastLane.Application.Tests.csproj
dotnet test tests/CleanArchMvcBallastLane.API.Tests/CleanArchMvcBallastLane.API.Tests.csproj
```

The application integration tests use Testcontainers to start an isolated PostgreSQL 16 container. Docker must therefore be available when running those tests.

## Security notes

- The committed database credentials and JWT key are for local development only.
- Use secret management or environment variables for production configuration.
- Keep HTTPS enabled when accessing authenticated endpoints.
- Do not expose the PostgreSQL port or development secrets publicly.
- Identity lockout and JWT validation are configured in the infrastructure layer.
- Entity Framework Core parameterization and LINQ help reduce SQL injection risk, but application input must still be validated and dependencies kept up to date.

## License

This project is distributed under the terms of the [MIT License](LICENSE).
