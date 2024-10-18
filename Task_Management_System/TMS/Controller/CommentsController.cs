using Microsoft.AspNetCore.Mvc;
using TMS.Models;

namespace TMS.Controller;

[ApiController]
[Route("api/[controller]")]
public class CommentsController : ControllerBase
{    
    [HttpGet]
    public String GetAllTasks()
    {
        return "Comentário!";
    }
}