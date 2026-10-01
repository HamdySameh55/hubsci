# OnlineLearning — Technical Analysis Report

- **Solution:** `OnlineLearning.sln` (ASP.NET Core MVC, .NET 9, SQL Server, EF Core 9.0.13, custom cookie auth, Stripe)
- **Build:** `dotnet build OnlineLearning.sln` — 0 warnings, 0 errors (SDK 9.0.317, dotnet-ef 9.0.13)
- **VCS:** No git repo, no CI.

---

# 1. PROJECT STRUCTURE

4 projects:

```
E:\Projects\OnlineLearning\
├── OnlineLearning.sln                     (solution file, no git repo / no CI)
├── AGENTS.md                              (project guidance doc)
│
├── OnlineLearning.Domain\                 (entities only — plain POCOs)
│   └── Entities\   13 files: User, Course, Module, Lesson, Quiz, Question,
│                   Answer, Enrollment, Payment, LessonProgress, QuizAttempt,
│                   Certificate, Feedback   (+ leftover Class1.cs placeholder)
│
├── OnlineLearning.Data\
│   ├── Context\ApplicationDbContext.cs    (13 DbSets)
│   ├── Configurations\  13 IEntityTypeConfiguration files
│   ├── Migrations\      3 migrations: InitialCreate, AddCourseStatus,
│   │                    AllowCourseCascadeDelete  (+ snapshot)
│   ├── Repositories\Interfaces\       13
│   ├── Repositories\Implementations\  13 (Course one lives in oddly-named
│   │                                   CourseRepository.cs.cs — valid)
│   └── (+ leftover Class1.cs placeholder)
│
├── OnlineLearning.Business\
│   ├── DTOs\          32 files (create + read DTOs per entity, RegisterDto,
│   │                   LoginDto, CourseProgressDto, CertificateDto, AdminDashboardDto)
│   ├── Services\Interfaces\       16
│   ├── Services\Implementations\  16
│   └── (+ leftover Class1.cs placeholder)
│
└── OnlineLearning.Web\
    ├── Controllers\   17 (incl. api/stripe/webhook)
    ├── Views\         45 .cshtml across Home, Account, Admin, User, Course,
    │                   Module, Lesson, Quiz, Question, Answer, Enrollment,
    │                   Payment, QuizAttempt, LessonProgress→(none), Certificate,
    │                   Feedback, Shared
    ├── ViewModels\    (empty folder)
    ├── Views\_ViewImports   (imports Web + Web.Models only)
    ├── Stripe\        StripeSettings.cs, StripeCheckoutService.cs, StripeWebhookService.cs (empty class)
    ├── Data\AdminSeeder.cs
    ├── wwwroot\       static assets (bootstrap/jquery bundles)
    ├── appsettings.json / appsettings.Development.json / Program.cs
    └── OnlineLearning.Web.csproj  (UserSecretsId 99ef741c-…)
```

- **Reference chain:** Web → Business → Data → Domain. Business references Data directly (services use repositories); Web also references Data directly (controllers inject `ICourseRepository` / `IEnrollmentRepository`).
- **Key packages (verified in csproj):** `Microsoft.EntityFrameworkCore.SqlServer` + `.Design` 9.0.13; `Stripe.net` 52.4.2; Business references `Microsoft.Extensions.Identity.Core` (10.0.11). All TFM `net9.0`. Bootstrap 5.3.3, jQuery 3.7.1, jQuery Validation 1.19.5.

---

# 2. PROJECT STATUS

| Project | Completeness | Notes |
|---|---|---|
| OnlineLearning.Domain | ✅ 100% | 13 clean POCOs, no logic, no enums |
| OnlineLearning.Data | ⚠️ ~95% | Schema + 3 migrations complete; statuses are unconstrained strings |
| OnlineLearning.Business | ✅ ~95% | 16 services cover all workflows; some N+1 in-memory filtering |
| OnlineLearning.Web | ⚠️ ~85% | All controllers + most views; **4 missing view targets**, no tests, no CI |

Overall implementation: approximately **90% complete**. The core student loop works end-to-end (browse → enroll → pay → learn → complete lesson → quiz → certificate → feedback). Gaps are view-page coverage, moderation, no testing, and secret-handling/config discipline.

