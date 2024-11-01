using Microsoft.AspNetCore.Mvc;
using TMS.Models;

namespace TMS.Controller;

[ApiController]
[Route("api/[controller]")]
public class ProjectController : ControllerBase
{
    private static List<Project> projects = new List<Project> {
        new Project { Id = 1, Name = "Project 1", Description = "Sistema de Gerenciamento de Tarefas", StartDate = DateTime.Parse("2023-01-01"), EndDate = DateTime.Parse("2023-12-31") },
        new Project { Id = 2, Name = "Project 2", Description = "Aplicativo de Fitness", StartDate = DateTime.Parse("2023-02-01"), EndDate = DateTime.Parse("2023-11-30") },
        new Project { Id = 3, Name = "Project 3", Description = "Plataforma de E-commerce", StartDate = DateTime.Parse("2023-03-01"), EndDate = DateTime.Parse("2023-10-31") },
        new Project { Id = 4, Name = "Project 4", Description = "Ferramenta de Análise", StartDate = DateTime.Parse("2023-04-01"), EndDate = DateTime.Parse("2023-09-30") },
        new Project { Id = 5, Name = "Project 5", Description = "Sistema de Gestão de Inventário", StartDate = DateTime.Parse("2023-05-01"), EndDate = DateTime.Parse("2023-08-31") },
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