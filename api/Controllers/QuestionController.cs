using Microsoft.AspNetCore.Mvc;
using api.DTOs.Questions;
using api.Mappers;
using api.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;

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
    [HttpGet]
    public async Task<IActionResult> GetQuestions(
        int subjectId,
        [FromQuery] string? lang,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var (questions, total) = await _questionRepository
            .GetPagedBySubjectIdAsync(subjectId, page, pageSize);

        var dtos = string.IsNullOrWhiteSpace(lang)
            ? questions.Select(q => q.ToQuestionDto())
            : questions.Select(q => q.ToLocalizedQuestionResponseDto(lang));

        return Ok(new {
            items = dtos,
            totalCount = total,
            page,
            pageSize,
            totalPages = (int)Math.Ceiling((double)total / pageSize)
        });
    }


    // GET /api/subjects/{subjectId}/questions/{id}/admin
    [HttpGet("{id}/admin")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetQuestionAdmin(int subjectId, int id)
    {
        var question = await _questionRepository.GetByIdAsync(id);
        if (question == null)
            return NotFound();

        if (question.SubjectId != subjectId)
            return BadRequest("Question does not belong to this subject.");
            
        return Ok(question.ToQuestionWithTranslationsDto());
    }

    // POST /api/subjects/{subjectId}/questions
    [HttpPost]
    public async Task<IActionResult> CreateQuestion(int subjectId, [FromBody] CreateQuestionDto dto)
    {
        if (dto == null)
            return BadRequest("Request body is required.");

        // At least one non-empty question translation required
        if (!dto.Translations.Any(t => !string.IsNullOrWhiteSpace(t.Text)))
            return BadRequest("At least one question translation is required.");

        // At least one non-empty translation required per answer option
        for (int i = 0; i < dto.AnswerOptions.Count; i++)
        {
            var opt = dto.AnswerOptions[i];
            if (!opt.Translations.Any(t => !string.IsNullOrWhiteSpace(t.Text)))
                return BadRequest($"Answer option {i + 1} requires at least one translation.");
        }

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

    // PATCH /api/subjects/{subjectId}/questions/{id}
    [HttpPatch("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateQuestion(int subjectId, int id, [FromBody] UpdateQuestionDto dto)
    {
        if (dto == null)
            return BadRequest();

        var question = await _questionRepository.GetByIdAsync(id);
        if (question == null)
            return NotFound();

        if (question.SubjectId != subjectId)
            return BadRequest("Question does not belong to this subject.");

        var updatedQuestion = dto.ToQuestionEntity(subjectId);
        updatedQuestion = await _questionRepository.UpdateAsync(id, updatedQuestion);

        if (updatedQuestion == null)
            return NotFound();

        return Ok(updatedQuestion.ToQuestionDto());
    }

    // DELETE /api/subjects/{subjectId}/questions/{id}
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteQuestion(int subjectId, int id)
    {
        var question = await _questionRepository.GetByIdAsync(id);
        if (question == null)
            return NotFound();

        if (question.SubjectId != subjectId)
            return BadRequest("Question does not belong to this subject.");

        var deleted = await _questionRepository.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}