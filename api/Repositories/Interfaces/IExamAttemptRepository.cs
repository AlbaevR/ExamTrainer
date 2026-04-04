using api.Models;

namespace api.Repositories.Interfaces
{
    public interface IExamAttemptRepository
    {
        Task<ExamAttempt> CreateAsync(ExamAttempt attempt);
        Task<List<ExamAttempt>> GetByUserIdAsync(string userId);
        Task<ExamAttempt?> GetByIdWithDetailsAsync(int id);
    }
}
