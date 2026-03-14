using api.Models;

namespace api.Repositories.Interfaces
{
    public interface IQuestionRepository
    {
        Task<IEnumerable<Question>> GetBySubjectIdAsync(int subjectId);
        Task<Question?> GetByIdAsync(int id);
        Task<Question> CreateAsync(Question question);
        Task<Question?> GetRandomBySubjectAsync(int subjectId);
    }
}
