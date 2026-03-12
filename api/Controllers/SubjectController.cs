using Microsoft.AspNetCore.Mvc;
using api.Data;
using api.DTOs.Subject;
using api.Mappers;

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
            .Select(s => s.ToSubjectDto())
            .ToList();

        return Ok(subjects);
    }

    [HttpPost]
    public IActionResult CreateSubject([FromBody] CreateSubjectDto createSubjectDto)
    {
        var subjectModel = createSubjectDto.ToSubjectFromCreateDto();

        _context.Subjects.Add(subjectModel);
        _context.SaveChanges();

        return CreatedAtAction(nameof(GetSubjects), new { id = subjectModel.Id }, subjectModel.ToSubjectDto());
    }
}