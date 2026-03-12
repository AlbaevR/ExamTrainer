using Microsoft.AspNetCore.Mvc;
using api.Data;
using api.DTOs;

namespace api.Controllers;

[ApiController]
[Route("api/question")]
public class QuestionController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public QuestionController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("random/{subjectId}")]
    public IActionResult GetRandomQuestion(int subjectId)
    {
        var question = _context.Questions
            .Where(q => q.SubjectId == subjectId)
            .OrderBy(q => Guid.NewGuid())
            .FirstOrDefault();

        if (question == null)
            return NotFound("No questions found for this subject.");

        var answers = new List<string>
        {
            question.CorrectAnswer,
            question.IncorrectAnswer1,
            question.IncorrectAnswer2,
            question.IncorrectAnswer3
        }
        .OrderBy(a => Guid.NewGuid())
        .ToList();

        var response = new QuestionResponseDto
        {
            Id = question.Id,
            Text = question.Text,
            Answers = answers
        };

        return Ok(response);
    }
}