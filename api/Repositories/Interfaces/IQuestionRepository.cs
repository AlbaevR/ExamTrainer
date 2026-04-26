using api.Models;

namespace api.Repositories.Interfaces
{
    public interface IQuestionRepository
    {
        Task<IEnumerable<Question>> GetBySubjectIdAsync(int subjectId);
        Task<Question?> GetByIdAsync(int id);
        Task<Question> CreateAsync(Question question);
        Task<Question?> GetRandomBySubjectAsync(int subjectId);
        Task<Question?> UpdateAsync(int id, Question question);
        Task<bool> DeleteAsync(int id);
        Task<(IEnumerable<Question> Items, int TotalCount)> GetPagedBySubjectIdAsync(
            int subjectId, int page, int pageSize);

    }
}
