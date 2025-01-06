using System.Net.Sockets;
using System.Text.Json.Serialization;

namespace TMS.Models;

public class Comments
{
    public int Id { get; set; }
    public string Text { get; set; }
    public int UserId { get; set; } // Foreign Key
    public User User { get; set; }
    public int TaskItemId { get; set; } // Foreign Key
    public TaskItem TaskItem { get; set; }
}