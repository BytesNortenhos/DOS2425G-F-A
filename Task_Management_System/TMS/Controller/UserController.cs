using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using TMS.Models;

namespace TMS.Controller;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase {

    private readonly AppDbContext _context;

    public UserController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<User>>> GetUsers()
    {
        var users = await _context.Users.ToListAsync();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound("Error: User not found!");

        return Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] UserDTO userDTO)
    {
        if (userDTO == null)
        {
            return BadRequest("User data is required.");
        }

        // verifica se o user já existe pelo email ou username
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == userDTO.Email || u.UserName == userDTO.UserName);

        if (existingUser != null)
        {
            return Conflict("A user with this email or username already exists.");
        }

        // Cria o user
        var user = new User
        {
            UserName = userDTO.UserName,
            Email = userDTO.Email,
            FullName = userDTO.FullName,
            Role = userDTO.Role
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UserDTO userDTO)
    {
        if (userDTO == null)
        {
            return BadRequest("User data is required.");
        }

        // Verifica se o user existe
        var user = await _context.Users.FindAsync(id);

        if (user == null)
        {
            return NotFound("User not found.");
        }

        // Verifica se o username ou email já estão a ser utilizados 
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => (u.Email == userDTO.Email || u.UserName == userDTO.UserName) && u.Id != id);

        if (existingUser != null)
        {
            return Conflict("A user with this email or username already exists.");
        }

        // Atualiza os campos
        user.UserName = userDTO.UserName;
        user.Email = userDTO.Email;
        user.FullName = userDTO.FullName;
        user.Role = userDTO.Role;

        // Guarda as alterações na base de dados
        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        return NoContent(); // Retorna 204 No Content, indica sucesso na atualização
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        // Verificar se o user existe
        var user = await _context.Users.FindAsync(id);

        if (user == null)
        {
            return NotFound("User not found.");
        }

        // Remove o user da base de dados
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        // Retornar status 204 (No Content), desta forma indica que a exclusão foi bem-sucedida
        return NoContent();
    }
}