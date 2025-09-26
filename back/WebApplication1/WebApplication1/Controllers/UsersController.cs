using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _users;

    public UsersController(IUserService users)
    {
        _users = users;
    }

    [HttpGet]
    public ActionResult<IEnumerable<UserItem>> GetAll() => Ok(_users.GetAll());

    [HttpGet("{id}")]
    public ActionResult<UserItem> GetById(string id)
    {
        var user = _users.GetById(id);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpPost]
    public ActionResult<UserItem> Create([FromBody] CreateUserRequest request)
    {
        try
        {
            var item = _users.Create(request);
            return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public ActionResult<UserItem> Update(string id, [FromBody] UpdateUserRequest request)
    {
        try
        {
            var item = _users.Update(id, request);
            if (item is null) return NotFound();
            return Ok(item);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(string id)
    {
        try
        {
            var ok = _users.Delete(id);
            return ok ? NoContent() : NotFound();
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}

