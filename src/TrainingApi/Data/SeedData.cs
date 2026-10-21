using System.Security.Cryptography;
using System.Text;
using TrainingApi.Models;

namespace TrainingApi.Data;

public static class SeedData
{
    public static void Initialise(AppDbContext db)
    {
        db.Database.EnsureCreated();

        if (db.Users.Any())
        {
            return;
        }

        var now = DateTime.Now;

        var users = new List<User>
        {
            NewUser("Sarah", "Mitchell", "sarah.mitchell@example.com", "Admin", NotificationChannel.Email),
            NewUser("Daniel", "Okafor", "daniel.okafor@example.com", "Learner", NotificationChannel.Teams, teamsUserId: "29:1a2b3c4d"),
            NewUser("Priya", "Sharma", "priya.sharma@example.com", "Learner", NotificationChannel.Sms, phoneNumber: "+447700900123"),
            NewUser("Tom", "Gallagher", "tom.gallagher@example.com", "Learner", NotificationChannel.Email),
            NewUser("Aisling", "Byrne", "aisling.byrne@example.com", "Learner", NotificationChannel.Teams, teamsUserId: "29:5e6f7a8b"),
        };

        var courses = new List<Course>
        {
            new() { Title = "Phishing Awareness", Description = "Spot and report suspicious emails.", DurationMinutes = 20 },
            new() { Title = "GDPR Essentials", Description = "Handling personal data correctly.", DurationMinutes = 30 },
            new() { Title = "Password Security", Description = "Creating and managing strong passwords.", DurationMinutes = 15 },
            new() { Title = "Social Engineering", Description = "Recognising manipulation by phone, email and in person.", DurationMinutes = 25 },
        };

        db.Users.AddRange(users);
        db.Courses.AddRange(courses);
        db.SaveChanges();

        var admin = users[0];

        db.Assignments.AddRange(
            new Assignment { UserId = users[1].Id, CourseId = courses[0].Id, AssignedByUserId = admin.Id, AssignedAt = now.AddDays(-45), DueDate = now.AddDays(-15), Status = "Assigned" },
            new Assignment { UserId = users[1].Id, CourseId = courses[1].Id, AssignedByUserId = admin.Id, AssignedAt = now.AddDays(-10), DueDate = now.AddDays(20), Status = "Assigned" },
            new Assignment { UserId = users[2].Id, CourseId = courses[0].Id, AssignedByUserId = admin.Id, AssignedAt = now.AddDays(-40), DueDate = now.AddDays(-10), CompletedAt = now.AddDays(-12), Status = "Completed" },
            new Assignment { UserId = users[3].Id, CourseId = courses[2].Id, AssignedByUserId = admin.Id, AssignedAt = now.AddDays(-35), DueDate = now.AddDays(-5), Status = "Assigned" },
            new Assignment { UserId = users[4].Id, CourseId = courses[3].Id, AssignedByUserId = admin.Id, AssignedAt = now.AddDays(-5), DueDate = now.AddDays(25), Status = "Assigned" });

        db.SaveChanges();
    }

    private static User NewUser(string firstName, string lastName, string email, string role,
        NotificationChannel channel, string? phoneNumber = null, string? teamsUserId = null)
    {
        return new User
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Role = role,
            PreferredChannel = channel,
            PhoneNumber = phoneNumber,
            TeamsUserId = teamsUserId,
            PasswordHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes("Password123!"))),
            CreatedAt = DateTime.Now,
        };
    }
}
