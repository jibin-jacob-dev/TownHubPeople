# TownHub People — HRMS

A dedicated, standalone HR Management System built on **ASP.NET Core MVC (.NET 10)** with
**Clean Architecture**. This repository was bootstrapped from the identity, role/claims, and
configurable role-based access-control foundation of the TownHub ERP, rehomed under the
`TownHubPeople.*` root with an HRMS-specific role and module taxonomy.

## Solution Structure

```
src/
├── TownHubPeople.Domain/         entities, enums, interfaces (no external deps)
├── TownHubPeople.Application/    services, DTOs, validators, Mapster
├── TownHubPeople.Infrastructure/ EF Core, Identity, repositories, seeders
└── TownHubPeople.Web/            MVC controllers, views, view components, Program.cs
```
**Dependency flow:** Web → Infrastructure → Application → Domain.

## Tech Stack

.NET 10 / ASP.NET Core MVC · EF Core 10 (SQL Server) · ASP.NET Core Identity + cookie auth ·
FluentValidation · Mapster · Serilog · Bootstrap 5 / jQuery / DataTables / Select2 / Toastr /
SweetAlert2 / Sortable.js.

## What's in this foundation phase

- **Identity & auth** — `ApplicationUser`/`ApplicationRole`, custom Identity table names
  (`Users`/`Roles`/`UserRoles`/…), cookie auth, login / register / profile.
- **HRMS roles** (seeded) — `Administrator, HRManager, Recruiter, PayrollManager, LineManager,
  Employee, Viewer`.
- **Configurable role-based access** — `RoleMenuAccess` + `RoleTabAccess`, the
  `MenuConfiguration` service, the `[AuthorizeTab("Module","Tab")]` attribute/filter +
  authorization handler, and the Administration UI (Users, Roles, Menu Config, Audit Trail,
  Login Slides, Organization).
- **Cross-cutting infra** — soft delete (global query filter), audit trail interceptor,
  auditable-entity interceptor, document numbering, reentrant Unit of Work, email outbox.

## How to Run

```bash
cd src/TownHubPeople.Web && dotnet run
```

SQL Server (Windows auth). Default connection string:
`Server=.;Database=TownHubPeople;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True`
(override via `ConnectionStrings__DefaultConnection`). **Migrations run automatically on startup.**

**Default admin:** `admin@townhubpeople.local` / `Admin@123456`

## Roadmap

HR domain (Employees, Leave, Payroll, Attendance, Recruitment, Performance, Learning),
an approval engine, a central Documents module, a REST API, and a mobile client build on top of
this foundation in subsequent phases.
