IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [Users] (
    [UserId] int NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    [Email] nvarchar(150) NOT NULL,
    [PasswordHash] nvarchar(max) NOT NULL,
    [Role] nvarchar(20) NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([UserId])
);

CREATE TABLE [Courses] (
    [CourseId] int NOT NULL IDENTITY,
    [Title] nvarchar(200) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [Price] decimal(18,2) NOT NULL,
    [InstructorId] int NOT NULL,
    CONSTRAINT [PK_Courses] PRIMARY KEY ([CourseId]),
    CONSTRAINT [FK_Courses_Users_InstructorId] FOREIGN KEY ([InstructorId]) REFERENCES [Users] ([UserId]) ON DELETE NO ACTION
);

CREATE TABLE [Enrollments] (
    [EnrollmentId] int NOT NULL IDENTITY,
    [EnrollmentDate] datetime2 NOT NULL,
    [Status] nvarchar(30) NOT NULL,
    [UserId] int NOT NULL,
    [CourseId] int NOT NULL,
    CONSTRAINT [PK_Enrollments] PRIMARY KEY ([EnrollmentId]),
    CONSTRAINT [FK_Enrollments_Courses_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [Courses] ([CourseId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Enrollments_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE NO ACTION
);

CREATE TABLE [Feedbacks] (
    [FeedbackId] int NOT NULL IDENTITY,
    [Rating] int NOT NULL,
    [Comment] nvarchar(1000) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UserId] int NOT NULL,
    [CourseId] int NOT NULL,
    CONSTRAINT [PK_Feedbacks] PRIMARY KEY ([FeedbackId]),
    CONSTRAINT [FK_Feedbacks_Courses_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [Courses] ([CourseId]) ON DELETE CASCADE,
    CONSTRAINT [FK_Feedbacks_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE NO ACTION
);

CREATE TABLE [Modules] (
    [ModuleId] int NOT NULL IDENTITY,
    [Title] nvarchar(200) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [OrderNumber] int NOT NULL,
    [CourseId] int NOT NULL,
    CONSTRAINT [PK_Modules] PRIMARY KEY ([ModuleId]),
    CONSTRAINT [FK_Modules_Courses_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [Courses] ([CourseId]) ON DELETE CASCADE
);

CREATE TABLE [Certificates] (
    [CertificateId] int NOT NULL IDENTITY,
    [IssueDate] datetime2 NOT NULL,
    [CertificateUrl] nvarchar(500) NOT NULL,
    [EnrollmentId] int NOT NULL,
    CONSTRAINT [PK_Certificates] PRIMARY KEY ([CertificateId]),
    CONSTRAINT [FK_Certificates_Enrollments_EnrollmentId] FOREIGN KEY ([EnrollmentId]) REFERENCES [Enrollments] ([EnrollmentId]) ON DELETE CASCADE
);

CREATE TABLE [Payments] (
    [PaymentId] int NOT NULL IDENTITY,
    [Amount] decimal(18,2) NOT NULL,
    [PaymentDate] datetime2 NOT NULL,
    [PaymentStatus] nvarchar(30) NOT NULL,
    [PaymentMethod] nvarchar(50) NOT NULL,
    [EnrollmentId] int NOT NULL,
    CONSTRAINT [PK_Payments] PRIMARY KEY ([PaymentId]),
    CONSTRAINT [FK_Payments_Enrollments_EnrollmentId] FOREIGN KEY ([EnrollmentId]) REFERENCES [Enrollments] ([EnrollmentId]) ON DELETE CASCADE
);

CREATE TABLE [Lessons] (
    [LessonId] int NOT NULL IDENTITY,
    [Title] nvarchar(200) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [ContentType] nvarchar(50) NOT NULL,
    [ContentUrl] nvarchar(500) NOT NULL,
    [ModuleId] int NOT NULL,
    CONSTRAINT [PK_Lessons] PRIMARY KEY ([LessonId]),
    CONSTRAINT [FK_Lessons_Modules_ModuleId] FOREIGN KEY ([ModuleId]) REFERENCES [Modules] ([ModuleId]) ON DELETE CASCADE
);

CREATE TABLE [Quizzes] (
    [QuizId] int NOT NULL IDENTITY,
    [Title] nvarchar(200) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [ModuleId] int NOT NULL,
    CONSTRAINT [PK_Quizzes] PRIMARY KEY ([QuizId]),
    CONSTRAINT [FK_Quizzes_Modules_ModuleId] FOREIGN KEY ([ModuleId]) REFERENCES [Modules] ([ModuleId]) ON DELETE CASCADE
);

CREATE TABLE [LessonProgresses] (
    [ProgressId] int NOT NULL IDENTITY,
    [EnrollmentId] int NOT NULL,
    [LessonId] int NOT NULL,
    [IsCompleted] bit NOT NULL,
    [CompletedAt] datetime2 NULL,
    CONSTRAINT [PK_LessonProgresses] PRIMARY KEY ([ProgressId]),
    CONSTRAINT [FK_LessonProgresses_Enrollments_EnrollmentId] FOREIGN KEY ([EnrollmentId]) REFERENCES [Enrollments] ([EnrollmentId]) ON DELETE CASCADE,
    CONSTRAINT [FK_LessonProgresses_Lessons_LessonId] FOREIGN KEY ([LessonId]) REFERENCES [Lessons] ([LessonId]) ON DELETE NO ACTION
);

CREATE TABLE [Questions] (
    [QuestionId] int NOT NULL IDENTITY,
    [QuestionText] nvarchar(500) NOT NULL,
    [QuizId] int NOT NULL,
    CONSTRAINT [PK_Questions] PRIMARY KEY ([QuestionId]),
    CONSTRAINT [FK_Questions_Quizzes_QuizId] FOREIGN KEY ([QuizId]) REFERENCES [Quizzes] ([QuizId]) ON DELETE CASCADE
);

CREATE TABLE [QuizAttempts] (
    [QuizAttemptId] int NOT NULL IDENTITY,
    [EnrollmentId] int NOT NULL,
    [QuizId] int NOT NULL,
    [Score] decimal(5,2) NOT NULL,
    [AttemptDate] datetime2 NOT NULL,
    [Passed] bit NOT NULL,
    [AttemptNumber] int NOT NULL,
    CONSTRAINT [PK_QuizAttempts] PRIMARY KEY ([QuizAttemptId]),
    CONSTRAINT [FK_QuizAttempts_Enrollments_EnrollmentId] FOREIGN KEY ([EnrollmentId]) REFERENCES [Enrollments] ([EnrollmentId]) ON DELETE CASCADE,
    CONSTRAINT [FK_QuizAttempts_Quizzes_QuizId] FOREIGN KEY ([QuizId]) REFERENCES [Quizzes] ([QuizId]) ON DELETE NO ACTION
);

CREATE TABLE [Answers] (
    [AnswerId] int NOT NULL IDENTITY,
    [AnswerText] nvarchar(300) NOT NULL,
    [IsCorrect] bit NOT NULL,
    [QuestionId] int NOT NULL,
    CONSTRAINT [PK_Answers] PRIMARY KEY ([AnswerId]),
    CONSTRAINT [FK_Answers_Questions_QuestionId] FOREIGN KEY ([QuestionId]) REFERENCES [Questions] ([QuestionId]) ON DELETE CASCADE
);

CREATE INDEX [IX_Answers_QuestionId] ON [Answers] ([QuestionId]);

CREATE UNIQUE INDEX [IX_Certificates_EnrollmentId] ON [Certificates] ([EnrollmentId]);

CREATE INDEX [IX_Courses_InstructorId] ON [Courses] ([InstructorId]);

CREATE INDEX [IX_Enrollments_CourseId] ON [Enrollments] ([CourseId]);

CREATE UNIQUE INDEX [IX_Enrollments_UserId_CourseId] ON [Enrollments] ([UserId], [CourseId]);

CREATE INDEX [IX_Feedbacks_CourseId] ON [Feedbacks] ([CourseId]);

CREATE UNIQUE INDEX [IX_Feedbacks_UserId_CourseId] ON [Feedbacks] ([UserId], [CourseId]);

CREATE UNIQUE INDEX [IX_LessonProgresses_EnrollmentId_LessonId] ON [LessonProgresses] ([EnrollmentId], [LessonId]);

CREATE INDEX [IX_LessonProgresses_LessonId] ON [LessonProgresses] ([LessonId]);

CREATE INDEX [IX_Lessons_ModuleId] ON [Lessons] ([ModuleId]);

CREATE UNIQUE INDEX [IX_Modules_CourseId_OrderNumber] ON [Modules] ([CourseId], [OrderNumber]);

CREATE INDEX [IX_Payments_EnrollmentId] ON [Payments] ([EnrollmentId]);

CREATE INDEX [IX_Questions_QuizId] ON [Questions] ([QuizId]);

CREATE INDEX [IX_QuizAttempts_EnrollmentId] ON [QuizAttempts] ([EnrollmentId]);

CREATE INDEX [IX_QuizAttempts_QuizId] ON [QuizAttempts] ([QuizId]);

CREATE UNIQUE INDEX [IX_Quizzes_ModuleId] ON [Quizzes] ([ModuleId]);

CREATE UNIQUE INDEX [IX_Users_Email] ON [Users] ([Email]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260907080459_InitialCreate', N'9.0.13');

DROP INDEX [IX_Payments_EnrollmentId] ON [Payments];

ALTER TABLE [Feedbacks] ADD [Status] nvarchar(30) NOT NULL DEFAULT N'';

ALTER TABLE [Courses] ADD [Status] nvarchar(max) NOT NULL DEFAULT N'';

CREATE UNIQUE INDEX [IX_Payments_EnrollmentId] ON [Payments] ([EnrollmentId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260910203429_AddCourseStatus', N'9.0.13');

ALTER TABLE [Enrollments] DROP CONSTRAINT [FK_Enrollments_Courses_CourseId];

ALTER TABLE [Enrollments] ADD CONSTRAINT [FK_Enrollments_Courses_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [Courses] ([CourseId]) ON DELETE CASCADE;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260912161548_AllowCourseCascadeDelete', N'9.0.13');

ALTER TABLE [Payments] ADD [ProofFilePath] nvarchar(300) NULL;

ALTER TABLE [Payments] ADD [TransactionReference] nvarchar(100) NULL;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260916113337_ReplaceStripeWithInstaPay', N'9.0.13');

COMMIT;
GO

