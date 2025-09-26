using System.ComponentModel.DataAnnotations;
using WebApplication1.Models;

namespace WebApplication1.Services;

public interface ITaskService
{
    IEnumerable<TaskItem> GetAll();
    TaskItem? GetById(string id);
    TaskItem Create(CreateTaskRequest request, string createdBy);
    TaskItem? Update(string id, UpdateTaskRequest request, string updatedBy);
    bool Delete(string id);
    TaskStatsDto GetStats();
    IEnumerable<Activity> GetRecentActivities(int limit = 5);
}

public sealed class TaskService : ITaskService
{
    private readonly IInMemoryStore _store;

    public TaskService(IInMemoryStore store)
    {
        _store = store;
    }

    public IEnumerable<TaskItem> GetAll() => _store.Tasks;

    public TaskItem? GetById(string id) => _store.Tasks.FirstOrDefault(t => t.Id == id);

    public TaskItem Create(CreateTaskRequest request, string createdBy)
    {
        ValidateCreate(request);

        var assignee = _store.Users.FirstOrDefault(u => u.Id == request.AssigneeId)
            ?? throw new ValidationException("Assignee must be an existing user");

        if (request.DueDate.HasValue && request.DueDate.Value.Date < DateTime.UtcNow.Date)
        {
            throw new ValidationException("Due date cannot be in the past");
        }

        var now = DateTime.UtcNow;
        var item = new TaskItem
        {
            Id = GenerateId(),
            Title = request.Title,
            Description = request.Description,
            Status = global::WebApplication1.Models.TaskStatus.ToDo,
            Priority = request.Priority,
            AssigneeId = request.AssigneeId,
            AssigneeName = assignee.Name,
            DueDate = request.DueDate,
            CreatedDate = now,
            UpdatedDate = now,
            CreatedBy = createdBy
        };

        _store.Tasks.Add(item);
        _store.Activities.Insert(0, new Activity
        {
            Id = GenerateId(),
            TaskId = item.Id,
            TaskTitle = item.Title,
            Action = "created",
            UserId = createdBy,
            UserName = _store.Users.FirstOrDefault(u => u.Id == createdBy)?.Name ?? "Unknown User",
            Timestamp = now
        });

        UpdateAssignedCounts();
        return item;
    }

    public TaskItem? Update(string id, UpdateTaskRequest request, string updatedBy)
    {
        var item = _store.Tasks.FirstOrDefault(t => t.Id == id);
        if (item == null) return null;

        if (request.Title != null)
        {
            if (string.IsNullOrWhiteSpace(request.Title) || request.Title.Length > 100)
                throw new ValidationException("Title must be 1-100 characters");
            item.Title = request.Title;
        }
        if (request.Description != null)
        {
            if (request.Description.Length > 500)
                throw new ValidationException("Description max 500 characters");
            item.Description = request.Description;
        }
        if (request.Priority.HasValue)
        {
            item.Priority = request.Priority.Value;
        }
        if (request.Status.HasValue)
        {
            item.Status = request.Status.Value;
            if (item.Status == global::WebApplication1.Models.TaskStatus.Done)
            {
                _store.Activities.Insert(0, new Activity
                {
                    Id = GenerateId(),
                    TaskId = item.Id,
                    TaskTitle = item.Title,
                    Action = "completed",
                    UserId = updatedBy,
                    UserName = _store.Users.FirstOrDefault(u => u.Id == updatedBy)?.Name ?? "Unknown User",
                    Timestamp = DateTime.UtcNow
                });
            }
        }
        if (request.AssigneeId != null)
        {
            var assignee = _store.Users.FirstOrDefault(u => u.Id == request.AssigneeId)
                ?? throw new ValidationException("Assignee must be an existing user");
            item.AssigneeId = assignee.Id;
            item.AssigneeName = assignee.Name;
        }
        if (request.DueDate.HasValue)
        {
            if (request.DueDate.Value.Date < DateTime.UtcNow.Date)
                throw new ValidationException("Due date cannot be in the past");
            item.DueDate = request.DueDate.Value;
        }

        item.UpdatedDate = DateTime.UtcNow;

        _store.Activities.Insert(0, new Activity
        {
            Id = GenerateId(),
            TaskId = item.Id,
            TaskTitle = item.Title,
            Action = "updated",
            UserId = updatedBy,
            UserName = _store.Users.FirstOrDefault(u => u.Id == updatedBy)?.Name ?? "Unknown User",
            Timestamp = DateTime.UtcNow
        });

        UpdateAssignedCounts();
        return item;
    }

    public bool Delete(string id)
    {
        var index = _store.Tasks.FindIndex(t => t.Id == id);
        if (index < 0) return false;
        _store.Tasks.RemoveAt(index);
        _store.Activities.RemoveAll(a => a.TaskId == id);
        _store.History.RemoveAll(h => h.TaskId == id);
        UpdateAssignedCounts();
        return true;
    }

    public TaskStatsDto GetStats()
    {
        return new TaskStatsDto
        {
            Total = _store.Tasks.Count,
            Completed = _store.Tasks.Count(t => t.Status == global::WebApplication1.Models.TaskStatus.Done),
            InProgress = _store.Tasks.Count(t => t.Status == global::WebApplication1.Models.TaskStatus.InProgress)
        };
    }

    public IEnumerable<Activity> GetRecentActivities(int limit = 5)
    {
        return _store.Activities
            .OrderByDescending(a => a.Timestamp)
            .Take(limit)
            .ToList();
    }

    private static void ValidateCreate(CreateTaskRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title) || request.Title.Length > 100)
            throw new ValidationException("Title must be 1-100 characters");
        if (request.Description != null && request.Description.Length > 500)
            throw new ValidationException("Description max 500 characters");
    }

    private string GenerateId() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString() + Guid.NewGuid().ToString("N").Substring(0, 6);

    private void UpdateAssignedCounts()
    {
        foreach (var user in _store.Users)
        {
            user.AssignedTasksCount = _store.Tasks.Count(t => t.AssigneeId == user.Id);
        }
    }
}

