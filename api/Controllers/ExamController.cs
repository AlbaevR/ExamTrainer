using Microsoft.AspNetCore.Mvc;
using api.DTOs.Exams;
using api.Mappers;
using api.Repositories.Interfaces;
using api.Models;

namespace api.Controllers;

[ApiController]
[Route("api/exam")]
public class ExamController : ControllerBase
{
    private readonly IQuestionRepository _questionRepository;
    private readonly IExamAttemptRepository _examAttemptRepository;
    private readonly ISubjectRepository _subjectRepository;

    public ExamController(
        IQuestionRepository questionRepository,
        IExamAttemptRepository examAttemptRepository,
        ISubjectRepository subjectRepository)
    {
        _questionRepository = questionRepository;
        _examAttemptRepository = examAttemptRepository;
        _subjectRepository = subjectRepository;
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

        var answerOptionIds = request.Answers.Select(a => a.SelectedAnswerOptionId).ToList();

        // Load options by id via question repository by reading question entities (could be optimized)
        var optionLookup = new Dictionary<int, bool>();

        foreach (var ans in request.Answers)
        {
            var question = await _questionRepository.GetByIdAsync(ans.QuestionId);
            if (question == null)
                return BadRequest("Some questions do not belong to the selected subject.");

            var option = question.AnswerOptions.FirstOrDefault(o => o.Id == ans.SelectedAnswerOptionId);
            if (option != null)
                optionLookup[option.Id] = option.IsCorrect;
        }

        int score = request.Answers.Count(a => optionLookup.ContainsKey(a.SelectedAnswerOptionId) && optionLookup[a.SelectedAnswerOptionId]);

        var attempt = new ExamAttempt
        {
            StudentName = request.StudentName,
            SubjectId = request.SubjectId,
            Score = score,
            TakenAt = DateTime.UtcNow
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
}