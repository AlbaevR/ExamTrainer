using api.Data;
using api.Models;
using api.Repositories.Interfaces;

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
    }
}