---

# 3. DATABASE DESIGN

13 entities (`Domain/Entities`, configured one-per-file in `Data/Configurations`, applied via `ApplyConfigurationsFromAssembly`, 3 migrations):

```
User ──┬──< Course        (Instructor, FK Restrict)
       ├──< Enrollment     (FK Restrict)
       └──< Feedback       (FK Restrict)

Course ──┬──< Module ──< Lesson
         │       └─── 1:1 Quiz ──< Question ──< Answer
         ├──< Enrollment ──┬── 1:1 Payment
         │                 ├── 1:1 Certificate
         │                 ├──< LessonProgress
         │                 └──< QuizAttempt
         └──< Feedback  (FK Cascade)
```

Key facts verified:
- **Keys:** all PKs `int` identity.
- **Unique indexes:** `User.Email`; `Enrollment(UserId, CourseId)`; `Module(CourseId, OrderNumber)`; `Payment.EnrollmentId` (1:1); `Certificate.EnrollmentId` (1:1); `Quiz.ModuleId` (1:1); `LessonProgress(EnrollmentId, LessonId)`; `Feedback(UserId, CourseId)`.
- **Delete behavior:** Course→Instructor Restrict; Enrollment→User Restrict, →Course Cascade (added by 3rd migration); Payment/Certificate/LessonProgress/QuizAttempt→Enrollment Cascade except QuizAttempt.Quiz Restrict, LessonProgress.Lesson Restrict; Module/Lesson→Course Cascade; Question→Quiz Cascade; Answer→Question Cascade; Feedback→Course Cascade, →User Restrict.
- **Columns of note:** `Course.Price` / `Payment.Amount` decimal(18,2); `QuizAttempt.Score` decimal(5,2); statuses `Course.Status` (default "Draft"), `Enrollment.Status` (default "Pending"), `Payment.PaymentStatus` (default "Pending"), `Feedback.Status` (default "Pending") — all plain `nvarchar(max)` strings.
- **Migrations:** `20260907080459_InitialCreate` → `20260910203429_AddCourseStatus` (added Course.Status, Feedback.Status) → `20260912161548_AllowCourseCascadeDelete` (latest).

⚠️ Design notes: low-level states are magic strings, not enums/constrained; `Course.Status` is not configured in `CourseConfiguration` (relies on the migration adding an unconstrained column). No int rowversion/optimistic concurrency.

---

# 4. DATA ACCESS LAYER

- **DbContext:** `ApplicationDbContext` (DbSet per entity, `ApplyConfigurationsFromAssembly`), SQL Server via `UseSqlServer` on `DefaultConnection`.
- **Repositories:** classic per-entity `IRepository` pattern — `GetByIdAsync`, `GetAllAsync`, `AddAsync` (Add, no save), `Update`/`Delete`, `SaveChangesAsync`. Not a generic `IRepository<T>` — 13 bespoke interfaces.
- **Notable query methods:** `EnrollmentRepository.GetByUserAndCourseAsync` / `HasActiveEnrollmentAsync` / `GetActiveEnrollmentAsync` (includes Course+User); `LessonProgressRepository.GetByUserIdAsync` / `GetByIdForUserAsync`; `QuizRepository.GetByIdWithQuestionsAsync` (includes Module, Questions, Answers) / `GetByCourseIdAsync`; `QuestionRepository.GetByIdWithAnswersAsync`; `QuizAttemptRepository.GetRecentAttemptsAsync(enrollmentId, quizId, since)`; `CertificateRepository.GetByEnrollmentIdAsync`; `UserRepository.GetByEmailAsync`, `HasEnrollmentsAsync`, `HasCoursesAsync`, `HasFeedbacksAsync`; `LessonRepository.GetModuleByLessonIdAsync`.

⚠️ Perf: several flows load all rows and filter in memory — `QuizAttemptService` (checks prior pass via `GetAllAsync`), `ModuleController.Details` (filters lessons/quizzes from full lists), `CertificateService`/`CertificateController.Index` (per-certificate enrollment lookup — N+1), `AdminDashboardService` (loads 4 full tables). Fine at small scale; will degrade.

---

