using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using OnlineLearning.Business.Services.Implementations;
using OnlineLearning.Business.Services.Interfaces;

using OnlineLearning.Data.Context;
using OnlineLearning.Data.Repositories.Implementations;
using OnlineLearning.Data.Repositories.Interfaces;

using OnlineLearning.Domain.Entities;

using OnlineLearning.Web.Data;
using OnlineLearning.Web.Services;

var builder = WebApplication.CreateBuilder(args);


// =====================================================
// MVC
// =====================================================

builder.Services.AddControllersWithViews();


// =====================================================
// Authentication & Authorization
// =====================================================

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";

        if (!builder.Environment.IsDevelopment())
        {
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.Lax;
        }
    });

builder.Services.AddAuthorization();


// =====================================================
// Database
// =====================================================

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddSingleton<PaymentProofUploader>();


// =====================================================
// Repositories
// =====================================================

builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<IModuleRepository, ModuleRepository>();
builder.Services.AddScoped<ILessonRepository, LessonRepository>();

builder.Services.AddScoped<IQuizRepository, QuizRepository>();
builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
builder.Services.AddScoped<IAnswerRepository, AnswerRepository>();
builder.Services.AddScoped<IQuizAttemptRepository, QuizAttemptRepository>();

builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();

builder.Services.AddScoped<ILessonProgressRepository, LessonProgressRepository>();
builder.Services.AddScoped<ICertificateRepository, CertificateRepository>();
builder.Services.AddScoped<IFeedbackRepository, FeedbackRepository>();

builder.Services.AddScoped<IUserRepository, UserRepository>();


// =====================================================
// Services
// =====================================================

builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IModuleService, ModuleService>();
builder.Services.AddScoped<ILessonService, LessonService>();

builder.Services.AddScoped<IQuizService, QuizService>();
builder.Services.AddScoped<IQuestionService, QuestionService>();
builder.Services.AddScoped<IAnswerService, AnswerService>();
builder.Services.AddScoped<IQuizAttemptService, QuizAttemptService>();

builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

builder.Services.AddScoped<ILessonProgressService, LessonProgressService>();
builder.Services.AddScoped<ICourseProgressService, CourseProgressService>();

builder.Services.AddScoped<ICertificateService, CertificateService>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();

builder.Services.AddScoped<IUserService, UserService>();


// =====================================================
// Admin
// =====================================================

builder.Services.AddScoped<IAdminDashboardService, AdminDashboardService>();


// =====================================================
// Authentication Services
// =====================================================

builder.Services.AddScoped<PasswordHasher<User>>();
builder.Services.AddScoped<IAuthService, AuthService>();


// =====================================================
// Build Application
// =====================================================

var app = builder.Build();


// =====================================================
// Seed Admin Account
// =====================================================

await AdminSeeder.SeedAsync(app.Services);


// =====================================================
// HTTP Request Pipeline
// =====================================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();


// Authentication MUST come before Authorization
app.UseAuthentication();
app.UseAuthorization();


// =====================================================
// Routes
// =====================================================

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


// =====================================================
// Run
// =====================================================

app.Run();