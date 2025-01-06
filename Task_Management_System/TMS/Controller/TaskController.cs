using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TMS.Models;

namespace TMS.Controller;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly AppDbContext _context;

    public TasksController(AppDbContext context)
    {
        _context = context;
    }

    
    [HttpGet]
    public async Task<IActionResult> GetTaskItems()
    {
        var taskItems = await _context.TaskItems
            .Include(t => t.Assigne)
            .Include(t => t.Project)
            .Include(t => t.Comments)
            .ToListAsync();

        var taskItemDtos = taskItems.Select(t => new TaskDTO
        {
            Id = t.Id,
            TicketNumber = t.TicketNumber,
            Title = t.Title,
            Description = t.Description,
            IsCompleted = t.IsCompleted,
            DueDate = t.DueDate,
            Priority = t.Priority,
            AssigneId = t.AssigneId,
            Assigne = new UserDTO
            {
                UserName = t.Assigne.UserName,
                Email = t.Assigne.Email,
                FullName = t.Assigne.FullName,
                Role = t.Assigne.Role
            },
            ProjectId = t.ProjectId,
            Project = new ProjectDTO
            {
                ProjectName = t.Project.ProjectName,
                Description = t.Project.Description
            }
        }).ToList();

        return Ok(taskItemDtos);
    }

    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTaskItemById(int id)
    {
        var taskItem = await _context.TaskItems
            .Include(t => t.Assigne)
            .Include(t => t.Project)
            .Include(t => t.Comments)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (taskItem == null)
        {
            return NotFound("TaskItem not found.");
        }

        var taskItemDto = new TaskDTO
        {
            Id = taskItem.Id,
            TicketNumber = taskItem.TicketNumber,
            Title = taskItem.Title,
            Description = taskItem.Description,
            IsCompleted = taskItem.IsCompleted,
            DueDate = taskItem.DueDate,
            Priority = taskItem.Priority,
            AssigneId = taskItem.AssigneId,
            Assigne = new UserDTO
            {
                UserName = taskItem.Assigne.UserName,
                Email = taskItem.Assigne.Email,
                FullName = taskItem.Assigne.FullName,
                Role = taskItem.Assigne.Role
            },
            ProjectId = taskItem.ProjectId,
            Project = new ProjectDTO
            {
                ProjectName = taskItem.Project.ProjectName,
                Description = taskItem.Project.Description
            }
        };

        return Ok(taskItemDto);
    }

    
    [HttpPost]
    public async Task<IActionResult> CreateTaskItem([FromBody] TaskDTO taskItemDto)
    {
        if (taskItemDto == null)
        {
            return BadRequest("TaskItem data is required.");
        }

        var taskItem = new TaskItem
        {
            TicketNumber = taskItemDto.TicketNumber,
            Title = taskItemDto.Title,
            Description = taskItemDto.Description,
            IsCompleted = taskItemDto.IsCompleted,
            DueDate = taskItemDto.DueDate,
            Priority = taskItemDto.Priority,
            AssigneId = taskItemDto.AssigneId,
            ProjectId = taskItemDto.ProjectId
        };

        _context.TaskItems.Add(taskItem);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTaskItemById), new { id = taskItem.Id }, taskItem);
    }

    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTaskItem(int id, [FromBody] TaskDTO taskItemDto)
    {
        if (taskItemDto == null)
        {
            return BadRequest("TaskItem data is required.");
        }

        var taskItem = await _context.TaskItems.FindAsync(id);

        if (taskItem == null)
        {
            return NotFound("TaskItem not found.");
        }

        taskItem.TicketNumber = taskItemDto.TicketNumber;
        taskItem.Title = taskItemDto.Title;
        taskItem.Description = taskItemDto.Description;
        taskItem.IsCompleted = taskItemDto.IsCompleted;
        taskItem.DueDate = taskItemDto.DueDate;
        taskItem.Priority = taskItemDto.Priority;
        taskItem.AssigneId = taskItemDto.AssigneId;
        taskItem.ProjectId = taskItemDto.ProjectId;

        _context.TaskItems.Update(taskItem);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTaskItem(int id)
    {
        var taskItem = await _context.TaskItems.FindAsync(id);

        if (taskItem == null)
        {
            return NotFound("TaskItem not found.");
        }

        _context.TaskItems.Remove(taskItem);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}