namespace AskErik.LessonApi.Models;

public enum Difficulty
{
    Beginner,
    Intermediate,
    Advanced
}

public record Material
{
    public string Name { get; set; } = string.Empty;
    public string Quantity { get; set; } = string.Empty;
}

public class LessonWorkshop
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
}

public class LessonExpert
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class LessonHelper
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
