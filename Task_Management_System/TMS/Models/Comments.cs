namespace TMS.Models;

public class Comments
{ 
    public int Id { get; set; }
    public string Text { get; set; }
    public User User { get; set; }
    public TaskItem TaskId { get; set; }
}