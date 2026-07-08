# ASP.NET Web API 8.0.421

A RESTful Web API built with ASP.NET Core, following clean architecture principles. Built as part of Formulatrix CS Bootcamp Batch 19.

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
| JWT Bearer | Stateless bearer token authentication |
| Swagger / OpenAPI | API documentation |

---

## Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [dotnet-ef CLI tool](https://learn.microsoft.com/en-us/ef/core/cli/dotnet)

Install the EF CLI tool globally:

```bash
dotnet tool install --global dotnet-ef
```

**Only needed if you're switching away from the default SQLite provider** — install the database engine you want to run against:

| Provider | How to install |
|---|---|
| SQLite (default) | Nothing to install — it ships via the `Microsoft.EntityFrameworkCore.Sqlite` package as a local file |
| PostgreSQL | [Download for Windows](https://www.postgresql.org/download/windows/), or run `winget install PostgreSQL.PostgreSQL.16`, or spin it up in [Docker](https://www.docker.com/products/docker-desktop/) |
| MySQL | [Download MySQL Community Server](https://dev.mysql.com/downloads/mysql/), or run `winget install Oracle.MySQL`, or use Docker |

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

The default is **SQLite** — no setup needed, skip straight to [step 4](#4-initialize-user-secrets).

To use **PostgreSQL** or **MySQL**, set `Database:Provider` and `ConnectionStrings:DefaultConnection` via User Secrets (step 4) rather than editing `appsettings.json` directly. Connection strings usually contain passwords, so keep them out of source control.

<details>
<summary><strong>PostgreSQL setup</strong></summary>

1. Install PostgreSQL (see Requirements) and confirm the server is running on the default port `5432`.
2. Create a database and user via `psql` or pgAdmin:
   ```sql
   CREATE DATABASE productcatalog;
   CREATE USER webapi WITH PASSWORD 'password';
   GRANT ALL PRIVILEGES ON DATABASE productcatalog TO webapi;
   ```
3. Your connection string will look like this. Note: use `Host`/`Username`, not `Server`/`User` — Npgsql will reject MySQL-style syntax:
   ```
   Host=localhost;Port=5432;Database=productcatalog;Username=webapi;Password=password
   ```

</details>

<details>
<summary><strong>MySQL setup</strong></summary>

1. Install MySQL (see Requirements) and confirm the server is running on the default port `3306`.
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

> **Migrations are provider-specific.** The committed `Migrations/` folder targets whichever provider it was last generated against (the current provider is PostgreSQL) — if you switch, see [Database Providers](#database-providers) before running the next step.

### 4. Initialize User Secrets

[User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) keeps sensitive local values — real connection strings, JWT signing keys — out of source control. It stores everything outside the repo in an unencrypted local file. Fine for local dev, not for production secrets.

Run this once from the `WebApi/` folder:

```bash
dotnet user-secrets init
```

This adds a `UserSecretsId` GUID to `WebApi.csproj` and creates an empty secrets store at `%APPDATA%\Microsoft\UserSecrets\<id>\secrets.json` on Windows.

**If you're using SQLite (default),** set a JWT key at minimum — `appsettings.json` ships with an empty `Jwt:Key` on purpose:

```bash
dotnet user-secrets set "Jwt:Key" "some-random-dev-only-secret-key-32chars+"
```

**If you're using PostgreSQL or MySQL,** also set the provider and connection string from step 3:

```bash
# For PostgreSQL
dotnet user-secrets set "Database:Provider" "Postgres"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=productcatalog;Username=webapi;Password=password"

# For MySQL
dotnet user-secrets set "Database:Provider" "MySql"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Port=3306;Database=productcatalog;User=webapi;Password=password"
```

Check what's currently set with:

```bash
dotnet user-secrets list
```

### 5. Apply migrations

```bash
dotnet ef database update
```

This creates the schema in whichever database your `Database:Provider` and connection string point to. For SQLite, it creates `ProductCatalog.db` in the project folder.

### 6. Run the app

```bash
dotnet run
```

Runs at `http://localhost:5280` by default.

### 7. Open Swagger UI

```
http://localhost:5280/swagger
```

---

## Project Structure

```
WebApi/
├── Configurations/       # EF Core IEntityTypeConfiguration<T> classes — column types,
│   └── ...                 max lengths, indexes, and relationships per entity
├── Controllers/          # HTTP endpoints — handles requests and responses only
│   └── ...
├── Data/                 # AppDbContext (EF Core) and startup seeding orchestration
│   └── Seeders/          # One ISeeder per entity — idempotent, skips if data already exists
├── DTOs/                 # Data Transfer Objects — controls what the API accepts and returns
│   ├── AuthDtos/         # Register/login request and response shapes
│   ├── Common/           # Shared wrappers: ApiResponseDto<T>, PagedResponse<T>
│   ├── ProductDtos/      # Product create/update/response shapes
│   └── UnitProductDtos/  # UnitProduct create/update/response shapes
├── Exceptions/           # GlobalExceptionHandler — converts unhandled exceptions to ProblemDetails
│   └── ...
├── Migrations/           # EF Core auto-generated files — provider-specific, see Database Providers
├── Models/               # Entity classes that EF Core maps to database tables
│   └── ...
├── Profiles/             # AutoMapper profiles — maps between entities and DTOs
│   └── ...
├── Repositories/         # Database query layer — talks only to AppDbContext, no business logic here
│   └── ...
├── Services/             # Business logic — orchestrates repositories, mapping, and validation rules
│   └── ...
├── Validators/           # FluentValidation rules, one file per DTO
│   ├── AuthValidators/
│   └── ProductValidators/
├── appsettings.json                # Shared defaults — committed, no real secrets
├── appsettings.Development.json
├── appsettings.Production.json
└── Program.cs                      # Composition root — DI registration, middleware pipeline, DB provider switch
```

---

## Request Lifecycle

Every request passes through these layers in order:

```
HTTP Request
    ↓
Global Exception Handler   (catches all unhandled errors)
    ↓
JWT Authentication         (rejects missing/invalid bearer token with 401)
    ↓
FluentValidation           (rejects invalid request body with 400)
    ↓
Controller                 (receives DTO, returns HTTP response)
    ↓
Service                    (business logic, maps DTOs ↔ Models)
    ↓
Repository                 (database queries only)
    ↓
AppDbContext               (EF Core → SQLite / MySQL / PostgreSQL)
    ↓
Database
```

---

## Authentication

The API uses **JWT bearer** authentication. Register or log in, get a signed token back, then send it in the `Authorization` header on every protected request. The server keeps no session state.

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

Both endpoints return a token and basic user info (valid for 24 hours):

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

```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

- **Frontend:** store the token (e.g. `localStorage`) and attach the header on each request.
- **Swagger UI:** click **Authorize**, paste `Bearer {your token}`, and protected endpoints become callable.

### Logout

JWT is stateless — logout is handled on the client by discarding the stored token. There is no server-side logout endpoint.

---

## API Response Format

All responses use the same wrapper:

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

The app seeds sample product data on first run in the **Development** environment. Seeding skips if data already exists.

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

## Configuration

The app reads configuration from these sources in order — later entries override earlier ones:

```
appsettings.json  →  appsettings.{Environment}.json  →  User Secrets (dev only)  →  Environment Variables
```

The active environment comes from `ASPNETCORE_ENVIRONMENT`. Locally it defaults to `Development` via `Properties/launchSettings.json`; on a server with nothing set, .NET falls back to `Production`.

`appsettings.json` ships with a development JWT key so the app runs out of the box. **In production, override `Jwt:Key` with a strong secret via an environment variable — never commit a real key.**

### Config keys

| Key | Description |
|---|---|
| `Jwt:Key` | Signing key, minimum 32 characters. Dev key ships in `appsettings.json`; override in production. |
| `Jwt:Issuer` | JWT issuer name |
| `Jwt:Audience` | JWT audience name |
| `Database:Provider` | `Sqlite` (default), `MySql`, or `Postgres` / `PostgreSql` / `Pgsql` (case-insensitive) |
| `ConnectionStrings:DefaultConnection` | Database connection string — defaults to local SQLite |
| `Cors:AllowedOrigins` | Array of frontend origins allowed by CORS |

### Production environment variables

Use `__` (double underscore) for nested keys, and a numeric index for array elements:

```bash
ASPNETCORE_ENVIRONMENT=Production
Jwt__Key=<your-strong-32+char-secret>
Database__Provider=MySql
ConnectionStrings__DefaultConnection=<your-db-connection-string>
Cors__AllowedOrigins__0=https://your-frontend.example.com
```

A `appsettings.Production.json` template ships with the project — but secrets like `Jwt:Key` and DB passwords should still come from environment variables, not that file.

---

## Database Providers

The DB engine is selected at startup from `Database:Provider` in `Program.cs`. The same codebase runs against all three — only config changes:

| Provider | Typical use | Connection string format |
|---|---|---|
| `Sqlite` | Local dev (default) | `Data Source=ProductCatalog.db` |
| `MySql` | Production | `Server=host;Port=3306;Database=productcatalog;User=appuser;Password=...` |
| `Postgres` | Production | `Host=host;Port=5432;Database=productcatalog;Username=appuser;Password=...` |

> **Migrations are provider-specific.** The committed `Migrations/` folder targets one provider at a time — column types, default-value SQL, and identity syntax all differ across SQLite, MySQL, and PostgreSQL. If you switch providers, delete the folder and regenerate:
>
> ```bash
> # Make sure Database:Provider and ConnectionStrings:DefaultConnection point at the new provider first
> rm -r Migrations
> dotnet ef migrations add InitialCreate
> dotnet ef database update
> ```
>
> The app calls `Database.Migrate()` on startup, so once migrations match the provider, the schema applies on next run.

### Removing providers you don't need

All three EF Core provider packages ship in `WebApi.csproj` so the boilerplate works out of the box. If you're only targeting one database, remove the packages you don't need — and delete the matching `case` branches in `Program.cs`'s provider switch:

```bash
dotnet remove package Pomelo.EntityFrameworkCore.MySql
dotnet remove package Npgsql.EntityFrameworkCore.PostgreSQL
# keep only Microsoft.EntityFrameworkCore.Sqlite, for example
```

---

## Development Notes

- The `.db` file is excluded from git — each developer keeps their own local database
- Migrations are committed so all developers share the same schema
- Swagger only runs in the `Development` environment
- Database seeding only runs in the `Development` environment
