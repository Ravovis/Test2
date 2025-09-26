using WebApplication1.Models;

namespace WebApplication1.Services;

public interface ISeedData
{
    void EnsureSeeded();
}

public sealed class SeedData : ISeedData
{
    private readonly IInMemoryStore _store;

    public SeedData(IInMemoryStore store)
    {
        _store = store;
    }

    public void EnsureSeeded()
    {
        if (_store.Users.Count > 0 || _store.Tasks.Count > 0) return;

        _store.Users.AddRange(new[]
        {
            new UserItem { Id = "1", Name = "John Developer", Email = "john@company.com", Role = "Developer" },
            new UserItem { Id = "2", Name = "Sarah Designer", Email = "sarah@company.com", Role = "Designer" },
            new UserItem { Id = "3", Name = "Mike QA", Email = "mike@company.com", Role = "QA" },
            new UserItem { Id = "4", Name = "Lisa Manager", Email = "lisa@company.com", Role = "Manager" },
        });

        var tasks = new List<TaskItem>
        {
            new TaskItem { Id = "1", Title = "Setup project structure", Description = "Initialize the project with proper folder structure and configuration", Status = global::WebApplication1.Models.TaskStatus.ToDo, Priority = TaskPriority.High, AssigneeId = "1", AssigneeName = "John Developer", CreatedDate = new DateTime(2024,1,15), UpdatedDate = new DateTime(2024,1,15), CreatedBy = "4" },
            new TaskItem { Id = "2", Title = "Design login page mockup", Description = "Create wireframes and mockups for the login page", Status = global::WebApplication1.Models.TaskStatus.InProgress, Priority = TaskPriority.Medium, AssigneeId = "2", AssigneeName = "Sarah Designer", CreatedDate = new DateTime(2024,1,16), UpdatedDate = new DateTime(2024,1,18), CreatedBy = "4" },
            new TaskItem { Id = "3", Title = "Write user authentication tests", Description = "Create comprehensive test suite for user authentication", Status = global::WebApplication1.Models.TaskStatus.ToDo, Priority = TaskPriority.High, AssigneeId = "3", AssigneeName = "Mike QA", CreatedDate = new DateTime(2024,1,17), UpdatedDate = new DateTime(2024,1,17), CreatedBy = "1" },
            new TaskItem { Id = "4", Title = "Review sprint planning", Description = "Review and approve the upcoming sprint planning", Status = global::WebApplication1.Models.TaskStatus.Done, Priority = TaskPriority.Low, AssigneeId = "4", AssigneeName = "Lisa Manager", CreatedDate = new DateTime(2024,1,10), UpdatedDate = new DateTime(2024,1,12), CreatedBy = "4" },
            new TaskItem { Id = "5", Title = "Implement user registration API", Description = "Create REST API endpoints for user registration", Status = global::WebApplication1.Models.TaskStatus.InProgress, Priority = TaskPriority.High, AssigneeId = "1", AssigneeName = "John Developer", CreatedDate = new DateTime(2024,1,18), UpdatedDate = new DateTime(2024,1,19), CreatedBy = "4" },
            new TaskItem { Id = "6", Title = "Create color palette and typography guide", Description = "Establish design system with colors and typography", Status = global::WebApplication1.Models.TaskStatus.Done, Priority = TaskPriority.Medium, AssigneeId = "2", AssigneeName = "Sarah Designer", CreatedDate = new DateTime(2024,1,14), UpdatedDate = new DateTime(2024,1,16), CreatedBy = "4" },
            new TaskItem { Id = "7", Title = "Setup automated testing pipeline", Description = "Configure CI/CD pipeline with automated testing", Status = global::WebApplication1.Models.TaskStatus.ToDo, Priority = TaskPriority.Medium, AssigneeId = "3", AssigneeName = "Mike QA", CreatedDate = new DateTime(2024,1,19), UpdatedDate = new DateTime(2024,1,19), CreatedBy = "1" },
            new TaskItem { Id = "8", Title = "Prepare quarterly review presentation", Description = "Create presentation for quarterly business review", Status = global::WebApplication1.Models.TaskStatus.InProgress, Priority = TaskPriority.Low, AssigneeId = "4", AssigneeName = "Lisa Manager", CreatedDate = new DateTime(2024,1,20), UpdatedDate = new DateTime(2024,1,21), CreatedBy = "4" },
        };

        _store.Tasks.AddRange(tasks);

        var activities = new List<Activity>();
        foreach (var task in tasks)
        {
            activities.Add(new Activity
            {
                Id = $"activity-{task.Id}-created",
                TaskId = task.Id,
                TaskTitle = task.Title,
                Action = "created",
                UserId = task.CreatedBy,
                UserName = ResolveUserName(task.CreatedBy),
                Timestamp = task.CreatedDate
            });

            if (task.UpdatedDate != task.CreatedDate)
            {
                activities.Add(new Activity
                {
                    Id = $"activity-{task.Id}-updated",
                    TaskId = task.Id,
                    TaskTitle = task.Title,
                    Action = "updated",
                    UserId = task.CreatedBy,
                    UserName = ResolveUserName(task.CreatedBy),
                    Timestamp = task.UpdatedDate
                });
            }
        }

        _store.Activities.AddRange(activities
            .OrderByDescending(a => a.Timestamp)
            .ToList());

        UpdateAssignedCounts();
    }

    private string ResolveUserName(string userId)
    {
        return _store.Users.FirstOrDefault(u => u.Id == userId)?.Name ?? "Unknown User";
    }

    private void UpdateAssignedCounts()
    {
        foreach (var user in _store.Users)
        {
            user.AssignedTasksCount = _store.Tasks.Count(t => t.AssigneeId == user.Id);
        }
    }
}

