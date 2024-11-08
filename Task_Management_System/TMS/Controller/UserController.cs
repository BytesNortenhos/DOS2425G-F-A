using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using TMS.Models;

namespace TMS.Controller;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase {
    private static List<User> users = new List<User> {
        new User { Id = 1, UserName = "user1", Email = "1@x.com", FullName = "User 1", Role = "Admin", tickets = new List<TaskItem> { new TaskItem { Id = 1, TicketNumber = "001", Title = "Task 1", Description = "Aaa", IsCompleted = false, DueDate = DateTime.Now.AddDays(1), Priority = "High", Assigne = new User { Id = 1, UserName = "user1", Email = "1@x.com", FullName = "User 1", Role = "Admin" } }, new TaskItem { Id = 2, TicketNumber = "002", Title = "Task 2", Description = "Aaa", IsCompleted = false, DueDate = DateTime.Now.AddDays(2), Priority = "Low", Assigne = new User { Id = 1, UserName = "user2", Email = "2@x.com", FullName = "User 1", Role = "Dev" } }} },
        new User { Id = 2, UserName = "user2", Email = "2@x.com", FullName = "User 2", Role = "User", tickets = new List<TaskItem> { new TaskItem { Id = 1, TicketNumber = "001", Title = "Task 1", Description = "Aaa", IsCompleted = false, DueDate = DateTime.Now.AddDays(1), Priority = "High", Assigne = new User { Id = 1, UserName = "user1", Email = "1@x.com", FullName = "User 1", Role = "Admin" } }, new TaskItem { Id = 2, TicketNumber = "002", Title = "Task 2", Description = "Aaa", IsCompleted = false, DueDate = DateTime.Now.AddDays(2), Priority = "Low", Assigne = new User { Id = 1, UserName = "user2", Email = "3@x.com", FullName = "User 3", Role = "Dev" } }} },
        new User { Id = 3, UserName = "user3", Email = "3@x.com", FullName = "User 3", Role = "User", tickets = new List<TaskItem> { new TaskItem { Id = 1, TicketNumber = "001", Title = "Task 1", Description = "Aaa", IsCompleted = false, DueDate = DateTime.Now.AddDays(1), Priority = "High", Assigne = new User { Id = 1, UserName = "user1", Email = "1@x.com", FullName = "User 1", Role = "Admin" } }, new TaskItem { Id = 2, TicketNumber = "002", Title = "Task 2", Description = "Aaa", IsCompleted = false, DueDate = DateTime.Now.AddDays(2), Priority = "Low", Assigne = new User { Id = 1, UserName = "user2", Email = "2@x.com", FullName = "User 2", Role = "Dev" } }} },
        new User { Id = 4, UserName = "user4", Email = "4@x.com", FullName = "User 4", Role = "User", tickets = new List<TaskItem> { new TaskItem { Id = 1, TicketNumber = "001", Title = "Task 1", Description = "Aaa", IsCompleted = false, DueDate = DateTime.Now.AddDays(1), Priority = "High", Assigne = new User { Id = 1, UserName = "user1", Email = "1@x.com", FullName = "User 1", Role = "Admin" } }, new TaskItem { Id = 2, TicketNumber = "002", Title = "Task 2", Description = "Aaa", IsCompleted = false, DueDate = DateTime.Now.AddDays(2), Priority = "Low", Assigne = new User { Id = 1, UserName = "user2", Email = "1@x.com", FullName = "User 1", Role = "Dev" } }} },
        new User { Id = 5, UserName = "user5", Email = "5@x.com", FullName = "User 5", Role = "User", tickets = new List<TaskItem> { new TaskItem { Id = 1, TicketNumber = "001", Title = "Task 1", Description = "Aaa", IsCompleted = false, DueDate = DateTime.Now.AddDays(1), Priority = "High", Assigne = new User { Id = 1, UserName = "user1", Email = "1@x.com", FullName = "User 1", Role = "Admin" } }, new TaskItem { Id = 2, TicketNumber = "002", Title = "Task 2", Description = "Aaa", IsCompleted = false, DueDate = DateTime.Now.AddDays(2), Priority = "Low", Assigne = new User { Id = 1, UserName = "user2", Email = "2@x.com", FullName = "User 1", Role = "Dev" } }} },
    };

    [HttpGet]
    public ActionResult<List<User>> GetUsers() {
        return users;
    }

    [HttpGet("{id}")]
    public ActionResult<User> GetUser(int id) {
        var index = users.FindIndex(u => u.Id == id);
        if (index == -1) return NotFound("User not found!");
        
        return users[index];
    }

    [HttpPost]
    public ActionResult<User> CreateUser([FromBody] User user) {
        try {
            users.Add(user);
        } catch {
            return null;
        }

        return Created("User created!", user);
    }

    [HttpPut]
    public ActionResult<List<User>> UpdateUser([FromBody] User user) {
        var index = users.FindIndex(u => u.Id == user.Id);
        if (index == -1) return NotFound("Error: user not found!");
        
        users[index] = user;
        return users;
    }

    [HttpDelete("{id}")]
    public ActionResult<List<User>> DeleteUser(int id) {
        var index = users.FindIndex(u => u.Id == id);
        if (index == -1) return NotFound("User not found!");

        users.RemoveAt(index);
        return users;
    }
}