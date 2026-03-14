using api.Models;

namespace api.Repositories.Interfaces
{
    public interface IExamAttemptRepository
    {
        Task<ExamAttempt> CreateAsync(ExamAttempt attempt);
    }
}
