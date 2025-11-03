namespace TaskManager.API.DTOs
{
  public class CreateTaskDto
  {
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public string Priority { get; set; } = "Medium"; // Low, Medium, High
  }
}