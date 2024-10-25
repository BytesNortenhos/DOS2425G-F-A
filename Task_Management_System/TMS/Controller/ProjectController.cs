using Microsoft.AspNetCore.Mvc;
using TMS.Models;

namespace TMS.Controller;

[ApiController]
[Route("api/[controller]")]
public class ProjectController : ControllerBase
{
    [HttpGet]
    public String projectMethod()
    {
        return "Project Features Done";
    }
}