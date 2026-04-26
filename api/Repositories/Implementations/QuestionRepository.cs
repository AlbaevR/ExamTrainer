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

        public async Task<Question?> UpdateAsync(int id, Question question)
        {
            var existingQuestion = await _context.Questions
                .Include(q => q.Translations)
                .Include(q => q.AnswerOptions).ThenInclude(a => a.Translations)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (existingQuestion == null)
                return null;

            // Update translations
            existingQuestion.Translations.Clear();
            foreach (var translation in question.Translations)
            {
                existingQuestion.Translations.Add(translation);
            }

            // Update answer options
            existingQuestion.AnswerOptions.Clear();
            foreach (var answerOption in question.AnswerOptions)
            {
                existingQuestion.AnswerOptions.Add(answerOption);
            }

            _context.Questions.Update(existingQuestion);
            await _context.SaveChangesAsync();
            return existingQuestion;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var question = await _context.Questions.FirstOrDefaultAsync(q => q.Id == id);
            if (question == null)
                return false;

            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();
            return true;
        }
        
        public async Task<(IEnumerable<Question>, int)> GetPagedBySubjectIdAsync(
            int subjectId, int page, int pageSize)
        {
            var query = _context.Questions.Where(q => q.SubjectId == subjectId);
            var total = await query.CountAsync();
            var items = await query
                .OrderBy(q => q.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(q => q.Translations)
                .Include(q => q.AnswerOptions).ThenInclude(a => a.Translations)
                .ToListAsync();
            return (items, total);
        }

    }
}
