# Agent Guide: api.auth

> **MANDATORY — READ FIRST, EVERY TIME**
>
> Every AI agent MUST read this file before doing ANY work in this repository
> (reading, editing, building, or answering questions about the project). This
> file is the single source of truth for project shape and conventions. Do not
> rely on memory from a previous session — re-read it at the start of each task.
>
> If your change conflicts with anything here, stop and confirm with the user
> before proceeding. If you change project structure, conventions, or commands,
> update this file in the same task so it stays accurate.

This file is the working guide for AI agents and developers changing the
`api.auth` project. Read it before editing code so changes follow the existing
project shape and conventions.

## Project Overview

`api.auth` is a .NET 8 ASP.NET Core Web API for authentication, user
management, permissions, screens/resources, system config, and OTP flows. It
uses SQL Server through Entity Framework Core and ASP.NET Identity.

The main solution is `api.auth.sln`.

| Project | Path | Responsibility |
| --- | --- | --- |
| Authentication | `Services/Authentication/Authentication.csproj` | Web API, controllers, services, repositories, models, startup config |
| ApplicationDB | `Database/ApplicationDB/ApplicationDB.csproj` | EF Core contexts, database entities, migrations |
| utils.core.mssql | `Libraries/Utils/Utils/utils.core.mssql.csproj` | Shared utilities: controller base, auth/config helpers, email, encryption, middleware |

## Directory Structure

```text
api.auth/
|-- api.auth.sln
|-- Dockerfile.auth
|-- Services/
|   `-- Authentication/
|       |-- Program.cs
|       |-- StartupService.cs
|       |-- SetupHttpClient.cs
|       |-- appsettings*.json
|       |-- Controllers/
|       |-- Services/
|       |-- Repositories/
|       |-- Models/
|       |-- ApiClients/
|       |-- Providers/
|       |-- Validators/
|       `-- Templates/
|-- Database/
|   `-- ApplicationDB/
|       |-- ApplicationDbContext.cs
|       |-- SystemDbContext.cs
|       |-- Models/
|       `-- Migrations/
`-- Libraries/
    `-- Utils/
        `-- Utils/
            |-- Extensions/
            |-- Helper/
            |-- Constants/
            |-- Middleware/
            `-- Models/
```

## Runtime Composition

The application entry point is `Services/Authentication/Program.cs`.

Startup registers:

- `ApplicationDbContext` and `SystemDbContext` with `ConnectionStrings:DBConnection`.
- ASP.NET Identity using `ApplicationUser`, `ApplicationRole`,
  `ApplicationUserManager`, and `ApplicationSignInManager`.
- JWT/authentication, CORS, Swagger, controller config, request size config,
  Redis, and shared utils through `Utils.Startup`.
- Repositories and services through `StartupService.InitialService`.
- External HTTP clients through `SetupHttpClient.InitialService`.
- SMS client base URL from `ApiClients:SMSApi`.

## Main API Modules

| Module | Controller | Service | Repository | Model |
| --- | --- | --- | --- | --- |
| User Management | `UMS010Controller.cs` | `UMS010Service.cs` | `UMS010Repository.cs` | `Models/UMS010/UMS010.cs` |
| Permission/Role related | `UMS020Controller.cs` | `UMS020Service.cs` | `UMS020Repository.cs` | `Models/UMS020/UMS020.cs` |
| Role/Group related | `UMS030Controller.cs` | `UMS030Service.cs` | `UMS030Repository.cs` | `Models/UMS030/UMS030.cs` |
| System screen/config | `SSS010Controller.cs` | `SSS010Services.cs` | `SSS010Repository.cs` | `Models/SSS010/SSS010.cs` |
| OTP | `OtpController.cs` | `OtpServic.cs` | `OtpRepository.cs` | `Models/OTP/OTPModels.cs` |
| Common | `commonController.cs` | `CommonService.cs` | `CommonRepository.cs` | `Models/CommonModels.cs` |

## Code Pattern

The project follows a Controller -> Service -> Repository pattern.

1. Controller receives the request, validates `criteria == null ||
   !ModelState.IsValid`, and calls the service.
2. Service handles business flow, transactions, and orchestration.
3. Repository queries or updates the database through EF Core, Identity
   managers, email/SMS clients, or system config.
4. Models usually store request/response DTOs as nested classes in each module
   model file.

Most controllers inherit from `Utils.Extensions.AppControllerBase`.

Response helpers:

- `Ok(data)` wraps data as `{ resultStatus, resultCode, resultMessage, data }`.
- `InternalServerError(ex)` returns the shared error response shape.
- Routes commonly use lowercase module routes such as `[Route("ums010")]` and
  action routes such as `[HttpPost("search")]`.

## Dependency Injection

Register new services and repositories in
`Services/Authentication/StartupService.cs`.

```csharp
services.AddTransient<IMyRepository, MyRepository>();
services.AddTransient<IMyService, MyService>();
```

Register new external API clients in `Services/Authentication/SetupHttpClient.cs`.

## Database

There are two main DbContexts.

`ApplicationDbContext`:

