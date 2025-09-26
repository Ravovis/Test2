using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models;

public enum TaskStatus
{
    ToDo,
    InProgress,
    Done
}

public enum TaskPriority
{
    Low,
    Medium,
    High
}

public sealed class TaskItem
{
    [Required]
    public string Id { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Required]
    public TaskStatus Status { get; set; } = TaskStatus.ToDo;

    [Required]
    public TaskPriority Priority { get; set; }

    [Required]
    public string AssigneeId { get; set; } = string.Empty;

    [Required]
    public string AssigneeName { get; set; } = string.Empty;

    public DateTime? DueDate { get; set; }

    [Required]
    public DateTime CreatedDate { get; set; }

    [Required]
    public DateTime UpdatedDate { get; set; }

    [Required]
    public string CreatedBy { get; set; } = string.Empty;
}

public sealed class TaskHistoryEntry
{
    public string Id { get; set; } = string.Empty;
    public string TaskId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty; // created | updated | completed
    public string? Field { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public DateTime Timestamp { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
}

public sealed class Activity
{
    public string Id { get; set; } = string.Empty;
    public string TaskId { get; set; } = string.Empty;
    public string TaskTitle { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty; // created | updated | completed
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}

public sealed class CreateTaskRequest
{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Required]
    public TaskPriority Priority { get; set; }

    [Required]
    public string AssigneeId { get; set; } = string.Empty;

    public DateTime? DueDate { get; set; }
}

public sealed class UpdateTaskRequest
{
    [StringLength(100, MinimumLength = 1)]
    public string? Title { get; set; }
    [StringLength(500)]
    public string? Description { get; set; }
    public TaskStatus? Status { get; set; }
    public TaskPriority? Priority { get; set; }
    public string? AssigneeId { get; set; }
    public DateTime? DueDate { get; set; }
}

public sealed class TaskStatsDto
{
    public int Total { get; set; }
    public int Completed { get; set; }
    public int InProgress { get; set; }
}

