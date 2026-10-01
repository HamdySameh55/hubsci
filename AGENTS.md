# AGENTS.md

ASP.NET Core MVC online-learning platform (.NET 9). SQL Server + EF Core 9.0.13. Custom cookie auth (no ASP.NET Identity). InstaPay payment proof workflow (no Stripe/webhooks). No tests, no CI. Repo has no VCS (not a git repo).

## Solution layout

4-layer project (reference chain: Web → Business → Data → Domain; Web also references Data):
- `OnlineLearning.Domain` — entities only (`Entities/`). Has placeholder `Class1.cs` (ignore).
- `OnlineLearning.Data` — EF Core: `Context/ApplicationDbContext.cs`, `Configurations/` (IEntityTypeConfiguration per entity), `Migrations/`. Repos: `Repositories/Interfaces/*` + `Implementations/*`. Note: `CourseRepository.cs.cs` is a naming quirk (leave as-is).
- `OnlineLearning.Business` — `Services/Interfaces/*` + `Implementations/*`, `DTOs/`. Business depends on Data (and Domain). Uses `Microsoft.Extensions.Identity.Core` for `PasswordHasher<User>`.
- `OnlineLearning.Web` — MVC app: `Controllers/`, `Views/`, `wwwroot/`, `Services/PaymentProofUploader.cs`, `Data/AdminSeeder.cs`, `ViewModels/`. ViewModels exist; views bind to Business DTOs.

## Commands (run from repo root)

- Build: `dotnet build OnlineLearning.sln`
- Run Web (http): `dotnet run --project OnlineLearning.Web --launch-profile http` (http://localhost:5296; https also configured)
- Migrations: `dotnet ef migrations add <Name> --project OnlineLearning.Data --startup-project OnlineLearning.Web`
- Update DB: `dotnet ef database update --project OnlineLearning.Data --startup-project OnlineLearning.Web` (requires SQL Server reachable at `ConnectionStrings:DefaultConnection`)
- EF tools: `dotnet ef --version` (EF Core .NET tools 9.0.13 installed)

## Runtime prerequisites (must configure before running)

App throws on startup unless these are set. Use user secrets (Web has `UserSecretsId 99ef741c-5a3f-43db-941d-029a2ec595d8`):
- `Admin:Email` and `Admin:Password` — required by `Web/Data/AdminSeeder.cs` (throws if missing/whitespace). Set via:
  - `dotnet user-secrets set "Admin:Email" <email> --project OnlineLearning.Web`
  - `dotnet user-secrets set "Admin:Password" <password> --project OnlineLearning.Web`
- `ConnectionStrings:DefaultConnection` — must point to reachable SQL Server. In Development, only Logging is defined; Production has empty `DefaultConnection`. Set as needed (e.g. `dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=OnlineLearningDb;Trusted_Connection=True" --project OnlineLearning.Web`).

Payment: InstaPay proof upload via `PaymentProofUploader` (5 MB max, allowed `.jpg,.jpeg,.png,.gif,.webp`) to `wwwroot/uploads/proofs/`. No Stripe/webhooks present.

## Must-know wiring details

- DI is manual in `Web/Program.cs` — every repository and service registered via `AddScoped`. If adding new interface+impl, register it there (Repos/Services/Admin/Auth/PasswordHasher blocks).
- Auth is custom cookie auth (scheme Cookie, `LoginPath=/Account/Login`, `AccessDeniedPath=/Account/AccessDenied`). Roles are plain string on `User` ("Admin","Instructor","Student"). `PasswordHasher<User>` from `Microsoft.Extensions.Identity.Core`. AuthService verifies hash and returns user.
- DbContext: `ApplicationDbContext` with `DbSet`s for all entities; `OnModelCreating` calls `ApplyConfigurationsFromAssembly` to load IEntityTypeConfiguration classes.
- Migrations in Data; startup project must be Web for config/connection.
- Admin seeding runs at app start (`AdminSeeder.SeedAsync`) and only creates Admin if none exists (`user.Role == "Admin"`).

## Conventions (repo-specific)

- New entity: add POCO in `Domain/Entities/`, `DbSet` in `ApplicationDbContext`, `IEntityTypeConfiguration` in `Data/Configurations/`, create migration.
- Repositories: `Data/Repositories/Interfaces/I<X>Repository.cs` + `Implementations/<X>Repository.cs` (note `CourseRepository.cs.cs` name; keep as-is). Services mirror in `Business/Services/`.
- Views use Business DTOs, not domain entities. `Web/ViewModels/` is small and exists for specific cases (e.g. payment proof, home).