using System.ComponentModel.DataAnnotations;
using WebApplication1.Models;

namespace WebApplication1.Services;

public interface IUserService
{
    IEnumerable<UserItem> GetAll();
    UserItem? GetById(string id);
    UserItem Create(CreateUserRequest request);
    UserItem? Update(string id, UpdateUserRequest request);
    bool Delete(string id);
}

public sealed class UserService : IUserService
{
    private readonly IInMemoryStore _store;

    public UserService(IInMemoryStore store)
    {
        _store = store;
    }

    public IEnumerable<UserItem> GetAll() => _store.Users;

    public UserItem? GetById(string id) => _store.Users.FirstOrDefault(u => u.Id == id);

    public UserItem Create(CreateUserRequest request)
    {
        ValidateCreate(request);

        var item = new UserItem
        {
            Id = GenerateId(),
            Name = request.Name,
            Email = request.Email,
            Role = request.Role,
            AssignedTasksCount = 0
        };
        _store.Users.Add(item);
        return item;
    }

    public UserItem? Update(string id, UpdateUserRequest request)
    {
        var item = _store.Users.FirstOrDefault(u => u.Id == id);
        if (item == null) return null;

        if (request.Name != null)
        {
            if (string.IsNullOrWhiteSpace(request.Name) || request.Name.Length > 50)
                throw new ValidationException("Name must be 1-50 characters");
            item.Name = request.Name;
        }
        if (request.Email != null)
        {
            var addr = new System.Net.Mail.MailAddress(request.Email);
            if (addr.Address != request.Email) throw new ValidationException("Invalid email");
            item.Email = request.Email;
        }
        if (request.Role != null)
        {
            if (!(request.Role == "Developer" || request.Role == "Designer" || request.Role == "QA" || request.Role == "Manager"))
                throw new ValidationException("Invalid role");
            item.Role = request.Role;
        }
        return item;
    }

    public bool Delete(string id)
    {
        if (_store.Tasks.Any(t => t.AssigneeId == id))
        {
            throw new ValidationException("Cannot delete user with assigned tasks");
        }
        var index = _store.Users.FindIndex(u => u.Id == id);
        if (index < 0) return false;
        _store.Users.RemoveAt(index);
        return true;
    }

    private static void ValidateCreate(CreateUserRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || request.Name.Length > 50)
            throw new ValidationException("Name must be 1-50 characters");
        var addr = new System.Net.Mail.MailAddress(request.Email);
        if (addr.Address != request.Email) throw new ValidationException("Invalid email");
        if (!(request.Role == "Developer" || request.Role == "Designer" || request.Role == "QA" || request.Role == "Manager"))
            throw new ValidationException("Invalid role");
    }

    private string GenerateId() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString() + Guid.NewGuid().ToString("N").Substring(0, 6);
}

