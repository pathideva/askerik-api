namespace AskErik.LessonApi.Models;

public class CosmosSettings
{
    public string AccountEndpoint { get; set; } = string.Empty;
    public string AccountKey { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = "LessonsDb";
    public string ContainerName { get; set; } = "Lessons";
}
