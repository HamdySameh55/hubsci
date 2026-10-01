namespace OnlineLearning.Business.DTOs;

public class CreateModuleDto
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int OrderNumber { get; set; }
}