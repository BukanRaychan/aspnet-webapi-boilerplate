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

### 3. Apply migrations

```bash
dotnet ef database update
```

This creates the `ProductCatalog.db` SQLite file and applies all migrations.

### 4. Run the app

```bash
dotnet run
```

The app runs at `http://localhost:5280` by default.

### 5. Open Swagger UI

```
http://localhost:5280/swagger
```

---

## Project Structure

```
ProductCatalogAPI/
├── Controllers/    # HTTP endpoints, handles requests responses
│   └── ...
├── Data/   # Database context and seeding
│   └── ...
├── DTOs/   # Data Transfer Objects — controls API input/output shape
│   ├── AuthDtos/
│   │   └── ...
│   └── ProductDtos/
│       └── ...
├── Exceptions/   # Global error handling
│   └── ...
├── Migrations/           # EF Core auto-generated migration files
├── Models/               # Database entity classes
│   └── ...
├── Profiles/             # AutoMapper mapping profiles
│   └── ...
├── Repositories/         # Database query layer
│   └── ...
├── Services/             # Business logic layer
│   └── ...
├── Validators/           # FluentValidation rules
│   ├── AuthValidators/
│   │   └── ...
│   └── ProductValidators/
│       └── ...
├── appsettings.json
├── appsettings.Development.json
└── Program.cs
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
| `Database:Provider` | Database engine: `Sqlite` (default, local dev) or `MySql` |
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

The DB engine is selected at startup from `Database:Provider`, so the same code runs on SQLite locally and MySQL in production with no code changes:

| Provider | When | Connection string example |
|---|---|---|
| `Sqlite` | Local dev (default) | `Data Source=ProductCatalog.db` |
| `MySql` | Production | `Server=host;Port=3306;Database=productcatalog;User=appuser;Password=...` |

> **⚠️ Migrations are provider-specific.** The committed `Migrations/` folder was generated for **SQLite**. When you switch to MySQL, delete the `Migrations/` folder and regenerate it against MySQL before deploying:
>
> ```bash
> # with Database:Provider=MySql and a reachable MySQL connection string
> rm -r Migrations
> dotnet ef migrations add InitialCreate
> ```
>
> The app calls `Database.Migrate()` on startup, so once the migrations match the provider, the schema is applied automatically on first run.

---

## Development Notes

- The `.db` file is excluded from git via `.gitignore` — each developer has their own local database
- Migrations are committed to git so all developers share the same schema
- Swagger is only enabled in the `Development` environment
- Database seeding only runs in the `Development` environment
