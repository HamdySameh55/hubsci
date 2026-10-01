namespace OnlineLearning.Domain.Entities;

public class Quiz
{
    public int QuizId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int ModuleId { get; set; }

    // Navigation Properties
    public Module Module { get; set; } = null!;

    public ICollection<Question> Questions { get; set; }
        = new List<Question>();

    public ICollection<QuizAttempt> Attempts { get; set; }
        = new List<QuizAttempt>();
}