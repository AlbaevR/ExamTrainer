using Microsoft.AspNetCore.Mvc;
using api.Data;
using api.Models;
using api.DTOs.Exam;
using api.DTOs.Question;


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

    [HttpPost("submit")]
    public IActionResult SubmitExam([FromBody] SubmitExamRequestDto request)
    {
        if (request.Answers == null || request.Answers.Count == 0)
            return BadRequest("No answers submitted.");

        var questionIds = request.Answers.Select(a => a.QuestionId).ToList();

        var questions = _context.Questions
            .Where(q => questionIds.Contains(q.Id))
            .ToList();

        if (questions.Any(q => q.SubjectId != request.SubjectId))
            return BadRequest("Some questions do not belong to the selected subject.");

        int score = 0;

        foreach (var submitted in request.Answers)
        {
            var question = questions.FirstOrDefault(q => q.Id == submitted.QuestionId);

            if (question != null && question.CorrectAnswer == submitted.SelectedAnswer)
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

    [HttpGet("start/{subjectId}")]
    public IActionResult StartExam(int subjectId)
    {
        var subject = _context.Subjects.FirstOrDefault(s => s.Id == subjectId);

        if (subject == null)
            return NotFound("Subject not found.");

        var questions = _context.Questions
            .Where(q => q.SubjectId == subjectId)
            .OrderBy(q => Guid.NewGuid())
            .Take(30)
            .ToList();

        //if (questions.Count < 30)
        //    return BadRequest("Not enough questions for this subject.");

        var questionDtos = questions.Select(q =>
        {
            var answers = new List<string>
            {
                q.CorrectAnswer,
                q.IncorrectAnswer1,
                q.IncorrectAnswer2,
                q.IncorrectAnswer3
            }
            .OrderBy(a => Guid.NewGuid())
            .ToList();

            return new QuestionResponseDto
            {
                Id = q.Id,
                Text = q.Text,
                Answers = answers
            };
        }).ToList();

        var result = new ExamStartDto
        {
            SubjectId = subject.Id,
            SubjectName = subject.Name,
            Questions = questionDtos
        };

        return Ok(result);
    }
}