using Microsoft.AspNetCore.Mvc;
using TMS.Models;

namespace TMS.Controller;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private static List<TaskItem> tasks = new List<TaskItem> {
        new TaskItem { Id = 1, TicketNumber = "001", Title = "Task 1", Description = "Aaa", IsCompleted = false, DueDate = DateTime.Now.AddDays(1), Priority = "High", Assigne = new User { Id = 1, UserName = "user1", Email = "1@x.com", FullName = "User 1", Role = "Admin" } },
        new TaskItem { Id = 2, TicketNumber = "002", Title = "Task 2", Description = "Aaa", IsCompleted = false, DueDate = DateTime.Now.AddDays(2), Priority = "Low", Assigne = new User { Id = 1, UserName = "user2", Email = "2@x.com", FullName = "User 1", Role = "Dev" } },
        new TaskItem { Id = 3, TicketNumber = "003", Title = "Task 3", Description = "Aaa", IsCompleted = false, DueDate = DateTime.Now.AddDays(3), Priority = "Medium", Assigne = new User { Id = 1, UserName = "user3", Email = "3@x.com", FullName = "User 1", Role = "Admin" } },
        new TaskItem { Id = 4, TicketNumber = "004", Title = "Task 4", Description = "Aaa", IsCompleted = false, DueDate = DateTime.Now.AddDays(4), Priority = "Low", Assigne = new User { Id = 1, UserName = "user4", Email = "2@x.com", FullName = "User 1", Role = "Dev" } },
        new TaskItem { Id = 5, TicketNumber = "005", Title = "Task 5", Description = "Aaa", IsCompleted = false, DueDate = DateTime.Now.AddDays(5), Priority = "Low", Assigne = new User { Id = 1, UserName = "user3", Email = "3@x.com", FullName = "User 1", Role = "Admin" } },

    };

    [HttpGet]
    public ActionResult<List<TaskItem>> GetTasks()
    {
        return tasks;
    }

    [HttpGet("{id}")]
    public ActionResult<TaskItem> GetTask(int id)
    {
        var index = tasks.FindIndex(u => u.Id == id);
        if (index == -1) return NotFound("Task not found!");

        return tasks[index];
    }

    [HttpPost]
    public ActionResult<TaskItem> CreateTask([FromBody] TaskItem task)
    {
        try
        {
            tasks.Add(task);
        }
        catch
        {
            return null;
        }

        return Created("Task created!", tasks);
    }

    [HttpPut]
    public ActionResult<List<TaskItem>> UpdateTask([FromBody] TaskItem task)
    {
        var index = tasks.FindIndex(u => u.Id == task.Id);
        if (index == -1) return NotFound("Error: Task not found!");

        tasks[index] = task;
        return tasks;
    }

    [HttpDelete("{id}")]
    public ActionResult<List<TaskItem>> DeleteTask(int id)
    {
        var index = tasks.FindIndex(u => u.Id == id);
        if (index == -1) return NotFound("Task not found!");

        tasks.RemoveAt(index);
        return tasks;
    }
}