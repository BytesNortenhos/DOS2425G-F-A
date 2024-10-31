using Microsoft.AspNetCore.Mvc;
using TMS.Models;

namespace TMS.Controller;

[ApiController]
[Route("api/[controller]")]
public class CommentsController : ControllerBase
{
    private static List<Comments> comments = new List<Comments>
    {
        new Comments { Id = 1, Text = "Aaa", TaskId = new TaskItem { Id = 1, TicketNumber = "001", Title = "Tarefa 1", Description = "Aaa", IsCompleted = false, DueDate = new DateTime(2023, 10, 5) }},
        new Comments { Id = 2, Text = "Bbb", TaskId = new TaskItem { Id = 2, TicketNumber = "002", Title = "Tarefa 2", Description = "Bbb", IsCompleted = false, DueDate = new DateTime(2023, 10, 6) }},
        new Comments { Id = 3, Text = "Ccc", TaskId = new TaskItem { Id = 3, TicketNumber = "003", Title = "Tarefa 3", Description = "Ccc", IsCompleted = false, DueDate = new DateTime(2023, 10, 7) }},
        new Comments { Id = 4, Text = "Ddd", TaskId = new TaskItem { Id = 4, TicketNumber = "004", Title = "Tarefa 4", Description = "Ddd", IsCompleted = false, DueDate = new DateTime(2023, 10, 8) }},
        new Comments { Id = 5, Text = "Eee", TaskId = new TaskItem { Id = 5, TicketNumber = "005", Title = "Tarefa 5", Description = "Eee", IsCompleted = false, DueDate = new DateTime(2023, 10, 9) }},
    };

    [HttpGet]
    public ActionResult<List<Comments>> GetComments()
    {
        return comments;
    }

    [HttpGet("{id}")]
    public ActionResult<Comments> GetComment(int id)
    {
        var index = comments.FindIndex(u => u.Id == id);
        if (index == -1) return NotFound("Error: Comment not found!");

        return comments[index];
    }

    [HttpPost]
    public ActionResult<Comments> CreateComment([FromBody] Comments comment)
    {
        try
        {
            comments.Add(comment);
        }
        catch
        {
            return null;
        }

        return Created("Comment created!", comments);
    }

    [HttpPut]
    public ActionResult<List<Comments>> UpdateComment([FromBody] Comments comment)
    {
        var index = comments.FindIndex(u => u.Id == comment.Id);
        if (index == -1) return NotFound("Error: Comment not found!");

        comments[index] = comment;
        return comments;
    }

    [HttpDelete("{id}")]
    public ActionResult<List<Comments>> DeleteComment(int id)
    {
        var index = comments.FindIndex(u => u.Id == id);
        if (index == -1) return NotFound("Error: Comment not found!");

        comments.RemoveAt(index);
        return comments;
    }
}