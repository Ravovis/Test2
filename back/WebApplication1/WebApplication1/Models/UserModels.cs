using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models;

public sealed class UserItem
{
    [Required]
    public string Id { get; set; } = string.Empty;

    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [RegularExpression("^(Developer|Designer|QA|Manager)$")]
    public string Role { get; set; } = string.Empty;

    public int AssignedTasksCount { get; set; }
}

public sealed class CreateUserRequest
{
    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [RegularExpression("^(Developer|Designer|QA|Manager)$")]
    public string Role { get; set; } = string.Empty;
}

public sealed class UpdateUserRequest
{
    [StringLength(50, MinimumLength = 1)]
    public string? Name { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    [RegularExpression("^(Developer|Designer|QA|Manager)$")]
    public string? Role { get; set; }
}

