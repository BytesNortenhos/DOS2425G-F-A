using Microsoft.AspNetCore.Mvc;
using TMS.Models;

namespace TMS.Controller;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private static readonly List<TaskItem> tasks = new List<TaskItem>
    {
        new TaskItem
        {
            Id = 1,
            TicketNumber = "JS-1203",
            Title = "Bug reported - fix",
            Description = "A bug was detected in Service X",
            IsCompleted = false,
            DueDate = DateTime.Now.AddDays(2)
        },
        new TaskItem
        {
            Id = 2,
            TicketNumber = "AB-352",
            Title = "New Functionality - use C#",
            Description = "It's necessary use .net core in these lessons",
            IsCompleted = true,
            DueDate = DateTime.Now.AddDays(-1)
        },
        new TaskItem
        {
            Id = 3,
            TicketNumber = "AA-9855",
            Title = "Improvements ",
            Description = "Create new stories to implement new features",
            IsCompleted = false,
            DueDate = DateTime.Now.AddDays(5)
        }
    };
    
    [HttpGet]
    public IActionResult GetAllTasks()
    {
        return Ok(tasks);
    }
}