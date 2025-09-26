using WebApplication1.Models;

namespace WebApplication1.Services;

public interface IInMemoryStore
{
    List<UserItem> Users { get; }
    List<TaskItem> Tasks { get; }
    List<Activity> Activities { get; }
    List<TaskHistoryEntry> History { get; }
}

public sealed class InMemoryStore : IInMemoryStore
{
    public List<UserItem> Users { get; } = new();
    public List<TaskItem> Tasks { get; } = new();
    public List<Activity> Activities { get; } = new();
    public List<TaskHistoryEntry> History { get; } = new();
}

