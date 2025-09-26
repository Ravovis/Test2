using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _tasks;

    public TasksController(ITaskService tasks)
    {
        _tasks = tasks;
    }

    [HttpGet]
    public ActionResult<IEnumerable<TaskItem>> GetAll()
        => Ok(_tasks.GetAll());

    [HttpGet("stats")]
    public ActionResult<TaskStatsDto> GetStats()
        => Ok(_tasks.GetStats());

    [HttpGet("activities")]
    public ActionResult<IEnumerable<Activity>> GetRecent([FromQuery] int limit = 5)
        => Ok(_tasks.GetRecentActivities(limit));

    [HttpGet("{id}")]
    public ActionResult<TaskItem> GetById(string id)
    {
        var task = _tasks.GetById(id);
        return task is null ? NotFound() : Ok(task);
    }

    [HttpPost]
    public ActionResult<TaskItem> Create([FromBody] CreateTaskRequest request, [FromQuery] string userId = "4")
    {
        try
        {
            var item = _tasks.Create(request, userId);
            return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public ActionResult<TaskItem> Update(string id, [FromBody] UpdateTaskRequest request, [FromQuery] string userId = "4")
    {
        try
        {
            var item = _tasks.Update(id, request, userId);
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
        var ok = _tasks.Delete(id);
        return ok ? NoContent() : NotFound();
    }
}

