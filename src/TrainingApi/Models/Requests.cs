namespace TrainingApi.Models;

public record CreateUserRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string? PhoneNumber,
    string? TeamsUserId,
    NotificationChannel PreferredChannel);

public record UpdateUserRequest(
    string FirstName,
    string LastName,
    string? PhoneNumber,
    string? TeamsUserId,
    NotificationChannel PreferredChannel);

public record CreateAssignmentRequest(int UserId, int CourseId, int AssignedByUserId);
