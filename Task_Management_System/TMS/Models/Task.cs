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
    public User Assigne { get; set; }
    public List<Comments> Comments { get; set; }
}