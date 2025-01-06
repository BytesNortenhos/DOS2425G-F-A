namespace TMS.Models;

public class TaskItem
{
    public int Id { get; set; }
    public string TicketNumber { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime DueDate { get; set; }
    public string Priority { get; set; }
    public int AssigneId { get; set; } // Foreign Key
    public User Assigne { get; set; }   
    public int ProjectId { get; set; } // Foreign Key
    public Project Project { get; set; }
    public List<Comments> Comments { get; set; } = new();
}