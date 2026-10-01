using OnlineLearning.Business.DTOs;
using OnlineLearning.Business.Services.Interfaces;
using OnlineLearning.Data.Repositories.Interfaces;

namespace OnlineLearning.Business.Services.Implementations;

public class CourseProgressService : ICourseProgressService
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly ILessonRepository _lessonRepository;
    private readonly ILessonProgressRepository _lessonProgressRepository;
    private readonly IQuizRepository _quizRepository;
    private readonly IQuizAttemptRepository _quizAttemptRepository;

    public CourseProgressService(
        IEnrollmentRepository enrollmentRepository,
        ILessonRepository lessonRepository,
        ILessonProgressRepository lessonProgressRepository,
        IQuizRepository quizRepository,
        IQuizAttemptRepository quizAttemptRepository)
    {
        _enrollmentRepository = enrollmentRepository;
        _lessonRepository = lessonRepository;
        _lessonProgressRepository = lessonProgressRepository;
        _quizRepository = quizRepository;
        _quizAttemptRepository = quizAttemptRepository;
    }

    public async Task<CourseProgressDto?> GetProgressAsync(
        int enrollmentId)
    {
        // 1. Get enrollment
        var enrollment =
            await _enrollmentRepository.GetByIdAsync(
                enrollmentId);

        if (enrollment == null)
        {
            return null;
        }

        // 2. Get all lessons in the course
        var lessons =
            await _lessonRepository.GetByCourseIdAsync(
                enrollment.CourseId);

        var lessonList = lessons.ToList();

        // 3. Get student's lesson progress
        var progressRecords =
            await _lessonProgressRepository
                .GetByEnrollmentIdAsync(
                    enrollmentId);

        var progressList =
            progressRecords.ToList();

        // 4. Count completed lessons
        int completedLessons =
            lessonList.Count(lesson =>
                progressList.Any(progress =>
                    progress.LessonId == lesson.LessonId &&
                    progress.IsCompleted));

        // 5. Get all quizzes in the course
        var quizzes =
            await _quizRepository.GetByCourseIdAsync(
                enrollment.CourseId);

        var quizList = quizzes.ToList();

        // 6. Get all quiz attempts
        var allAttempts =
            await _quizAttemptRepository.GetAllAsync();

        // 7. Get attempts for this enrollment only
        var enrollmentAttempts =
            allAttempts
                .Where(attempt =>
                    attempt.EnrollmentId == enrollmentId)
                .ToList();

        // 8. Count passed quizzes
        int passedQuizzes =
            quizList.Count(quiz =>
                enrollmentAttempts.Any(attempt =>
                    attempt.QuizId == quiz.QuizId &&
                    attempt.Passed));

        // 9. Calculate total items
        int totalItems =
            lessonList.Count +
            quizList.Count;

        // 10. Calculate completed items
        int completedItems =
            completedLessons +
            passedQuizzes;

        // 11. Calculate progress percentage
        decimal progressPercentage =
            totalItems == 0
                ? 0
                : (decimal)completedItems /
                  totalItems *
                  100;

        // 12. Check if all lessons are completed
        bool allLessonsCompleted =
            completedLessons == lessonList.Count;

        // 13. Check if all quizzes are passed
        //
        // If the course has no quizzes,
        // this will be TRUE automatically.
        bool allQuizzesPassed =
            passedQuizzes == quizList.Count;

        // 14. Course must contain at least
        // one lesson or one quiz
        bool hasContent =
            lessonList.Any() ||
            quizList.Any();

        // 15. Final completion rule
        //
        // Student is considered completed when:
        // - Course has content
        // - All lessons are completed
        // - All quizzes are passed
        //
        // If there are no quizzes,
        // allQuizzesPassed = true.
        bool isCompleted =
            hasContent &&
            allLessonsCompleted &&
            allQuizzesPassed;

        // 16. Return progress DTO
        return new CourseProgressDto
        {
            EnrollmentId = enrollmentId,

            CourseId = enrollment.CourseId,

            TotalLessons =
                lessonList.Count,

            CompletedLessons =
                completedLessons,

            TotalQuizzes =
                quizList.Count,

            PassedQuizzes =
                passedQuizzes,

            ProgressPercentage =
                progressPercentage,

            IsCompleted =
                isCompleted
        };
    }
}