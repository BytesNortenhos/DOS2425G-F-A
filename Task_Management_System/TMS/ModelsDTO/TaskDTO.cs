
namespace TMS.Models;

public class TaskDTO
{
    public int Id { get; set; }
    public string TicketNumber { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime DueDate { get; set; }
    public string Priority { get; set; }
    public int AssigneId { get; set; }
    public UserDTO Assigne { get; set; } // Nested DTO for User
    public int ProjectId { get; set; }
    public ProjectDTO Project { get; set; } // Nested DTO for Project
}