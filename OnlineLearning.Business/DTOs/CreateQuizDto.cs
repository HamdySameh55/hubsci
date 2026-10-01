
namespace OnlineLearning.Business.DTOs;

public class CreateQuizDto
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int ModuleId { get; set; }
}

