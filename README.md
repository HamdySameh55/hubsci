# HubSci — Online Learning Platform

> A full-stack online learning platform built with ASP.NET Core MVC, .NET 9, Entity Framework Core, and SQL Server.

### 🔗 Links

* **Live Demo:** http://hubsci.runasp.net/
* **GitHub:** https://github.com/HamdySameh55/hubsci

---

## 📌 Overview

HubSci is a web-based online learning platform that provides a complete learning workflow for **Students, Instructors, and Administrators**.

The platform supports course management, enrollment, lessons, quizzes, progress tracking, certificates, student feedback, and InstaPay payment-proof verification.

---

## 🚀 Features

### 👨‍🎓 Student

* Browse available courses
* Enroll in free and paid courses
* Upload InstaPay payment proof for paid courses
* Access enrolled courses
* Study modules and lessons
* Take module quizzes
* Track learning progress
* Receive certificates after completing course requirements
* Submit course feedback after completion

### 👨‍🏫 Instructor

* Create and manage courses
* Create modules and lessons
* Add module quizzes
* Review student enrollments
* Verify InstaPay payment proofs
* Monitor course-related activities

### 👨‍💼 Admin

* Administrative access
* User management
* Platform management
* Automatic admin account seeding

---

## 🏗️ Architecture

HubSci follows a **4-layer architecture**:

```text
┌─────────────────────────────┐
│     OnlineLearning.Web      │
│ Controllers / Views / UI    │
└──────────────┬──────────────┘
               │
┌──────────────▼──────────────┐
│   OnlineLearning.Business   │
│ Services / DTOs / Logic     │
└──────────────┬──────────────┘
               │
┌──────────────▼──────────────┐
│     OnlineLearning.Data     │
│ EF Core / Repositories / DB │
└──────────────┬──────────────┘
               │
┌──────────────▼──────────────┐
│    OnlineLearning.Domain    │
│         Entities            │
└─────────────────────────────┘
```

### Layer Responsibilities

| Layer        | Responsibility                                               |
| ------------ | ------------------------------------------------------------ |
| **Domain**   | Entities and domain models                                   |
| **Data**     | EF Core, DbContext, configurations, migrations, repositories |
| **Business** | Services, DTOs, and business logic                           |
| **Web**      | MVC controllers, views, authentication, and UI               |

---

## 🛠️ Technologies

* C#
* .NET 9
* ASP.NET Core MVC
* Entity Framework Core 9.0.13
* SQL Server
* LINQ
* Dependency Injection
* Repository Pattern
* Service Layer
* DTO Pattern
* Cookie Authentication
* Role-Based Authorization
* Bootstrap
* JavaScript
* HTML5
* CSS3

---

## 🔐 Authentication & Security

The application uses **custom Cookie Authentication** instead of ASP.NET Identity.

Implemented security features include:

* Cookie-based authentication
* Role-based authorization
* Password hashing using `PasswordHasher<User>`
* Protected administrative functionality
* User Secrets for sensitive configuration
* Payment proof file validation
* Restricted access to uploaded payment proofs

Sensitive credentials and database connection strings are not stored in the repository.

---

## 💳 Payment Workflow

HubSci implements an InstaPay-based payment verification workflow.

```text
Student
   │
   ▼
Enroll in Paid Course
   │
   ▼
Upload Payment Proof
   │
   ▼
Instructor Reviews Proof
   │
   ├───────────────┐
   │               │
Approved        Rejected
   │               │
   ▼               ▼
Enrollment       Payment
Activated        Review Required
```

Free courses bypass the payment verification process.

---

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

---

## 🗄️ Database & Data Access

The application uses **SQL Server** with **Entity Framework Core**.

EF Core is used for:

* Database access
* Entity relationships
* Fluent API configurations
* Database migrations
* Repository-based data access

Entity configurations are automatically loaded through:

```csharp
modelBuilder.ApplyConfigurationsFromAssembly(
    typeof(ApplicationDbContext).Assembly
);
```

---

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

---

## ⚙️ Getting Started

### Prerequisites

* .NET 9 SDK
* SQL Server
* Entity Framework Core CLI

### Clone

```bash
git clone https://github.com/HamdySameh55/hubsci.git
cd hubsci
```

### Configure User Secrets

```bash
dotnet user-secrets set "Admin:Email" "<admin-email>" --project OnlineLearning.Web

dotnet user-secrets set "Admin:Password" "<admin-password>" --project OnlineLearning.Web

dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<connection-string>" --project OnlineLearning.Web
```

### Apply Database Migrations

```bash
dotnet ef database update --project OnlineLearning.Data --startup-project OnlineLearning.Web
```

### Build

```bash
dotnet build OnlineLearning.sln
```

### Run

```bash
dotnet run --project OnlineLearning.Web --launch-profile http
```

---

## 🎯 Project Highlights

* Built a complete online learning platform using **ASP.NET Core MVC and .NET 9**.
* Designed a **4-layer architecture** separating Domain, Data, Business, and Web responsibilities.
* Implemented **custom Cookie Authentication and role-based authorization** for three user roles.
* Applied **Repository Pattern, Service Layer, DTOs, and Dependency Injection**.
* Implemented course enrollment, quizzes, progress tracking, certificates, and student feedback.
* Developed an **InstaPay payment-proof verification workflow** for paid courses.
* Used **Entity Framework Core migrations and SQL Server** for database management.
* Deployed the application as a live web application.

---

## 🌐 Live Application

**Try HubSci:**
http://hubsci.runasp.net/

---

## 👨‍💻 Author

### Hamdy Sameh

Computer Science Student & .NET Backend Developer

* GitHub: https://github.com/HamdySameh55
* Project: https://github.com/HamdySameh55/hubsci
