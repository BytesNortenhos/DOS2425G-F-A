using Microsoft.AspNetCore.Mvc;
using TMS.Models;

namespace TMS.Controller;

[ApiController]
[Route("api/[controller]")]
public class ProjectController : ControllerBase
{
    private static List<Project> projects = new List<Project> {
        new Project { Id = 1, Name = "Project 1", Description = "Sistema de Gerenciamento de Tarefas", StartDate = DateTime.Parse("2023-01-01"), EndDate = DateTime.Parse("2023-12-31"), Tasks = new List<TaskItem> { new TaskItem { Id = 1, TicketNumber = "001", Title = "Task 1", Description = "Aaa", IsCompleted = false, DueDate = DateTime.Now.AddDays(1), Priority = "High", Assigne = new User { Id = 1, UserName = "user1", Email = "1@x.com", FullName = "User 1", Role = "Admin" } }, new TaskItem { Id = 2, TicketNumber = "002", Title = "Task 2", Description = "Aaa", IsCompleted = false, DueDate = DateTime.Now.AddDays(2), Priority = "Low", Assigne = new User { Id = 1, UserName = "user2", Email = "2@x.com", FullName = "User 1", Role = "Dev" } } } },
        new Project { Id = 2, Name = "Project 2", Description = "Aplicativo de Fitness", StartDate = DateTime.Parse("2023-02-01"), EndDate = DateTime.Parse("2023-11-30"), Tasks = new List<TaskItem> { new TaskItem { Id = 1, TicketNumber = "001", Title = "Task 1", Description = "Aaa", IsCompleted = false, DueDate = DateTime.Now.AddDays(1), Priority = "High", Assigne = new User { Id = 1, UserName = "user1", Email = "1@x.com", FullName = "User 1", Role = "Admin" } }, new TaskItem { Id = 2, TicketNumber = "002", Title = "Task 2", Description = "Aaa", IsCompleted = false, DueDate = DateTime.Now.AddDays(2), Priority = "Low", Assigne = new User { Id = 1, UserName = "user2", Email = "2@x.com", FullName = "User 1", Role = "Dev" } } } },
        new Project { Id = 3, Name = "Project 3", Description = "Plataforma de E-commerce", StartDate = DateTime.Parse("2023-03-01"), EndDate = DateTime.Parse("2023-10-31"), Tasks = new List<TaskItem> { new TaskItem { Id = 1, TicketNumber = "001", Title = "Task 1", Description = "Aaa", IsCompleted = false, DueDate = DateTime.Now.AddDays(1), Priority = "High", Assigne = new User { Id = 1, UserName = "user1", Email = "1@x.com", FullName = "User 1", Role = "Admin" } }, new TaskItem { Id = 2, TicketNumber = "002", Title = "Task 2", Description = "Aaa", IsCompleted = false, DueDate = DateTime.Now.AddDays(2), Priority = "Low", Assigne = new User { Id = 1, UserName = "user2", Email = "2@x.com", FullName = "User 1", Role = "Dev" } } } },
        new Project { Id = 4, Name = "Project 4", Description = "Ferramenta de Análise", StartDate = DateTime.Parse("2023-04-01"), EndDate = DateTime.Parse("2023-09-30"), Tasks = new List<TaskItem> { new TaskItem { Id = 1, TicketNumber = "001", Title = "Task 1", Description = "Aaa", IsCompleted = false, DueDate = DateTime.Now.AddDays(1), Priority = "High", Assigne = new User { Id = 1, UserName = "user1", Email = "1@x.com", FullName = "User 1", Role = "Admin" } }, new TaskItem { Id = 2, TicketNumber = "002", Title = "Task 2", Description = "Aaa", IsCompleted = false, DueDate = DateTime.Now.AddDays(2), Priority = "Low", Assigne = new User { Id = 1, UserName = "user2", Email = "2@x.com", FullName = "User 1", Role = "Dev" } } } },
        new Project { Id = 5, Name = "Project 5", Description = "Sistema de Gestão de Inventário", StartDate = DateTime.Parse("2023-05-01"), EndDate = DateTime.Parse("2023-08-31"), Tasks = new List<TaskItem> { new TaskItem { Id = 1, TicketNumber = "001", Title = "Task 1", Description = "Aaa", IsCompleted = false, DueDate = DateTime.Now.AddDays(1), Priority = "High", Assigne = new User { Id = 1, UserName = "user1", Email = "1@x.com", FullName = "User 1", Role = "Admin" } }, new TaskItem { Id = 2, TicketNumber = "002", Title = "Task 2", Description = "Aaa", IsCompleted = false, DueDate = DateTime.Now.AddDays(2), Priority = "Low", Assigne = new User { Id = 1, UserName = "user2", Email = "2@x.com", FullName = "User 1", Role = "Dev" } } } },
    };


    [HttpGet]
    public ActionResult<List<Project>> GetProjects()
    {
        return projects;
    }

    [HttpGet("{id}")]
    public ActionResult<Project> GetProject(int id)
    {
        var index = projects.FindIndex(u => u.Id == id);
        if (index == -1) return NotFound("Error: Project not found!");

        return projects[index];
    }

    [HttpPost]
    public ActionResult<List<Project>> CreateProject([FromBody] Project project)
    {
        try
        {
            projects.Add(project);
        }
        catch
        {
            return null;
        }

        return Created("Project created!", projects);
    }

    [HttpPut]
    public ActionResult<List<Project>> UpdateProject([FromBody] Project project)
    {
        var index = projects.FindIndex(u => u.Id == project.Id);
        if (index == -1) return NotFound("Error: Project not found!");

        projects[index] = project;
        return projects;
    }

    [HttpDelete("{id}")]
    public ActionResult<List<Project>> DeleteProject(int id)
    {
        var index = projects.FindIndex(u => u.Id == id);
        if (index == -1) return NotFound("Error: Project not found!");

        projects.RemoveAt(index);
        return projects;
    }






}