- File: `Database/ApplicationDB/ApplicationDbContext.cs`
- Identity and application user tables such as `tb_User`, `tb_Role`,
  `tb_UserRole`, `tb_UserInfo`, and `tb_LoginOtp`.

`SystemDbContext`:

- File: `Database/ApplicationDB/SystemDbContext.cs`
- System tables such as `ts_SystemConfig`, `ts_Screen`,
  `ts_LocalizedResources`, and `tb_API_SMS_Logs`.

Migration folders:

- `Database/ApplicationDB/Migrations/Application`
- `Database/ApplicationDB/Migrations/System`

Avoid manually editing migrations or model snapshots unless the task is
explicitly about migration repair. If schema changes are needed, create an EF
Core migration for the correct DbContext.

## Configuration

Configuration files:

- `Services/Authentication/appsettings.json`
- `Services/Authentication/appsettings.Development.json`
- `Services/Authentication/appsettings.UAT.json`
- `Services/Authentication/appsettings.PROD.json`

Important keys:

- `ConnectionStrings:DBConnection`
- `JwtKey`, `JwtIssuer`, `JwtExpireMinutes`
- `Encryption:Base64Key`
- `Constants:*`
- `Authentication:FirstLogin`
- `Authentication:OTPLogin`
- `Authentication:SendSMSOTP`
- `Authentication:SendEmailOTP`
- `Swagger:Enabled`
- `Redis:*`
- `ApiClients:SMSApi`

Security note: config files already contain secrets and credentials. Do not add
new secrets to config files unless the task explicitly requires it. Prefer
environment variables, user secrets, or the deployment secret manager.

## Common Commands

Restore:

```powershell
dotnet restore .\api.auth.sln
```

Build:

```powershell
dotnet build .\api.auth.sln
```

Run API locally:

```powershell
dotnet run --project .\Services\Authentication\Authentication.csproj
```

Build Docker image:

```powershell
docker build -f Dockerfile.auth -t auth-api:latest .
```

## Development Guidelines For Agents

- Before changing a module, read its `Controller`, `Service`, `Repository`, and
  `Models` files together.
- Follow existing module naming such as `UMS010_*_Criteria` and
  `UMS010_*_Result`.
- For a new endpoint, usually add the DTO in `Models/<Module>/...`, service
  interface/class methods, repository interface/class methods, and DI
  registration if new classes are introduced.
- If multiple database writes must be atomic, start the transaction in the
  service layer as existing modules do.
- Use EF Core async APIs such as `ToListAsync`, `FirstOrDefaultAsync`, and
  `SaveChangesAsync`.
- Preserve the existing response format by returning result objects through
  `Ok(result)`.
- Avoid changing global behavior in `Utils.Startup`, `AppControllerBase`,
  authentication config, or Identity options unless the requirement is explicit.
- Do not edit or commit generated/build output such as `bin/`, `obj/`, or `.vs/`.
- Preserve existing public names even when they contain typos, such as
  `IOtpServic`, `OtpServic`, and `IResouresService`, unless a rename is the
  explicit task.

## Token Efficiency Rules (All AI Agents)

Apply these rules on every task to keep token usage low while staying correct.

Reading and searching:

- Read this `agent.md` first, then go straight to the relevant files. Do not
  scan the whole repo "just in case".
- Use targeted search (grep/file search) for symbols or strings instead of
  reading many files. Read only the specific files a search points to.
- Read a file once. Reuse what you already have in context instead of
  re-reading the same file.
- Prefer reading specific line ranges or signatures over whole large files,
  unless you genuinely need the full content.
- Never read or dump generated/build output (`bin/`, `obj/`, `.vs/`,
  migrations snapshots) unless the task is explicitly about them.

Working pattern:

- Touch only the module in scope. For a feature in one module, read just its
  `Controller` / `Service` / `Repository` / `Models` set, not every module.
- Make focused edits (string replace) instead of rewriting whole files.
- Batch independent reads/searches in parallel; avoid one-at-a-time round trips.

Responding:

- Be concise. Answer the question asked; skip restating the full file or task.
- Do not paste large code blocks back to the user when a short reference
  (file + function name) is enough.
- Do not repeat unchanged code in explanations. Show only the diff or the part
  that changed.
- Skip filler ("let me", "now I will", long recaps). State the outcome briefly.
- Stop when the task is done. Do not add extra suggestions, refactors, or
  defensive code beyond what was requested.

Verification:

- Run the minimum needed: `dotnet build .\api.auth.sln`. Run Docker or
  migration steps only when those files changed.

## Testing And Verification

Minimum verification after code changes:

```powershell
dotnet build .\api.auth.sln
```

If Docker/deployment files changed:

```powershell
docker build -f Dockerfile.auth -t auth-api:latest .
```

If database models or migrations changed, inspect the generated migration and
build the full solution before handing off.

## Known Notes

- `README.md` is still mostly the default GitLab template.
- `Dockerfile.auth` builds from the repo root, restores
  `Services/Authentication/Authentication.csproj`, publishes `Authentication.dll`,
  and exposes ports `8080` and `8081`.
- The project references both `AutoMapper` and `Mapster`; check the existing
  module pattern before choosing one.
- `AppControllerBase.Ok(object)` returns camelCase JSON using Newtonsoft
  settings.
