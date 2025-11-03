using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.API.DTOs;
using TaskManager.API.Services;

namespace TaskManager.API.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  [Authorize] // Requires authentication for all endpoints
  public class TasksController : ControllerBase
  {
    private readonly TaskService _taskService;

    public TasksController(TaskService taskService)
    {
      _taskService = taskService;
    }

    // Helper method to get current user ID from JWT token
    private int GetCurrentUserId()
    {
      var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      return int.Parse(userIdClaim!);
    }

    // GET: api/tasks
    [HttpGet]
    public async Task<IActionResult> GetAllTasks()
    {
      var userId = GetCurrentUserId();
      var tasks = await _taskService.GetUserTasks(userId);
      return Ok(tasks);
    }

    // GET: api/tasks/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTaskById(int id)
    {
      var userId = GetCurrentUserId();
      var task = await _taskService.GetTaskById(id, userId);

      if (task == null)
      {
        return NotFound(new { message = "Task not found" });
      }

      return Ok(task);
    }

    // POST: api/tasks
    [HttpPost]
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto createTaskDto)
    {
      if (string.IsNullOrWhiteSpace(createTaskDto.Title))
      {
        return BadRequest(new { message = "Title is required" });
      }

      var userId = GetCurrentUserId();
      var task = await _taskService.CreateTask(createTaskDto, userId);

      return CreatedAtAction(nameof(GetTaskById), new { id = task.Id }, task);
    }

    // PUT: api/tasks/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateTaskDto updateTaskDto)
    {
      var userId = GetCurrentUserId();
      var task = await _taskService.UpdateTask(id, updateTaskDto, userId);

      if (task == null)
      {
        return NotFound(new { message = "Task not found" });
      }

      return Ok(task);
    }

    // DELETE: api/tasks/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
      var userId = GetCurrentUserId();
      var result = await _taskService.DeleteTask(id, userId);

      if (!result)
      {
        return NotFound(new { message = "Task not found" });
      }

      return Ok(new { message = "Task deleted successfully" });
    }
  }
}