using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api.Data;
using api.DTOs.Exams;
using api.Mappers;
using api.Models;

namespace api.Controllers;

[ApiController]
[Route("api/exam")]
public class ExamController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ExamController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET /api/exam/start/{subjectId}?lang=ru
    [HttpGet("start/{subjectId}")]
    public IActionResult StartExam(int subjectId, [FromQuery] string? lang)
    {
        var subject = _context.Subjects
            .Include(s => s.Translations)
            .FirstOrDefault(s => s.Id == subjectId);

        if (subject == null)
            return NotFound("Subject not found.");

        var language = lang ?? string.Empty;

        var questions = _context.Questions
            .Where(q => q.SubjectId == subjectId)
            .Include(q => q.Translations)
            .Include(q => q.AnswerOptions).ThenInclude(a => a.Translations)
            .OrderBy(q => Guid.NewGuid())
            .Take(30)
            .ToList();

        var questionDtos = questions.Select(q => q.ToLocalizedQuestionResponseDto(language)).ToList();

        var result = new ExamStartDto
        {
            SubjectId = subject.Id,
            SubjectName = subject.Translations.FirstOrDefault(t => t.LanguageCode == language)?.Name
                ?? subject.Translations.FirstOrDefault()?.Name ?? string.Empty,
            Questions = questionDtos
        };

        return Ok(result);
    }

    // POST /api/exam/submit
    [HttpPost("submit")]
    public IActionResult SubmitExam([FromBody] SubmitExamRequestDto request)
    {
        if (request == null || request.Answers == null || request.Answers.Count == 0)
            return BadRequest("No answers submitted.");

        var questionIds = request.Answers.Select(a => a.QuestionId).ToList();

        var answerOptionIds = request.Answers.Select(a => a.SelectedAnswerOptionId).ToList();

        var options = _context.AnswerOptions
            .Where(o => answerOptionIds.Contains(o.Id))
            .ToList();

        int score = 0;

        foreach (var submitted in request.Answers)
        {
            var option = options.FirstOrDefault(o => o.Id == submitted.SelectedAnswerOptionId);
            if (option != null && option.IsCorrect)
            {
                score++;
            }
        }

        var attempt = new ExamAttempt
        {
            StudentName = request.StudentName,
            SubjectId = request.SubjectId,
            Score = score,
            TakenAt = DateTime.UtcNow
        };

        _context.ExamAttempts.Add(attempt);
        _context.SaveChanges();

        var result = new ExamResultDto
        {
            Score = score,
            TotalQuestions = request.Answers.Count,
            Percentage = Math.Round((double)score / request.Answers.Count * 100, 2)
        };

        return Ok(result);
    }
}