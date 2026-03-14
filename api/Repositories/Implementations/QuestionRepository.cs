using api.Data;
using api.Models;
using Microsoft.EntityFrameworkCore;
using api.Repositories.Interfaces;

namespace api.Repositories.Implementations
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly ApplicationDbContext _context;

        public QuestionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Question> CreateAsync(Question question)
        {
            _context.Questions.Add(question);
            await _context.SaveChangesAsync();
            return question;
        }

        public async Task<IEnumerable<Question>> GetBySubjectIdAsync(int subjectId)
        {
            return await _context.Questions
                .Where(q => q.SubjectId == subjectId)
                .Include(q => q.Translations)
                .Include(q => q.AnswerOptions).ThenInclude(a => a.Translations)
                .ToListAsync();
        }

        public async Task<Question?> GetByIdAsync(int id)
        {
            return await _context.Questions
                .Include(q => q.Translations)
                .Include(q => q.AnswerOptions).ThenInclude(a => a.Translations)
                .FirstOrDefaultAsync(q => q.Id == id);
        }

        public async Task<Question?> GetRandomBySubjectAsync(int subjectId)
        {
            return await _context.Questions
                .Where(q => q.SubjectId == subjectId)
                .Include(q => q.Translations)
                .Include(q => q.AnswerOptions).ThenInclude(a => a.Translations)
                .OrderBy(q => Guid.NewGuid())
                .FirstOrDefaultAsync();
        }
    }
}
