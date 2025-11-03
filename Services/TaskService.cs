using Microsoft.EntityFrameworkCore;
using TaskManager.API.Data;
using TaskManager.API.DTOs;
using TaskManager.API.Models;

namespace TaskManager.API.Services
{
  public class TaskService
  {
    private readonly AppDbContext _context;

    public TaskService(AppDbContext context)
    {
      _context = context;
    }

    // Get all tasks for a user
    public async Task<List<TaskResponseDto>> GetUserTasks(int userId)
    {
      var tasks = await _context.Tasks
          .Where(t => t.UserId == userId)
          .OrderByDescending(t => t.CreatedAt)
          .Select(t => new TaskResponseDto
          {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            IsCompleted = t.IsCompleted,
            DueDate = t.DueDate,
            Priority = t.Priority,
            UserId = t.UserId,
            CreatedAt = t.CreatedAt,
            UpdatedAt = t.UpdatedAt
          })
          .ToListAsync();

      return tasks;
    }

    // Get a single task by ID
    public async Task<TaskResponseDto?> GetTaskById(int taskId, int userId)
    {
      var task = await _context.Tasks
          .Where(t => t.Id == taskId && t.UserId == userId)
          .Select(t => new TaskResponseDto
          {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            IsCompleted = t.IsCompleted,
            DueDate = t.DueDate,
            Priority = t.Priority,
            UserId = t.UserId,
            CreatedAt = t.CreatedAt,
            UpdatedAt = t.UpdatedAt
          })
          .FirstOrDefaultAsync();

      return task;
    }

    // Create a new task
    public async Task<TaskResponseDto> CreateTask(CreateTaskDto createTaskDto, int userId)
    {
      var task = new TaskItem
      {
        Title = createTaskDto.Title,
        Description = createTaskDto.Description,
        DueDate = createTaskDto.DueDate,
        Priority = createTaskDto.Priority,
        UserId = userId,
        IsCompleted = false,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
      };

      _context.Tasks.Add(task);
      await _context.SaveChangesAsync();

      return new TaskResponseDto
      {
        Id = task.Id,
        Title = task.Title,
        Description = task.Description,
        IsCompleted = task.IsCompleted,
        DueDate = task.DueDate,
        Priority = task.Priority,
        UserId = task.UserId,
        CreatedAt = task.CreatedAt,
        UpdatedAt = task.UpdatedAt
      };
    }

    // Update a task
    public async Task<TaskResponseDto?> UpdateTask(int taskId, UpdateTaskDto updateTaskDto, int userId)
    {
      var task = await _context.Tasks
          .FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);

      if (task == null)
      {
        return null;
      }

      // Update only provided fields
      if (updateTaskDto.Title != null)
        task.Title = updateTaskDto.Title;

      if (updateTaskDto.Description != null)
        task.Description = updateTaskDto.Description;

      if (updateTaskDto.IsCompleted.HasValue)
        task.IsCompleted = updateTaskDto.IsCompleted.Value;

      if (updateTaskDto.DueDate.HasValue)
        task.DueDate = updateTaskDto.DueDate.Value;

      if (updateTaskDto.Priority != null)
        task.Priority = updateTaskDto.Priority;

      task.UpdatedAt = DateTime.UtcNow;

      await _context.SaveChangesAsync();

      return new TaskResponseDto
      {
        Id = task.Id,
        Title = task.Title,
        Description = task.Description,
        IsCompleted = task.IsCompleted,
        DueDate = task.DueDate,
        Priority = task.Priority,
        UserId = task.UserId,
        CreatedAt = task.CreatedAt,
        UpdatedAt = task.UpdatedAt
      };
    }

    // Delete a task
    public async Task<bool> DeleteTask(int taskId, int userId)
    {
      var task = await _context.Tasks
          .FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);

      if (task == null)
      {
        return false;
      }

      _context.Tasks.Remove(task);
      await _context.SaveChangesAsync();

      return true;
    }
  }
}