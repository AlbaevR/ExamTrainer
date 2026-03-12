using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api.Data;
using api.DTOs.Subjects;
using api.Mappers;

namespace api.Controllers
{
    [ApiController]
    [Route("api/subjects")]
    public class SubjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SubjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetSubjects()
        {
            var subjects = _context.Subjects
                .Include(s => s.Translations)
                .Select(s => s.ToSubjectDto())
                .ToList();

            return Ok(subjects);
        }

        [HttpPost]
        public IActionResult CreateSubject([FromBody] CreateSubjectDto createSubjectDto)
        {
            if (createSubjectDto == null || createSubjectDto.Translations == null || createSubjectDto.Translations.Count == 0)
                return BadRequest("Subject must have at least one translation.");

            var subjectModel = createSubjectDto.ToSubjectFromCreateDto();

            _context.Subjects.Add(subjectModel);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetSubjects), new { id = subjectModel.Id }, subjectModel.ToSubjectDto());
        }

        [HttpGet("{subjectId}")]
        public IActionResult GetSubject(int subjectId, [FromQuery] string? lang)
        {
            var subject = _context.Subjects
                .Include(s => s.Translations)
                .FirstOrDefault(s => s.Id == subjectId);

            if (subject == null)
                return NotFound();

            if (string.IsNullOrWhiteSpace(lang))
                return Ok(subject.ToSubjectDto());

            return Ok(subject.ToLocalizedSubjectDto(lang));
        }
    }
}