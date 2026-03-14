using Microsoft.AspNetCore.Mvc;
using api.DTOs.Subjects;
using api.Mappers;
using api.Repositories.Interfaces;

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
        public async Task<IActionResult> GetSubjects()
        {
            var subjects = (await _subjectRepository.GetAllAsync())
                .Select(s => s.ToSubjectDto())
                .ToList();

            return Ok(subjects);
        }

        [HttpPost]
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
    }
}