# 5. DTOs (32 files)

Create/read pair per entity plus shared types, all in `Business/DTOs`:
- **Write:** CreateCourseDto, CreateModuleDto (OrderNumber), CreateLessonDto (ContentType, ContentUrl), CreateQuizDto, CreateQuestionDto, CreateAnswerDto (IsCorrect), CreateEnrollmentDto, CreateLessonProgressDto, CreateQuizAttemptDto (EnrollmentId, QuizId, List<QuizAnswerSelectionDto>), CreateFeedbackDto, CreateUserDto, RegisterDto (Password+ConfirmPassword), LoginDto.
- **Read:** CourseDto (includes Status, Price, InstructorId), ModuleDto, LessonDto (adds CourseId), QuizDto (includes CourseId + Questions), QuestionDto (QuizId), AnswerDto, QuizAnswerDto, EnrollmentDto, PaymentDto, LessonProgressDto, QuizAttemptDto, CertificateDto (adds StudentName, CourseTitle), FeedbackDto, UserDto, CourseProgressDto (TotalLessons/CompletedLessons/TotalQuizzes/PassedQuizzes/ProgressPercentage/IsCompleted), AdminDashboardDto.
- **Validation:** `[Required]`, `[StringLength]` (Title/Description/Comment/ContentType/ContentUrl), `[EmailAddress]`, `[Range(1,5)]` Rating, `[Range(0,100)]` Score, `[Range(1,…)]` OrderNumber, Password min-lengths on Register/User DTOs.

Good practice: quiz answer DTOs deliberately omit `IsCorrect` for students; views bind DTOs.

---

# 6. BUSINESS FUNCTIONS (16 services)

All logic lives in `Business/Services` (interface + implementation per feature). Verified rules:

- **AuthService** — login verifies `PasswordHasher<User>` hash; register creates User with Role "Student"/"Instructor"; duplicate-email guard.
- **CourseService** — create defaults Status **"Draft"**; update/delete/publish enforce owner (instructorId); `GetPublishedByIdAsync` returns only Published; `DeleteByAdminAsync` for admin.
- **ModuleService** — CRUD with course-ownership validation + duplicate-`OrderNumber` guard (`InvalidOperationException`).
- **LessonService / QuizService / QuestionService / AnswerService** — CRUD; Question/Answer creation validated against quiz; Update/Delete ownership done at controller layer (not in service).
- **EnrollmentService.CreateAsync** — course must be **Published**, no existing enrollment (duplicate guarded both in service and DB unique index), creates Status **"Pending"**.
- **PaymentService.CompletePaymentAsync** — amount must equal `Course.Price`; creates Payment(Successful) + sets Enrollment **"Active"**; idempotent re-run for already-successful payments.
- **LessonProgressService** — requires **Active** enrollment + unique (EnrollmentId, LessonId); marks lesson complete.
- **CourseProgressService** — computes totals, completed lessons/quiz-count, `ProgressPercentage`, `IsCompleted` (all lessons done + all quizzes passed).
- **QuizAttemptService.CreateAsync** (the heavyweight rule set): active enrollment; quiz belongs to enrolled course; quiz has questions; **cannot retake after passing**; **locked after 3 failed attempts within 24h**; must answer every question, exactly one answer each, all answers belong to quiz; score = correct/total×100; **pass at ≥60%**; attempt number sequential.
- **CertificateService.CreateAsync** — enrollment owned by caller + **Active**; no existing certificate (also DB-unique); `CourseProgress.IsCompleted` required; `CertificateUrl` = **empty string** (no PDF generated).
- **FeedbackService** — active enrollment + completed course required; only **one feedback per (user, course)** (service + unique index); rating optional but must be 0–5 ("at least rating OR comment"); edit resets Status to "Pending".
- **UserService** — admin user CRUD with delete guards if user has enrollments/courses/feedbacks.
- **AdminDashboardService** — counts users/students/instructors/courses/published/enrollments + sum of `Successful` payments.

---

# 7. WEB APPLICATION / CONTROLLERS (17)

Custom cookie auth (no ASP.NET Identity). Default route `{controller=Home}/{action=Index}/{id?}`. Claims: NameIdentifier, Name, Email, Role. `LoginPath = /Account/Login`, `AccessDeniedPath = /Account/AccessDenied`.

