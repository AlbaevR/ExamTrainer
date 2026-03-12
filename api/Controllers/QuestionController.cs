using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api.Data;
using api.DTOs.Questions;
using api.Mappers;

namespace api.Controllers;

[ApiController]
[Route("api/subjects/{subjectId}/questions")]
public class QuestionController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public QuestionController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET /api/subjects/{subjectId}/questions
    [HttpGet]
    public IActionResult GetQuestions(int subjectId)
    {
        var questions = _context.Questions
            .Where(q => q.SubjectId == subjectId)
            .Include(q => q.Translations)
            .Include(q => q.AnswerOptions).ThenInclude(a => a.Translations)
            .ToList()
            .Select(q => q.ToQuestionDto())
            .ToList();

        return Ok(questions);
    }

    // POST /api/subjects/{subjectId}/questions
    [HttpPost]
    public IActionResult CreateQuestion(int subjectId, [FromBody] CreateQuestionDto dto)
    {
        if (dto == null)
            return BadRequest();

        var subject = _context.Subjects.FirstOrDefault(s => s.Id == subjectId);
        if (subject == null)
            return NotFound("Subject not found.");

        var question = dto.ToQuestionEntity(subjectId);

        _context.Questions.Add(question);
        _context.SaveChanges();

        return CreatedAtAction(nameof(GetQuestions), new { subjectId = subjectId }, question.ToQuestionDto());
    }

    // GET /api/subjects/{subjectId}/questions/random?lang=ru
    [HttpGet("random")]
    public IActionResult GetRandomQuestion(int subjectId, [FromQuery] string? lang)
    {
        var question = _context.Questions
            .Where(q => q.SubjectId == subjectId)
            .Include(q => q.Translations)
            .Include(q => q.AnswerOptions).ThenInclude(a => a.Translations)
            .OrderBy(q => Guid.NewGuid())
            .FirstOrDefault();

        if (question == null)
            return NotFound("No questions found for this subject.");

        var dto = question.ToLocalizedQuestionResponseDto(lang ?? "");

        return Ok(dto);
    }
}