namespace TaskManager.API.DTOs
{
  public class UpdateTaskDto
  {
    public string? Title { get; set; }
    public string? Description { get; set; }
    public bool? IsCompleted { get; set; }
    public DateTime? DueDate { get; set; }
    public string? Priority { get; set; }
  }
}