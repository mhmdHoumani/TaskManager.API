namespace TaskManager.API.Models
{
  public class TaskItem
  {
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsCompleted { get; set; } = false;
    public DateTime DueDate { get; set; }
    public string Priority { get; set; } = "Medium"; // Low, Medium, High
    public int UserId { get; set; }
    public User? User { get; set; } // Navigation property
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
  }
}