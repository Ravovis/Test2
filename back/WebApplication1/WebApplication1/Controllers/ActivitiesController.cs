using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ActivitiesController : ControllerBase
{
    private readonly IInMemoryStore _store;

    public ActivitiesController(IInMemoryStore store)
    {
        _store = store;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Activity>> Get([FromQuery] int limit = 5)
    {
        var items = _store.Activities
            .OrderByDescending(a => a.Timestamp)
            .Take(limit)
            .ToList();
        return Ok(items);
    }
}

