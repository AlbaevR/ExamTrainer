using Microsoft.AspNetCore.Mvc;
using api.DTOs.Subjects;
using api.Mappers;
using api.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace api.Controllers
{
    [ApiController]
    [Route("api/subjects")]
    public class SubjectController : ControllerBase
    {
        private readonly ISubjectRepository _subjectRepository;

        public SubjectController(ISubjectRepository subjectRepository)
        {
            _subjectRepository = subjectRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetSubjects([FromQuery] string? lang)
        {
            var subjects = await _subjectRepository.GetAllAsync();
            
            if (string.IsNullOrWhiteSpace(lang))
            {
                var dtos = subjects.Select(s => s.ToSubjectDto()).ToList();
                return Ok(dtos);
            }
            else
            {
                var dtos = subjects.Select(s => s.ToLocalizedSubjectDto(lang)).ToList();
                return Ok(dtos);
            }
        }

        [HttpGet("{id}/admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetSubjectAdmin(int id)
        {
            var subject = await _subjectRepository.GetByIdAsync(id);
            if (subject == null)
                return NotFound();
            
            return Ok(subject.ToSubjectWithTranslationsDto());
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateSubject([FromBody] CreateSubjectDto createSubjectDto)
        {
            if (createSubjectDto == null || createSubjectDto.Translations == null || createSubjectDto.Translations.Count == 0)
                return BadRequest("Subject must have at least one translation.");

            var subjectModel = createSubjectDto.ToSubjectFromCreateDto();

            var created = await _subjectRepository.CreateAsync(subjectModel);

            return CreatedAtAction(nameof(GetSubjects), new { id = created.Id }, created.ToSubjectDto());
        }

        [HttpGet("{subjectId}")]
        public async Task<IActionResult> GetSubject(int subjectId, [FromQuery] string? lang)
        {
            var subject = await _subjectRepository.GetByIdAsync(subjectId);

            if (subject == null)
                return NotFound();

            if (string.IsNullOrWhiteSpace(lang))
                return Ok(subject.ToSubjectDto());

            return Ok(subject.ToLocalizedSubjectDto(lang));
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateSubject(int id, [FromBody] UpdateSubjectDto updateSubjectDto)
        {
            if (updateSubjectDto == null || updateSubjectDto.Translations == null || updateSubjectDto.Translations.Count == 0)
                return BadRequest("Subject must have at least one translation.");

            var subjectModel = updateSubjectDto.ToSubjectFromUpdateDto();
            var updated = await _subjectRepository.UpdateAsync(id, subjectModel);

            if (updated == null)
                return NotFound();

            return Ok(updated.ToSubjectDto());
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteSubject(int id)
        {
            var deleted = await _subjectRepository.DeleteAsync(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}