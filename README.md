# ASP.NET Web API 8.0.421

A RESTful Web API built with ASP.NET Core following clean architecture principles. Built as part of Formulatrix CS Bootcamp Batch 19.

---

## Tech Stack

| Technology | Purpose |
|---|---|
| ASP.NET Core 8 | Web API framework |
| Entity Framework Core | ORM |
| SQLite | Database |
| AutoMapper | Object mapping |
| FluentValidation | Request validation |
| ASP.NET Core Identity | User management |
| JWT Bearer | Authentication (stateless bearer token) |
| Swagger / OpenAPI | API documentation |

---

## Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [dotnet-ef CLI tool](https://learn.microsoft.com/en-us/ef/core/cli/dotnet)

Install the EF CLI tool globally if you haven't already:

```bash
dotnet tool install --global dotnet-ef
```

**Only required if you're not using the default SQLite provider** — install the database engine you intend to run against:

| Provider | Install |
|---|---|
| SQLite (default) | Nothing to install — it's a local file, ships via the `Microsoft.EntityFrameworkCore.Sqlite` package |
| PostgreSQL | [Download PostgreSQL for Windows](https://www.postgresql.org/download/windows/) (or `winget install PostgreSQL.PostgreSQL.16`, or run it in [Docker](https://www.docker.com/products/docker-desktop/)) |
| MySQL | [Download MySQL Community Server](https://dev.mysql.com/downloads/mysql/) (or `winget install Oracle.MySQL`, or run it in Docker) |

---

## Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/BukanRaychan/Formulatrix_CS_Bootcamp_Batch19.git
cd Formulatrix_CS_Bootcamp_Batch19
```

### 2. Restore dependencies

```bash
dotnet restore
```

### 3. Choose a database provider

The default provider is **SQLite** — it needs no setup, so you can skip straight to [step 4](#4-initialize-user-secrets) if that's what you're using.

To use **PostgreSQL** or **MySQL** instead, set `Database:Provider` and `ConnectionStrings:DefaultConnection` via User Secrets (see step 4 below) rather than editing `appsettings.json` directly, since the connection string usually contains a password.

<details>
<summary><strong>PostgreSQL setup</strong></summary>

1. Install PostgreSQL (see Requirements above) and make sure the server is running (default port `5432`).
2. Create a database and user matching whatever you'll put in your connection string, e.g. via `psql` or pgAdmin:
   ```sql
   CREATE DATABASE productcatalog;
   CREATE USER webapi WITH PASSWORD 'password';
   GRANT ALL PRIVILEGES ON DATABASE productcatalog TO webapi;
   ```
3. Connection string format (note: `Host`/`Username`, **not** `Server`/`User` — that's MySQL syntax and Npgsql will reject it):
   ```
   Host=localhost;Port=5432;Database=productcatalog;Username=webapi;Password=password
   ```

</details>

<details>
<summary><strong>MySQL setup</strong></summary>

1. Install MySQL (see Requirements above) and make sure the server is running (default port `3306`).
2. Create a database and user:
   ```sql
   CREATE DATABASE productcatalog;
   CREATE USER 'webapi'@'localhost' IDENTIFIED BY 'password';
   GRANT ALL PRIVILEGES ON productcatalog.* TO 'webapi'@'localhost';
   ```
3. Connection string format:
   ```
   Server=localhost;Port=3306;Database=productcatalog;User=webapi;Password=password
   ```

</details>

> **Migrations are provider-specific** — the committed `Migrations/` folder targets whatever provider it was last generated against. If you switch providers, see [Database Providers](#database-providers) below before running the next step.

### 4. Initialize User Secrets

[User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) keeps developer-specific values (real connection strings, JWT signing keys) out of source control. It's local-only and unencrypted — never use it for actual production secrets, but it's the right tool for local dev.

Run once per machine, from the `WebApi/` folder:

```bash
dotnet user-secrets init
```

This adds a `UserSecretsId` GUID to `WebApi.csproj` and creates an empty secrets store for this project outside the repo (`%APPDATA%\Microsoft\UserSecrets\<id>\secrets.json` on Windows).

If you're using the default SQLite provider, set at minimum a JWT key (the base `appsettings.json` ships with an empty `Jwt:Key` on purpose):

```bash
dotnet user-secrets set "Jwt:Key" "some-random-dev-only-secret-key-32chars+"
```

If you're using PostgreSQL or MySQL, also set the provider and connection string (values from step 3):

```bash
dotnet user-secrets set "Database:Provider" "Postgres"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=productcatalog;Username=webapi;Password=password"
```

Verify what's set at any time with:

```bash
dotnet user-secrets list
```

### 5. Apply migrations

```bash
dotnet ef database update
```

This creates the schema in whichever database your `Database:Provider`/connection string point to (`ProductCatalog.db` for the SQLite default).

### 6. Run the app

```bash
dotnet run
```

The app runs at `http://localhost:5280` by default.

### 7. Open Swagger UI

```
http://localhost:5280/swagger
```

---

## Project Structure

```
WebApi/
├── Configurations/       # EF Core IEntityTypeConfiguration<T> classes — column types,
│   └── ...                 max lengths, indexes, relationships, per entity
├── Controllers/           # HTTP endpoints, handles requests/responses only
│   └── ...
├── Data/                  # AppDbContext (EF Core) and startup seeding orchestration
│   └── Seeders/             # One ISeeder implementation per entity (idempotent — skips if data exists)
├── DTOs/                  # Data Transfer Objects — controls API input/output shape
│   ├── AuthDtos/            # Register/login request & response shapes
│   ├── Common/               # Cross-feature wrappers, e.g. ApiResponseDto<T>, PagedResponse<T>
│   ├── ProductDtos/          # Product create/update/response shapes
│   └── UnitProductDtos/      # UnitProduct create/update/response shapes
├── Exceptions/            # GlobalExceptionHandler — converts unhandled exceptions to ProblemDetails
│   └── ...
├── Migrations/            # EF Core auto-generated migration files (provider-specific, see below)
├── Models/                # Database entity classes (EF Core maps these to tables)
│   └── ...
├── Profiles/              # AutoMapper mapping profiles (Entity <-> DTO)
│   └── ...
├── Repositories/          # Database query layer — only talks to AppDbContext, no business logic
│   └── ...
├── Services/              # Business logic layer — orchestrates repositories, mapping, validation-adjacent rules
│   └── ...
├── Validators/            # FluentValidation rules, one file per DTO
│   ├── AuthValidators/
│   │   └── ...
│   └── ProductValidators/
│       └── ...
├── appsettings.json           # Shared defaults, committed — no real secrets
├── appsettings.Development.json
├── appsettings.Production.json
└── Program.cs              # Composition root: DI registration, middleware pipeline, DB provider switch
```

---

## Request Lifecycle

Every incoming request goes through these layers in order:

```
HTTP Request
    ↓
Global Exception Handler   (catches all unhandled errors)
    ↓
FluentValidation           (rejects invalid request body with 400)
    ↓
JWT Authentication         (rejects missing/invalid bearer token with 401)
    ↓
Controller                 (receives DTO, returns HTTP response)
    ↓
Service                    (business logic, maps DTOs ↔ Models)
    ↓
Repository                 (database queries only)
    ↓
AppDbContext               (EF Core → SQLite)
    ↓
Database
```



## Authentication

This API uses **JWT bearer** authentication. On register/login the server returns a signed JSON Web Token; the client stores it and sends it in the `Authorization` header on every protected request. The token is stateless — the server keeps no session.

### Step 1 — Register

```http
POST /api/Auth/register
Content-Type: application/json

{
  "firstName": "admin",
  "lastName": "utama",
  "email": "admin@example.com",
  "password": "password"
}
```

### Step 2 — Login

```http
POST /api/Auth/login
Content-Type: application/json

{
  "email": "admin@example.com",
  "password": "password"
}
```

Both register and login return a token plus basic user info (token valid for 24 hours):

```json
{
  "success": true,
  "message": "Login successful",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "email": "admin@example.com",
    "firstName": "admin",
    "lastName": "utama",
    "expiresAt": "2026-06-27T06:00:00Z"
  }
}
```

### Step 3 — Call protected endpoints

Send the token in the `Authorization` header on every protected request:

```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

- **Browser / frontend:** store the token (e.g. `localStorage`) and attach the header on each request.
- **Swagger UI:** click **Authorize**, paste `Bearer {your token}`, and protected endpoints become callable.

### Logout

JWT is stateless, so logout is handled entirely on the client by discarding the stored token — there is no server-side logout endpoint.

---

## API Response Format

All responses follow a consistent wrapper format:

### Success

```json
{
  "success": true,
  "message": "Products retrieved successfully",
  "data": { ... },
  "error": null
}
```

### Error

```json
{
  "success": false,
  "message": "Something went wrong",
  "data": null,
  "error": "Detailed error message"
}
```

---

## Database Seeding

The app automatically seeds sample product data on first run in the **Development** environment. Seeding is skipped if data already exists.

To reset and reseed from scratch:

```bash
dotnet ef database drop
dotnet ef database update
dotnet run
```

---

## Password Requirements

| Rule | Requirement |
|---|---|
| Minimum length | 6 characters |
| Requires digit | Yes |
| Requires uppercase | No |
| Requires non-alphanumeric | No |

---

## Configuration & Environment Variables

The app reads configuration from (later sources override earlier ones):

```
appsettings.json  →  appsettings.{Environment}.json  →  User Secrets (dev only)  →  Environment Variables
```

The active environment is set by `ASPNETCORE_ENVIRONMENT`. Locally it is `Development` (set in `Properties/launchSettings.json`); when deployed with nothing set, .NET defaults to `Production`.

Authentication is JWT-based: the API signs tokens with the symmetric `Jwt:Key`. `appsettings.json` ships a development key so the app runs out of the box; **in production, override `Jwt:Key` with a strong secret via an environment variable (or User Secrets locally) — never commit a real key.**

### Settings

| Config key | Description |
|---|---|
| `Jwt:Key` | Secret signing key (min 32 chars). Dev key in `appsettings.json`; override in production. |
| `Jwt:Issuer` | JWT issuer name |
| `Jwt:Audience` | JWT audience name |
| `Database:Provider` | Database engine: `Sqlite` (default, local dev), `MySql`, or `Postgres`/`PostgreSql`/`Pgsql` (case-insensitive) |
| `ConnectionStrings:DefaultConnection` | Database connection string (defaults to local SQLite) |
| `Cors:AllowedOrigins` | Array of frontend origins allowed by CORS |

### Production override (environment variables)

In production, supply secrets and per-environment values as OS/container environment variables. Nested keys use `__` (double underscore) and array elements use a numeric index:

```bash
ASPNETCORE_ENVIRONMENT=Production
Jwt__Key=<your-strong-32+char-secret>
Database__Provider=MySql
ConnectionStrings__DefaultConnection=<your-db-connection-string>
Cors__AllowedOrigins__0=https://your-frontend.example.com
```

A ready-made `appsettings.Production.json` template ships with the project; secrets (`Jwt:Key`, DB password) should still come from environment variables, not that file.

---

## Database Providers

The DB engine is selected at startup from `Database:Provider` (see [Program.cs](WebApi/Program.cs)'s provider switch), so the same code can run against any of the three with no code changes — only config:

| Provider | When | Connection string example |
|---|---|---|
| `Sqlite` | Local dev (default) | `Data Source=ProductCatalog.db` |
| `MySql` | Production | `Server=host;Port=3306;Database=productcatalog;User=appuser;Password=...` |
| `Postgres` | Production | `Host=host;Port=5432;Database=productcatalog;Username=appuser;Password=...` |

> **⚠️ Migrations are provider-specific.** The committed `Migrations/` folder is generated against exactly one provider at a time — column types, default-value SQL, and identity syntax differ between SQLite/MySQL/Postgres. If you switch providers, delete the `Migrations/` folder and regenerate against the new one:
>
> ```bash
> # with Database:Provider and ConnectionStrings:DefaultConnection (via User Secrets) pointing at the new provider
> rm -r Migrations
> dotnet ef migrations add InitialCreate
> dotnet ef database update
> ```
>
> The app also calls `Database.Migrate()` on startup, so once the migrations match the provider, the schema applies automatically on next run too.

### Removing providers you don't use

All three EF Core provider packages (`Microsoft.EntityFrameworkCore.Sqlite`, `Pomelo.EntityFrameworkCore.MySql`, `Npgsql.EntityFrameworkCore.PostgreSQL`) are referenced in [WebApi.csproj](WebApi/WebApi.csproj) so the boilerplate supports all three out of the box. If your project will only ever target one database, it's safe to remove the packages (and matching `case` branches in `Program.cs`'s provider switch) for the ones you don't need:

```bash
dotnet remove package Pomelo.EntityFrameworkCore.MySql
dotnet remove package Npgsql.EntityFrameworkCore.PostgreSQL
# keep only Microsoft.EntityFrameworkCore.Sqlite, for example
```

---

## Development Notes

- The `.db` file is excluded from git via `.gitignore` — each developer has their own local database
- Migrations are committed to git so all developers share the same schema
- Swagger is only enabled in the `Development` environment
- Database seeding only runs in the `Development` environment
