using Microsoft.AspNetCore.Mvc;
using api.DTOs.Questions;
using api.Mappers;
using api.Repositories.Interfaces;

namespace api.Controllers;

[ApiController]
[Route("api/subjects/{subjectId}/questions")]
public class QuestionController : ControllerBase
{
    private readonly IQuestionRepository _questionRepository;

    public QuestionController(IQuestionRepository questionRepository)
    {
        _questionRepository = questionRepository;
    }

    // GET /api/subjects/{subjectId}/questions
    [HttpGet]
    public async Task<IActionResult> GetQuestions(int subjectId)
    {
        var questions = (await _questionRepository.GetBySubjectIdAsync(subjectId))
            .Select(q => q.ToQuestionDto())
            .ToList();

        return Ok(questions);
    }

    // POST /api/subjects/{subjectId}/questions
    [HttpPost]
    public async Task<IActionResult> CreateQuestion(int subjectId, [FromBody] CreateQuestionDto dto)
    {
        if (dto == null)
            return BadRequest();

        var question = dto.ToQuestionEntity(subjectId);
        var created = await _questionRepository.CreateAsync(question);

        return CreatedAtAction(nameof(GetQuestions), new { subjectId = subjectId }, created.ToQuestionDto());
    }

    // GET /api/subjects/{subjectId}/questions/random?lang=ru
    [HttpGet("random")]
    public async Task<IActionResult> GetRandomQuestion(int subjectId, [FromQuery] string? lang)
    {
        var question = await _questionRepository.GetRandomBySubjectAsync(subjectId);

        if (question == null)
            return NotFound("No questions found for this subject.");

        var dto = question.ToLocalizedQuestionResponseDto(lang ?? string.Empty);

        return Ok(dto);
    }
}