| Controller | Actions | Access / enforcement |
|---|---|---|
| HomeController | Index, Privacy, **Profile**, **Admin**, Error | Profile `[Authorize]`, Admin `[Authorize(Roles=Admin)]` — **Profile/Admin have NO views** |
| AccountController | Login(GET/POST), Register(GET/POST→Student), InstructorRegister(GET/POST→Instructor), Logout(POST), AccessDenied | anonymous |
| AdminController | Dashboard, Courses, DeleteCourse(POST) | `[Authorize(Roles="Admin")]` |
| UserController | Index, Create(GET/POST), Delete(POST) | `[Authorize(Roles="Admin")]` |
| CourseController | Index (`[AllowAnonymous]` — published for all, own courses for Instructor), Details (published-only; students get feedback/certificate flags from progress), Create/Edit/Delete/Publish(POST) | CRUD `[Authorize(Roles="Instructor")]` + ownership check (`InstructorId`) |
| ModuleController | Index/Details (Instructor owner / enrolled Student / Admin), Create/Edit/Delete | CRUD Instructor + ownership |
| LessonController | Index/Details (student needs **active enrollment**), Create/Edit/Delete | Instructor + ownership |
| QuizController | Details (`Student,Admin`, student needs active enrollment), Create/Edit/Delete | Student/Admin view; Instructor CRUD + ownership via module→course |
| QuestionController | Create/Edit/Delete | `[Authorize(Roles="Instructor")]` + ownership |
| AnswerController | Create/Edit/Delete | same |
| EnrollmentController | Create(POST, Student → redirects to Enrollment/Details), Details (student ownership) | `[Authorize(Roles="Student")]` |
| PaymentController | Details (student ownership — **NO view**), Checkout (Student; **Pending** enrollment only; real price from DB; redirects to Stripe), Success, Cancel | `[Authorize(Roles="Student")]` |
| LessonProgressController | Index, Details, Create(POST: mark lesson complete) | `[Authorize(Roles="Student")]` — **NO views** |
| QuizAttemptController | Create(POST) → `View("Result", attempt)`; verifies enrollment belongs to caller + is Active | `[Authorize(Roles="Student")]` |
| CertificateController | Index (filters by user), Details (ownership), Create(POST) | `[Authorize(Roles="Student")]` |
| FeedbackController | Index, Details, Create(GET/POST), Edit(GET/POST), Delete(POST) | `[Authorize(Roles="Student")]` + ownership |
| StripeWebhookController | `POST api/stripe/webhook` — `[ApiController]`, signature validation, handles `checkout.session.completed` | public (signature-secured) |

Error mapping is consistent: `KeyNotFoundException → NotFound`, `UnauthorizedAccessException → Forbid`/403, `InvalidOperationException/ArgumentException → TempData["Error"] + redirect/return view`.

---

# 8. VIEWS (45 .cshtml)

Folders: Account(4), Admin(2), Answer(2), Certificate(2), Course(4), Enrollment(1), Feedback(4), Home(2), Lesson(4), Module(4), Payment(2), Question(2), Quiz(3), QuizAttempt(1), Shared(4), User(2).

- Views bind **Business DTOs**, except `Payment/Success.cshtml` and `Payment/Cancel.cshtml` which bind domain `OnlineLearning.Domain.Entities.Enrollment` directly (inconsistency + exposes raw entity to views).
- `_ViewImports.cshtml` only imports `OnlineLearning.Web` + `Web.Models` — DTO namespaces are fully qualified in every view.
- `_Layout.cshtml` is **static**: branded nav only shows Home + Privacy; **no login/logout links, no role-based nav, no TempData/alert rendering** — users cannot discover Account/Admin/Student actions from the shell.
- Pages use heavy per-view inline `<style>` blocks with a consistent purple/cream theme (breadcrumb-free, self-contained).
- `Lesson/Details` embeds the "Mark as Complete" form (posts to LessonProgress/Create); `Module/Details` lists lessons + quiz with owner/enrolled gating; `Enrollment/Details` shows Pending/Active states with "Pay with Stripe" / "Start Learning" CTAs.
- Shared: `_Layout.cshtml`, `_Layout.cshtml.css`, `Error.cshtml`, `_ValidationScriptsPartial.cshtml`.

