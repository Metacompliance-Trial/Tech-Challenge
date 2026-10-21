namespace TrainingApi.Models;

public class Assignment
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public int CourseId { get; set; }
    public Course? Course { get; set; }
    public int AssignedByUserId { get; set; }
    public DateTime AssignedAt { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string Status { get; set; } = "Assigned";
}
