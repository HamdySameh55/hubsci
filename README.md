# HubSci — Online Learning Platform

HubSci is a full-stack online learning platform built with **ASP.NET Core MVC** and **SQL Server**. The platform provides role-based learning management for students, instructors, and administrators, including course enrollment, quizzes, progress tracking, certificates, feedback, and payment verification through InstaPay.

## 🚀 Features

* **Role-Based Access Control**

  * Student
  * Instructor
  * Admin

* **Authentication & Authorization**

  * Custom Cookie Authentication
  * Secure password hashing
  * Role-based access control

* **Course Management**

  * Course creation and management
  * Free and paid courses
  * Course modules and lessons
  * Ordered learning content

* **Enrollment & Payments**

  * Course enrollment workflow
  * Pending/active enrollment states
  * InstaPay payment proof upload
  * Instructor payment verification
  * File type and size validation for uploaded proofs

* **Quizzes & Learning Progress**

  * Module-based quizzes
  * Passing score validation
  * Attempt restrictions
  * Lesson completion tracking
  * Progress calculation

* **Certificates**

  * Certificate eligibility after completing course requirements

* **Feedback**

  * Students can submit feedback after completing a course

* **Administration**

  * Admin account seeding
  * User and platform management

## 🏗️ Architecture

The project follows a **4-layer architecture**:

```text
OnlineLearning.Web
        │
        ▼
OnlineLearning.Business
        │
        ▼
OnlineLearning.Data
        │
        ▼
OnlineLearning.Domain
```

### Projects

| Project                   | Responsibility                                               |
| ------------------------- | ------------------------------------------------------------ |
| `OnlineLearning.Domain`   | Domain entities and core models                              |
| `OnlineLearning.Data`     | EF Core, DbContext, configurations, migrations, repositories |
| `OnlineLearning.Business` | Business logic, services, DTOs                               |
| `OnlineLearning.Web`      | ASP.NET Core MVC controllers, views, authentication, and UI  |

## 🛠️ Technologies

* **C#**
* **ASP.NET Core MVC**
* **.NET 9**
* **Entity Framework Core 9.0.13**
* **SQL Server**
* **LINQ**
* **RESTful architecture**
* **Dependency Injection**
* **Repository Pattern**
* **DTO Pattern**
* **Cookie Authentication**
* **Role-Based Authorization**
* **Bootstrap**
* **JavaScript**
* **HTML5 / CSS3**

## 🔐 Security

The application implements:

* Cookie-based authentication
* Role-based authorization
* Password hashing using `PasswordHasher<User>`
* Protected admin functionality
* Validation of payment proof uploads
* User Secrets for sensitive configuration
* Restricted access to uploaded payment proofs

Sensitive configuration such as database connection strings and administrator credentials is **not stored in source control**.

## 💳 Payment Workflow

HubSci uses an InstaPay-based payment verification workflow instead of an automated payment gateway.

```text
Student
   │
   │ Enroll in paid course
   ▼
Payment Required
   │
   │ Upload InstaPay proof
   ▼
Instructor Verification
   │
   ├── Approved ──► Enrollment Activated
   │
   └── Rejected ──► Payment Review Required
```

Free courses bypass the payment verification process.

## 📚 Learning Workflow

```text
Enroll
   ↓
Access Course
   ↓
Study Modules & Lessons
   ↓
Complete Required Lessons
   ↓
Pass Module Quizzes
   ↓
Complete Course
   ↓
Certificate Eligibility
   ↓
Submit Feedback
```

## 🗄️ Database

The application uses **SQL Server** with **Entity Framework Core**.

EF Core is responsible for:

* Database access
* Entity relationships
* Fluent API configurations
* Migrations
* Repository data access

The `ApplicationDbContext` loads entity configurations automatically using:

```csharp
ApplyConfigurationsFromAssembly(...)
```

## ⚙️ Getting Started

### Prerequisites

* .NET 9 SDK
* SQL Server
* Entity Framework Core CLI tools

### Clone the Repository

```bash
git clone https://github.com/HamdySameh55/hubsci.git
cd hubsci
```

### Configure User Secrets

The application requires:

```bash
dotnet user-secrets set "Admin:Email" "<admin-email>" --project OnlineLearning.Web

dotnet user-secrets set "Admin:Password" "<admin-password>" --project OnlineLearning.Web

dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<connection-string>" --project OnlineLearning.Web
```

### Apply Database Migrations

```bash
dotnet ef database update \
  --project OnlineLearning.Data \
  --startup-project OnlineLearning.Web
```

### Build the Solution

```bash
dotnet build OnlineLearning.sln
```

### Run the Application

```bash
dotnet run --project OnlineLearning.Web --launch-profile http
```

The application will be available locally through the configured HTTP launch profile.

## 📁 Project Structure

```text
OnlineLearning/
│
├── OnlineLearning.Domain/
│   └── Entities/
│
├── OnlineLearning.Data/
│   ├── Context/
│   ├── Configurations/
│   ├── Migrations/
│   └── Repositories/
│
├── OnlineLearning.Business/
│   ├── DTOs/
│   └── Services/
│
├── OnlineLearning.Web/
│   ├── Controllers/
│   ├── Data/
│   ├── Services/
│   ├── ViewModels/
│   ├── Views/
│   └── wwwroot/
│
└── OnlineLearning.sln
```

## 🎯 Project Highlights

* Designed and implemented a layered ASP.NET Core MVC architecture.
* Implemented custom authentication and role-based authorization without ASP.NET Identity.
* Applied Entity Framework Core with repository and service layers.
* Implemented course enrollment, learning progress, quizzes, certificates, and feedback workflows.
* Implemented InstaPay payment proof verification.
* Used DTOs to separate business-layer data from domain entities.
* Applied Dependency Injection throughout the application.
* Managed database schema changes using EF Core migrations.

## 👨‍💻 Author

**Hamdy Sameh**

Computer Science Student & .NET Backend Developer

* GitHub: [HamdySameh55](https://github.com/HamdySameh55)
* Repository: [HubSci](https://github.com/HamdySameh55/hubsci)

