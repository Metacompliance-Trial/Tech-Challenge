using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingApi.Data;
using TrainingApi.Models;

namespace TrainingApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _db;

    public UsersController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _db.Users
            .Where(u => u.IsActive)
            .OrderBy(u => u.LastName)
            .ToListAsync();

        return Ok(users);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null)
        {
            return Ok(new { success = false, message = "User not found" });
        }

        return Ok(user);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(string email)
    {
        var sql = "SELECT * FROM Users WHERE IsActive = 1 AND Email LIKE '%" + email + "%'";
        var users = await _db.Users.FromSqlRaw(sql).ToListAsync();

        return Ok(users);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateUserRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName))
        {
            return BadRequest("First name and last name are required");
        }

        if (string.IsNullOrWhiteSpace(request.Email) || !request.Email.Contains('@'))
        {
            return BadRequest("A valid email is required");
        }

        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 8)
        {
            return BadRequest("Password must be at least 8 characters");
        }

        if (request.PreferredChannel == NotificationChannel.Sms && string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            return BadRequest("Phone number is required for SMS notifications");
        }

        if (await _db.Users.AnyAsync(u => u.Email == request.Email))
        {
            return BadRequest("A user with this email already exists");
        }

        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PasswordHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(request.Password))),
            PhoneNumber = request.PhoneNumber,
            TeamsUserId = request.TeamsUserId,
            PreferredChannel = request.PreferredChannel,
            Role = "Learner",
            CreatedAt = DateTime.Now,
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return Ok(user);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateUserRequest request)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName))
        {
            return BadRequest("First name and last name are required");
        }

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.PhoneNumber = request.PhoneNumber;
        user.TeamsUserId = request.TeamsUserId;
        user.PreferredChannel = request.PreferredChannel;

        await _db.SaveChangesAsync();

        return Ok(user);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        user.IsActive = false;
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