⚠️ **Missing views (actions exist → runtime 500 if visited):**
- `Home/Profile.cshtml`, `Home/Admin.cshtml`
- `LessonProgress/Index.cshtml`, `Details.cshtml`, `Create.cshtml` (entire folder absent)
- `Payment/Details.cshtml`

---

# 9. PROGRAM.CS & STARTUP

`Web/Program.cs` (173 lines, top-level statements):
1. `AddControllersWithViews()`.
2. Cookie authentication (`LoginPath /Account/Login`, `AccessDeniedPath /Account/AccessDenied`) + `AddAuthorization()`.
3. `AddDbContext<ApplicationDbContext>` → `UseSqlServer(DefaultConnection)`.
4. `Configure<StripeSettings>("Stripe")`, `AddScoped<StripeCheckoutService>()`.
5. **Manual, centralized DI** — all 13 repositories, 14 feature services + `IAdminDashboardService`, `PasswordHasher<User>`, `IAuthService` registered `AddScoped` one-by-one. (Any new repo/service must be registered here.)
6. `await AdminSeeder.SeedAsync(app.Services)` — **runs at every startup** (throws if `Admin:Email`/`Admin:Password` missing).
7. Pipeline: ExceptionHandler + HSTS (non-dev) → HttpsRedirection → StaticFiles → Routing → **Authentication → Authorization** → MVC default route → `Run()`.

No health checks, no CORS, no session/response caching, no Serilog/OpenTelemetry.

---

# 10. CURRENCY & PAYMENT (Stripe)

- `StripeSettings` binds `Stripe:PublishableKey` / `SecretKey` / `WebhookSecret`.
- `StripeCheckoutService.CreateCheckoutSessionAsync` — sets global `StripeConfiguration.ApiKey`; `Mode=Payment`; `Currency="egp"` (hardcoded EGP); unit amount = `price × 100`; metadata `EnrollmentId`; Success/Cancel URLs → `Payment/Success|Cancel`.
- `PaymentController.Checkout` — enrollment must be owned + **"Pending"**; price read from `Course` in DB; redirects to Stripe-hosted checkout.
- Webhook `StripeWebhookController` — verifies `Stripe-Signature` via `EventUtility.ConstructEvent` (needs `WebhookSecret`); on `checkout.session.completed` reads `EnrollmentId` metadata, computes amount = `AmountTotal / 100`, calls `CompletePaymentAsync(enrollmentId, amount, "Stripe")` → creates Successful Payment + sets Enrollment **"Active"**. Amount is re-validated against `Course.Price` server-side.
- **Flow:** enroll (Pending) → pay → Stripe redirect → webhook activates enrollment → student starts learning.

⚠️ Issues:
- **Stripe keys are committed in `appsettings.json`** (test keys `pk_test_…`/`sk_test_…`). Real key material must move to user secrets; `WebhookSecret` is **not** in appsettings — without it the webhook fails.
- Test-mode only; no plan/metadata beyond `EnrollmentId`; no refunds/subscriptions. `StripeWebhookService` is an empty placeholder class.

---

# 11. ADMIN & FEEDBACK MODERATION

- **AdminSeeder** (`Web/Data/AdminSeeder.cs`): creates the first Admin user from `Admin:Email`/`Admin:Password` using `PasswordHasher<User>`; **throws `InvalidOperationException` at startup if the config is absent**; no-op if an admin already exists.
- **AdminController**: `Dashboard` (counts + total revenue via `AdminDashboardService`), `Courses` (list + delete).
- **Admin manages users**: `UserController` (list, create, delete with guards).
- ⚠️ **Feedback moderation is NOT implemented.** Feedback is created/edited with `Status="Pending"` and shows "waiting for review", but there is **no admin approval/rejection screen or endpoint** — status stays "Pending" forever. Admin has no feedback list at all.

---

# 12. REQUIREMENTS TRACKING

