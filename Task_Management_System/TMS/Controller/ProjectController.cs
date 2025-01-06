using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TMS.Models;

namespace TMS.Controller;

[ApiController]
[Route("api/[controller]")]
public class ProjectController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProjectController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
    {
        var projects = await _context.Projects.ToListAsync();
        return Ok(projects);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Project>> GetProjectById(int id)
    {
        var project = await _context.Projects.FindAsync(id);

        if (project == null)
        {
            return NotFound("Project not found.");
        }

        return Ok(project);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProject([FromBody] ProjectDTO projectDTO)
    {
        if (projectDTO == null)
        {
            return BadRequest("Project data is required.");
        }

        var project = new Project
        {
            ProjectName = projectDTO.ProjectName,
            Description = projectDTO.Description,
            StartDate = projectDTO.StartDate,
            EndDate = projectDTO.EndDate
        };

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProjectById), new { id = project.Id }, project);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProject(int id, [FromBody] ProjectDTO projectDTO)
    {
        if (projectDTO == null)
        {
            return BadRequest("Project data is required.");
        }

        var project = await _context.Projects.FindAsync(id);

        if (project == null)
        {
            return NotFound("Project not found.");
        }

        // Atualizar os campos do project
        project.ProjectName = projectDTO.ProjectName;
        project.Description = projectDTO.Description;
        project.StartDate = projectDTO.StartDate;
        project.EndDate = projectDTO.EndDate;

        _context.Projects.Update(project);
        await _context.SaveChangesAsync();

        return NoContent(); // Retorna 204 No Content, indica sucesso na atualização
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        var project = await _context.Projects.FindAsync(id);

        if (project == null)
        {
            return NotFound("Project not found.");
        }

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();

        return NoContent(); // Retorna 204 No Content, indica que o project foi excluído
    }
}