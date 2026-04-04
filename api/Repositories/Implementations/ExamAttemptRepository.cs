using api.Data;
using api.Models;
using api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories.Implementations
{
    public class ExamAttemptRepository : IExamAttemptRepository
    {
        private readonly ApplicationDbContext _context;

        public ExamAttemptRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ExamAttempt> CreateAsync(ExamAttempt attempt)
        {
            _context.ExamAttempts.Add(attempt);
            await _context.SaveChangesAsync();
            return attempt;
        }

        public async Task<List<ExamAttempt>> GetByUserIdAsync(string userId)
        {
            return await _context.ExamAttempts
                .Include(ea => ea.Subject)
                    .ThenInclude(s => s.Translations)
                .Where(ea => ea.UserId == userId)
                .OrderByDescending(ea => ea.TakenAt)
                .ToListAsync();
        }

        public async Task<ExamAttempt?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.ExamAttempts
                .Include(ea => ea.Subject)
                    .ThenInclude(s => s.Translations)
                .Include(ea => ea.UserAnswers)
                    .ThenInclude(ua => ua.Question)
                        .ThenInclude(q => q.Translations)
                .Include(ea => ea.UserAnswers)
                    .ThenInclude(ua => ua.Question)
                        .ThenInclude(q => q.AnswerOptions)
                            .ThenInclude(ao => ao.Translations)
                .Include(ea => ea.UserAnswers)
                    .ThenInclude(ua => ua.SelectedAnswerOption)
                        .ThenInclude(ao => ao.Translations)
                .FirstOrDefaultAsync(ea => ea.Id == id);
        }
    }
}
