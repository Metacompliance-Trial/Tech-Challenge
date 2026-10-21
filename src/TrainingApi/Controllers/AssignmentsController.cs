using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingApi.Data;
using TrainingApi.Models;
using TrainingApi.Notifications;

namespace TrainingApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AssignmentsController : ControllerBase
{
    private readonly AppDbContext _db = new AppDbContext();

    [HttpGet]
    public IActionResult GetAll(int? userId)
    {
        var query = _db.Assignments
            .Include(a => a.Course)
            .AsQueryable();

        if (userId.HasValue)
        {
            query = query.Where(a => a.UserId == userId.Value);
        }

        return Ok(query.ToList());
    }

    [HttpPost]
    public IActionResult Create(CreateAssignmentRequest request)
    {
        var assignedBy = _db.Users.Find(request.AssignedByUserId);
        if (assignedBy == null || assignedBy.Role != "Admin")
        {
            return BadRequest("Only admins can assign training");
        }

        var user = _db.Users.Find(request.UserId);
        if (user == null || !user.IsActive)
        {
            return BadRequest("User not found");
        }

        var course = _db.Courses.Find(request.CourseId);
        if (course == null)
        {
            return BadRequest("Course not found");
        }

        var alreadyAssigned = _db.Assignments
            .Any(a => a.UserId == user.Id && a.CourseId == course.Id && a.Status != "Completed");
        if (alreadyAssigned)
        {
            return BadRequest("This course is already assigned to the user");
        }

        var assignment = new Assignment
        {
            UserId = user.Id,
            CourseId = course.Id,
            AssignedByUserId = assignedBy.Id,
            AssignedAt = DateTime.Now,
            DueDate = DateTime.Now.AddDays(30),
            Status = "Assigned",
        };

        _db.Assignments.Add(assignment);
        _db.SaveChanges();

        var message = $"You have been assigned '{course.Title}'. Please complete it by {assignment.DueDate:dd MMM yyyy}.";

        try
        {
            switch (user.PreferredChannel)
            {
                case NotificationChannel.Email:
                    var emailSender = new EmailNotificationSender();
                    emailSender.SendAsync(user.Email, "New training assigned", message).Wait();
                    break;
                case NotificationChannel.Sms:
                    var smsSender = new SmsNotificationSender();
                    var sent = smsSender.SendSmsAsync(user.PhoneNumber ?? string.Empty, message).Result;
                    break;
                case NotificationChannel.Teams:
                    var teamsSender = new TeamsNotificationSender();
                    teamsSender.PostMessageAsync(user.TeamsUserId ?? string.Empty, "New training assigned", message).Wait();
                    break;
            }
        }
        catch (Exception)
        {
        }

        return Ok(assignment);
    }

    [HttpPost("{id:int}/complete")]
    public IActionResult Complete(int id)
    {
        var assignment = _db.Assignments.Find(id);
        if (assignment == null)
        {
            return NotFound();
        }

        if (assignment.Status == "Completed")
        {
            return BadRequest("Assignment is already completed");
        }

        assignment.Status = "Completed";
        assignment.CompletedAt = DateTime.Now;
        _db.SaveChanges();

        return Ok(assignment);
    }

    [HttpPost("{id:int}/remind")]
    public IActionResult SendReminder(int id)
    {
        var assignment = _db.Assignments
            .Include(a => a.User)
            .Include(a => a.Course)
            .FirstOrDefault(a => a.Id == id);

        if (assignment == null || assignment.User == null || assignment.Course == null)
        {
            return NotFound();
        }

        if (assignment.Status == "Completed")
        {
            return BadRequest("Assignment is already completed");
        }

        var daysLeft = (assignment.DueDate - DateTime.Now).Days;
        var message = daysLeft < 0
            ? $"'{assignment.Course.Title}' is {-daysLeft} days overdue. Please complete it as soon as possible."
            : $"Reminder: '{assignment.Course.Title}' is due in {daysLeft} days.";

        var user = assignment.User;

        switch (user.PreferredChannel)
        {
            case NotificationChannel.Email:
                new EmailNotificationSender().SendAsync(user.Email, "Training reminder", message).Wait();
                break;
            case NotificationChannel.Sms:
                new SmsNotificationSender().SendSmsAsync(user.PhoneNumber ?? string.Empty, message).Wait();
                break;
            case NotificationChannel.Teams:
                new TeamsNotificationSender().PostMessageAsync(user.TeamsUserId ?? string.Empty, "Training reminder", message).Wait();
                break;
        }

        return Ok(new { success = true, message = "Reminder sent" });
    }

    [HttpGet("overdue")]
    public IActionResult GetOverdue()
    {
        var users = _db.Users.Where(u => u.IsActive).ToList();
        var result = new List<object>();

        foreach (var user in users)
        {
            var assignments = _db.Assignments.Where(a => a.UserId == user.Id).ToList();
            var overdue = assignments
                .Where(a => a.Status != "Completed" && a.DueDate < DateTime.Now)
                .ToList();

            foreach (var assignment in overdue)
            {
                var course = _db.Courses.Find(assignment.CourseId);

                result.Add(new
                {
                    assignmentId = assignment.Id,
                    userName = user.FirstName + " " + user.LastName,
                    email = user.Email,
                    course = course?.Title,
                    dueDate = assignment.DueDate,
                    daysOverdue = (DateTime.Now - assignment.DueDate).Days,
                });
            }
        }

        return Ok(result);
    }
}
