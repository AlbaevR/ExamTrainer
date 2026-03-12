using Microsoft.AspNetCore.Mvc;
using api.Data;

namespace api.Controllers;

[ApiController]
[Route("api/subject")]
public class SubjectController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public SubjectController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetSubjects()
    {
        var subjects = _context.Subjects
            .Select(s => new
            {
                s.Id,
                s.Name
            })
            .ToList();

        return Ok(subjects);
    }
}