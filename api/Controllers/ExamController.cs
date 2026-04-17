using Microsoft.AspNetCore.Mvc;
using api.DTOs.Exams;
using api.Mappers;
using api.Repositories.Interfaces;
using api.Models;
using api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Text.Json;

namespace api.Controllers;

[ApiController]
[Route("api/exam")]
public class ExamController : ControllerBase
{
    private readonly IQuestionRepository _questionRepository;
    private readonly IExamAttemptRepository _examAttemptRepository;
    private readonly ISubjectRepository _subjectRepository;
    private readonly IStatisticsService _statisticsService;

    public ExamController(
        IQuestionRepository questionRepository,
        IExamAttemptRepository examAttemptRepository,
        ISubjectRepository subjectRepository,
        IStatisticsService statisticsService)
    {
        _questionRepository = questionRepository;
        _examAttemptRepository = examAttemptRepository;
        _subjectRepository = subjectRepository;
        _statisticsService = statisticsService;
    }

    // GET /api/exam/start/{subjectId}?lang=ru
    [HttpGet("start/{subjectId}")]
    public async Task<IActionResult> StartExam(int subjectId, [FromQuery] string? lang)
    {
        var subject = await _subjectRepository.GetByIdAsync(subjectId);
        if (subject == null)
            return NotFound("Subject not found.");

        var language = lang ?? string.Empty;

        var questions = (await _questionRepository.GetBySubjectIdAsync(subjectId))
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
    public async Task<IActionResult> SubmitExam([FromBody] SubmitExamRequestDto request)
    {
        if (request == null || request.Answers == null || request.Answers.Count == 0)
            return BadRequest("No answers submitted.");

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        // Load options by id via question repository by reading question entities (could be optimized)
        var userAnswers = new List<UserAnswer>();
        int score = 0;

        foreach (var ans in request.Answers)
        {
            var question = await _questionRepository.GetByIdAsync(ans.QuestionId);
            if (question == null)
                return BadRequest("Some questions do not belong to the selected subject.");

            var option = question.AnswerOptions.FirstOrDefault(o => o.Id == ans.SelectedAnswerOptionId);
            if (option != null)
            {
                var isCorrect = option.IsCorrect;
                if (isCorrect)
                    score++;

                userAnswers.Add(new UserAnswer
                {
                    QuestionId = ans.QuestionId,
                    SelectedAnswerOptionId = ans.SelectedAnswerOptionId,
                    IsCorrect = isCorrect
                });
            }
        }

        var attempt = new ExamAttempt
        {
            StudentName = request.StudentName,
            UserId = userId,
            SubjectId = request.SubjectId,
            Score = score,
            TotalQuestions = request.Answers.Count,
            TakenAt = DateTime.UtcNow,
            UserAnswers = userAnswers
        };

        await _examAttemptRepository.CreateAsync(attempt);

        var result = new ExamResultDto
        {
            Score = score,
            TotalQuestions = request.Answers.Count,
            Percentage = Math.Round((double)score / request.Answers.Count * 100, 2)
        };

        return Ok(result);
    }

    // GET /api/exam/history
    [HttpGet("history")]
    [Authorize]
    public async Task<IActionResult> GetExamHistory([FromQuery] string? lang)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var attempts = await _examAttemptRepository.GetByUserIdAsync(userId);

        var language = lang ?? string.Empty;
        var dtos = attempts.Select(a => a.ToHistoryDto(language)).ToList();

        return Ok(dtos);
    }

    // GET /api/exam/attempts/{id}
    [HttpGet("attempts/{id}")]
    [Authorize]
    public async Task<IActionResult> GetExamAttemptDetail(int id, [FromQuery] string? lang)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var attempt = await _examAttemptRepository.GetByIdWithDetailsAsync(id);

        if (attempt == null)
            return NotFound();

        // Ensure user can only view their own attempts
        if (attempt.UserId != userId)
            return Forbid();

        var language = lang ?? string.Empty;
        var dto = attempt.ToDetailDto(language);

        return Ok(dto);
    }

    // GET /api/exam/attempts/{id}/feedback?lang=ru
    [HttpGet("attempts/{id}/feedback")]
    [Authorize]
    public async Task<IActionResult> GetExamFeedback(int id, [FromQuery] string? lang)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var attempt = await _examAttemptRepository.GetByIdWithDetailsAsync(id);

        if (attempt == null)
            return NotFound(new { error = "Exam attempt not found." });

        // Ensure user can only view their own attempts
        if (attempt.UserId != userId)
            return Forbid();

        try
        {
            var language = lang ?? "ru";
            var feedbackJson = await _statisticsService.GenerateExamFeedbackAsync(id, language);

            // Normalize through the DTO to guarantee camelCase keys in the response,
            // regardless of what was stored in the database.
            var deserializeOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var feedbackObject = JsonSerializer.Deserialize<api.DTOs.Exams.ExamFeedbackDto>(feedbackJson, deserializeOptions);

            return Ok(new {
                attemptId = id,
                feedback = feedbackObject,
                generatedAt = attempt.AiFeedbackGeneratedAt
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}