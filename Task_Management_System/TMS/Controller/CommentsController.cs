using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TMS.Models;

namespace TMS.Controller;

[ApiController]
[Route("api/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly AppDbContext _context;

    public CommentsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetComments()
    {
        var comments = await _context.Comments
            .Include(c => c.User)
            .Include(c => c.TaskItem)
                .ThenInclude(t => t.Project) // Inclui o relacionamento com Project
            .Include(c => c.TaskItem)
                .ThenInclude(t => t.Assigne) // Inclui o relacionamento com Assigne (User)
            .ToListAsync();

        var commentDtos = comments.Select(c => new CommentsDTO
        {
            Id = c.Id,
            Text = c.Text,
            UserId = c.UserId,
            User = new UserDTO
            {
                UserName = c.User.UserName,
                Email = c.User.Email,
                FullName = c.User.FullName,
                Role = c.User.Role
            },
            TaskItemId = c.TaskItemId,
            TaskItem = new TaskDTO
            {
                Id = c.TaskItem.Id,
                Title = c.TaskItem.Title,
                Description = c.TaskItem.Description,
                DueDate = c.TaskItem.DueDate,
                Priority = c.TaskItem.Priority,
                IsCompleted = c.TaskItem.IsCompleted,
                TicketNumber = c.TaskItem.TicketNumber,
                AssigneId = c.TaskItem.AssigneId,
                Assigne = c.TaskItem.Assigne != null ? new UserDTO
                {
                    UserName = c.TaskItem.Assigne.UserName,
                    Email = c.TaskItem.Assigne.Email,
                    FullName = c.TaskItem.Assigne.FullName,
                    Role = c.TaskItem.Assigne.Role
                } : null,
                ProjectId = c.TaskItem.ProjectId,
                Project = c.TaskItem.Project != null ? new ProjectDTO
                {
                    ProjectName = c.TaskItem.Project.ProjectName,
                    Description = c.TaskItem.Project.Description,
                    StartDate = c.TaskItem.Project.StartDate,
                    EndDate = c.TaskItem.Project.EndDate
                } : null
            }
        }).ToList();

        return Ok(commentDtos);
    }



    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCommentById(int id)
    {
        var comment = await _context.Comments
            .Include(c => c.User) // Incluir o User relacionado
            .Include(c => c.TaskItem) // Incluir o TaskItem relacionado
                .ThenInclude(t => t.Project) // Incluir o Project relacionado ao TaskItem
            .Include(c => c.TaskItem)
                .ThenInclude(t => t.Assigne) // Incluir o Assigne relacionado ao TaskItem
            .FirstOrDefaultAsync(c => c.Id == id); // Buscar pelo ID do comment

        if (comment == null)
        {
            return NotFound(); // Retornar um erro 404 se não encontrar o comment
        }

        var commentDto = new CommentsDTO
        {
            Text = comment.Text,
            UserId = comment.UserId,
            User = new UserDTO
            {
                UserName = comment.User.UserName,
                Email = comment.User.Email,
                FullName = comment.User.FullName,
                Role = comment.User.Role
            },
            TaskItemId = comment.TaskItemId,
            TaskItem = new TaskDTO
            {
                Id = comment.TaskItem.Id,
                Title = comment.TaskItem.Title,
                Description = comment.TaskItem.Description,
                DueDate = comment.TaskItem.DueDate,
                Priority = comment.TaskItem.Priority,
                IsCompleted = comment.TaskItem.IsCompleted,
                TicketNumber = comment.TaskItem.TicketNumber,
                AssigneId = comment.TaskItem.AssigneId,
                Assigne = comment.TaskItem.Assigne != null ? new UserDTO
                {
                    UserName = comment.TaskItem.Assigne.UserName,
                    Email = comment.TaskItem.Assigne.Email,
                    FullName = comment.TaskItem.Assigne.FullName,
                    Role = comment.TaskItem.Assigne.Role
                } : null,
                ProjectId = comment.TaskItem.ProjectId,
                Project = comment.TaskItem.Project != null ? new ProjectDTO
                {
                    ProjectName = comment.TaskItem.Project.ProjectName,
                    Description = comment.TaskItem.Project.Description,
                    StartDate = comment.TaskItem.Project.StartDate,
                    EndDate = comment.TaskItem.Project.EndDate
                } : null
            }
        };

        return Ok(commentDto); // Retorna o comment encontrado
    }


    
    [HttpPost]
    public async Task<IActionResult> CreateComment([FromBody] CommentsDTO commentDTO)
    {
        if (commentDTO == null)
        {
            return BadRequest("Comment data is required.");
        }

        var comment = new Comments
        {
            Text = commentDTO.Text,
            UserId = commentDTO.UserId,
            TaskItemId = commentDTO.TaskItemId
        };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCommentById), new { id = comment.Id }, comment);
    }

    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateComment(int id, [FromBody] CommentsDTO commentDTO)
    {
        if (commentDTO == null)
        {
            return BadRequest("Comment data is required.");
        }

        var comment = await _context.Comments.FindAsync(id);

        if (comment == null)
        {
            return NotFound("Comment not found.");
        }

        // Atualiza os dados do comentário
        comment.Text = commentDTO.Text;
        comment.UserId = commentDTO.UserId;
        comment.TaskItemId = commentDTO.TaskItemId;

        _context.Comments.Update(comment);
        await _context.SaveChangesAsync();

        return NoContent(); // Retorna 204 No Content, indica sucesso na atualização
    }

    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteComment(int id)
    {
        var comment = await _context.Comments.FindAsync(id);

        if (comment == null)
        {
            return NotFound("Comment not found.");
        }

        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync();

        return NoContent(); // Retorna 204 No Content, indica que o comentário foi excluído
    }
}