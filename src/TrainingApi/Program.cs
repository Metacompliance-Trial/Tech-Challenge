using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using TrainingApi.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    SeedData.Initialise(db);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/health", () => Results.Ok("Healthy"));

app.MapGet("/api/stats", (AppDbContext db) =>
{
    var assignments = db.Assignments.ToList();
    var total = assignments.Count;
    var completed = assignments.Count(a => a.Status == "Completed");
    var overdue = assignments.Count(a => a.Status != "Completed" && a.DueDate < DateTime.Now);

    return Results.Ok(new
    {
        activeUsers = db.Users.Count(u => u.IsActive),
        courses = db.Courses.Count(),
        totalAssignments = total,
        completedAssignments = completed,
        overdueAssignments = overdue,
        completionRate = total == 0 ? 0 : Math.Round((double)completed / total * 100, 1),
    });
});

app.Run();
