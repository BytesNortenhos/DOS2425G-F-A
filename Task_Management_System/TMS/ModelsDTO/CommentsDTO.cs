
namespace TMS.Models;

public class CommentsDTO
{
    public int Id { get; set; }
    public string Text { get; set; }
    public int UserId { get; set; }
    public int TaskItemId { get; set; }

    public UserDTO User { get; set; }
    public TaskDTO TaskItem { get; set; }
}