| # | Requirement | Status | Evidence |
|---|---|---|---|
| 1 | Student registration | ✅ | Account/Register → Role "Student" |
| 2 | Instructor registration | ✅ | Account/InstructorRegister → Role "Instructor" |
| 3 | Role-based login | ✅ | Custom cookie auth + role claims |
| 4 | Course publishing workflow | ✅ | Course.Status Draft→Published, PublishAsync + Publish action |
| 5 | Instructor course/drive structure (modules→lessons→quizzes→Q/A) | ✅ | Ownership-checked CRUD across 6 controllers |
| 6 | Course browsing | ⚠️ | Published list only; **no search/filter** by keyword/category |
| 7 | Unique module ordering | ✅ | OrderNumber + DB unique index (CourseId, OrderNumber) |
| 8 | Student enrollment | ✅ | Published-only + duplicate/active guard + unique index |
| 9 | Online payment via Stripe | ✅ | Checkout session flow (⚠️ WebhookSecret config gap) |
| 10 | Payment → enrollment active | ✅ | Webhook → CompletePaymentAsync sets "Active" |
| 11 | Payment status tracking | ⚠️ | PaymentStatus stored + Enrollment page shows Pending/Active; **no payment-status list page** |
| 12 | Content access control | ✅ | Module/Lesson/Quiz gated on enrollment/ownership |
| 13 | Lesson progress tracking | ⚠️ | Data + service + "Mark as Complete" button ✅, but **no progress list view** |
| 14 | Course progress % | ✅ | CourseProgressService |
| 15 | Automated quiz grading | ✅ | QuizAttemptService score calc |
| 16 | Passing threshold | ✅ | ≥60% |
| 17 | Limited retakes / lockout | ✅ | 3 failed attempts within 24h + no retake after pass |
| 18 | Attempt history | ⚠️ | Attempts stored + Result view; **no attempt-history list page** |
| 19 | Certificate generation | ⚠️ | Record issued after completion ✅, but `CertificateUrl` = empty string — **no PDF/certificate file generated** |
| 20 | One certificate per course | ✅ | Unique index + service check |
| 21 | One feedback per course | ✅ | Unique index + service check |
| 22 | Feedback moderation | ❌ | Status stays "Pending"; **no admin review UI/action** |
| 23 | Admin dashboard | ✅ | Admin/Dashboard with stats + revenue |
| 24 | Admin user management | ✅ | UserController CRUD |
| — | Missing views | ❌ | Home/Profile, Home/Admin, LessonProgress/*, Payment/Details |
| — | Tests / CI | ❌ | No test project, no CI config |

---

# 13. ENVIRONMENT, MIGRATIONS & SECRETS

| Item | Value / status |
|---|---|
| Connection string | `Server=localhost;Database=OnlineLearningDb;Trusted_Connection=True;TrustServerCertificate=True` |
| Migrations | 3 (InitialCreate → AddCourseStatus → AllowCourseCascadeDelete) |
| Stripe keys | **Committed in appsettings.json** (test keys) — ❗ should be user secrets |
| Stripe:WebhookSecret | Not in appsettings — must come from user secrets or webhook fails |
| Admin:Email / Admin:Password | **Not in appsettings** — must be user secrets (AdminSeeder throws otherwise) |
| UserSecretsId | `99ef741c-5a3f-43db-941d-029a2ec595d8` |
| Version control | No git repo; no CI |

Useful commands (verified):
- `dotnet build OnlineLearning.sln` ✅
- `dotnet run --project OnlineLearning.Web --launch-profile http` → http://localhost:5296
- `dotnet ef migrations add <Name> --project OnlineLearning.Data --startup-project OnlineLearning.Web`
- `dotnet ef database update --project OnlineLearning.Data --startup-project OnlineLearning.Web`

---

# 14. SESSION STATE & DI

- **DI is fully manual and centralized** in `Program.cs` (`AddScoped` for every repo/service). No `IServiceCollection` extensions, no Scrutor/reflection scanning.
- No in-memory/distributed caching, no `ISession`/`IDistributedCache` usage; transient messages via **TempData** + `ViewBag`/`ViewData` for page extras (IsOwner, EnrollmentId, Lessons, Quiz).
- No AutoMapper — hand-written DTO mapping helpers per service.

---

# 15. SECURITY

✅ in place:
- Passwords hashed with `PasswordHasher<User>` (never plaintext).
- Anti-forgery tokens on every POST (`[ValidateAntiForgeryToken]`).
- Authorization enforced at controller level with `[Authorize(Roles=…)]`; ownership checks on course/module/lesson/quiz/question/answer/enrollment/payment/certificate/feedback.
- Stripe webhook signature verification (`EventUtility.ConstructEvent`).
- Checkout amount taken from DB and re-validated against course price on the server.

⚠️ gaps:
- **Stripe SecretKey committed in `appsettings.json`** (even if test-key, key material shouldn't live in source).
- `AdminSeeder` executes on every startup and fails the build/run entirely if config missing (no graceful degradation).
- No login rate-limiting / lockout; password policy only via DTO `[StringLength]`/minimum length.
- No authorization claim→role centralization — relies on hardcoded role strings ("Admin"/"Instructor"/"Student") matching claims in controllers and views (`User.IsInRole`).
- Debug `Console.WriteLine` left in `CourseController.Details`.

---

# 16. RELIABILITY & ERROR HANDLING

✅
- Consistent exception-to-result mapping (NotFound/Forbid/TempData+redirect).
- Global error view (`Home/Error`) with RequestId; HSTS + exception handler outside dev.
- Idempotent payment completion; DB-level unique constraints protect against duplicates/races.

⚠️
- **Missing views cause runtime 500s** for 4 action targets (Home Profile/Admin, all LessonProgress pages, Payment/Details).
- N+1 / full-table loads in several services (see §4, §6).
- No pagination, no connection resiliency/retry, no structured logging beyond default `ILogger`.
- No global exception filter or centralized unhandled-exception logging.

---

# 17. TESTING

- **No test project exists** in the solution.
- No unit, integration, or E2E test files (no `*.Tests` anywhere).
- No CI configuration (GitHub Actions / Azure Pipelines / etc.).
- No README documenting how to run tests (none to run).
- Verification relies solely on `dotnet build` + manual runtime.

---

# 18. PUTTING IT ALL TOGETHER

The codebase is a coherent, disciplined 4-project layered solution that implements the **entire learning lifecycle**:

`register (student/instructor)` → `instructor publishes course (draft→published)` → `student browses + enrolls (published-only, pending)` → `Stripe checkout → webhook activates` → `student studies modules/lessons (enrollment-gated)` → `marks lessons complete` → `takes quizzes (60% pass, 3-fail/24h lock)` → `100% progress → certificate + one feedback`.

Strengths: clean entity/config separation, per-entity EF configuration, consistent repository/service/controller layering, DTO-bound views, thorough ownership/authorization checks, DB constraints that enforce the business rules, and honest error handling.

Weaknesses: missing view pages (LessonProgress, Home Profile/Admin, Payment Details), no admin feedback moderation (status stuck pending), no real certificate PDF, no course search, no tests/CI, committed Stripe keys, runtime requirement of user-secrets for boot, use of domain entities in two payment views, and N+1 query patterns.

---

# 19. FINAL STATUS SCORE

| Component | Score (0–100) |
|---|---|
| Architecture & layering | 90 |
| Database design & migrations | 92 |
| Data access (repositories) | 88 |
| Business logic (services) | 88 |
| Controllers & authorization | 88 |
| Views & UI polish | 70 |
| Stripe/payments | 80 |
| Admin & moderation | 55 |
| Security baseline | 82 |
| Testing & CI | **0** |
| **OVERALL** | **~80%** |

Biggest deltas from perfect: missing views (runtime errors), no feedback moderation, no certificate generation, no tests/CI, secrets hygiene.

---

## RECOMMENDED STARTING POINT

**Restore the missing views** — create the `LessonProgress` views (`Index`, `Details`, `Create`) plus `Home/Profile.cshtml`, `Home/Admin.cshtml`, and `Payment/Details.cshtml`. These are the only broken UI paths that currently throw runtime errors when their actions are hit, they complete the student learning loop's visibility, and they are low-risk, self-contained additions that follow the existing DTO + inline-style conventions — requiring no changes to services, controllers, or the database. (Second priority, after this: add a real test project, as the solution currently has zero automated verification.)