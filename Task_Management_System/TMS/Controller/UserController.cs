using Microsoft.AspNetCore.Mvc;
using TMS.Models;

namespace TMS.Controller;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    
    [HttpGet]
    public String GetAllTasks()
    {
        return "Hello World!";
    }
}