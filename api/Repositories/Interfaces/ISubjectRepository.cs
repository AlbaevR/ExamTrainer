using api.Models;

namespace api.Repositories.Interfaces
{
    public interface ISubjectRepository
    {
        Task<IEnumerable<Subject>> GetAllAsync();
        Task<Subject?> GetByIdAsync(int id);
        Task<Subject> CreateAsync(Subject subject);
        Task<Subject?> UpdateAsync(int id, Subject subject);
        Task<bool> DeleteAsync(int id);
    }
}
