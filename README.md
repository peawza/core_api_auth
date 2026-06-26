# MES Auth API

Authentication and user-management API for the MES backoffice stack. This
service is built with ASP.NET Core on .NET 8, ASP.NET Identity, Entity Framework
Core, and SQL Server.

## What This Service Handles

- User authentication and identity management
- User, role, permission, and screen management
- OTP send/resend/verify flows
- Localized resources and system configuration lookups
- Email/SMS integration for login and onboarding flows
- JWT authentication with optional Redis support

## Solution Structure

```text
api.auth/
|-- api.auth.sln
|-- Dockerfile.auth
|-- Services/
|   `-- Authentication/
|       |-- Program.cs
|       |-- StartupService.cs
|       |-- SetupHttpClient.cs
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

| Project | Path | Description |
| --- | --- | --- |
| Authentication | `Services/Authentication/Authentication.csproj` | ASP.NET Core Web API |
| ApplicationDB | `Database/ApplicationDB/ApplicationDB.csproj` | EF Core contexts, entities, and migrations |
| utils.core.mssql | `Libraries/Utils/Utils/utils.core.mssql.csproj` | Shared utilities and framework helpers |

## Main Modules

| Area | Controller | Service | Repository |
| --- | --- | --- | --- |
| Users | `UMS010Controller.cs` | `UMS010Service.cs` | `UMS010Repository.cs` |
| Permissions | `UMS020Controller.cs` | `UMS020Service.cs` | `UMS020Repository.cs` |
| Roles/Groups | `UMS030Controller.cs` | `UMS030Service.cs` | `UMS030Repository.cs` |
| System Screens | `SSS010Controller.cs` | `SSS010Services.cs` | `SSS010Repository.cs` |
| OTP | `OtpController.cs` | `OtpServic.cs` | `OtpRepository.cs` |
| Common Data | `commonController.cs` | `CommonService.cs` | `CommonRepository.cs` |

## Prerequisites

- .NET SDK 8.x
- SQL Server
- Docker, if building container images

## Configuration

Configuration files are under `Services/Authentication/`:

- `appsettings.json`
- `appsettings.Development.json`
- `appsettings.UAT.json`
- `appsettings.PROD.json`

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

Security note: existing config files may contain credentials. Avoid adding new
secrets to tracked config files. Prefer environment variables, user secrets, or
the deployment secret manager.

## Build And Run

Restore packages:

```powershell
dotnet restore .\api.auth.sln
```

Build the solution:

```powershell
dotnet build .\api.auth.sln
```

Run the API locally:

```powershell
dotnet run --project .\Services\Authentication\Authentication.csproj
```

Swagger is controlled by `Swagger:Enabled` in appsettings.

## Docker

Build a local Docker image:

```powershell
docker build -f Dockerfile.auth -t auth-api:latest .
```

The Dockerfile restores and publishes `Services/Authentication/Authentication.csproj`,
then runs:

```text
dotnet Authentication.dll
```

Container ports exposed by the image:

- `8080`
- `8081`

## Database

This service uses two EF Core DbContexts:

- `ApplicationDbContext`
  - Identity and user-related tables such as `tb_User`, `tb_Role`,
    `tb_UserRole`, `tb_UserInfo`, and `tb_LoginOtp`
- `SystemDbContext`
  - System tables such as `ts_SystemConfig`, `ts_Screen`,
    `ts_LocalizedResources`, and `tb_API_SMS_Logs`

Migration folders:

- `Database/ApplicationDB/Migrations/Application`
- `Database/ApplicationDB/Migrations/System`

When changing schema, create an EF Core migration for the correct DbContext and
verify the full solution still builds.

## Development Notes

- The API follows a Controller -> Service -> Repository structure.
- Controllers generally inherit from `Utils.Extensions.AppControllerBase`.
- API responses are wrapped by `AppControllerBase.Ok(object)` as
  `{ resultStatus, resultCode, resultMessage, data }`.
- Register services and repositories in
  `Services/Authentication/StartupService.cs`.
- Register external HTTP clients in `Services/Authentication/SetupHttpClient.cs`.
- Do not edit generated output such as `bin/`, `obj/`, or `.vs/`.
- Preserve existing public names, even if they contain typos, unless a rename is
  explicitly planned. Examples: `IOtpServic`, `OtpServic`, `IResouresService`.

## Agent Guide

For AI-agent-oriented project instructions, see `agent.md`.
