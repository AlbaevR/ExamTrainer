using api.Data;
using api.Models;
using Microsoft.EntityFrameworkCore;
using api.Repositories.Interfaces;

namespace api.Repositories.Implementations
{
    public class SubjectRepository : ISubjectRepository
    {
        private readonly ApplicationDbContext _context;

        public SubjectRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Subject>> GetAllAsync()
        {
            return await _context.Subjects
                .Include(s => s.Translations)
                .ToListAsync();
        }

        public async Task<Subject?> GetByIdAsync(int id)
        {
            return await _context.Subjects
                .Include(s => s.Translations)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Subject> CreateAsync(Subject subject)
        {
            _context.Subjects.Add(subject);
            await _context.SaveChangesAsync();
            return subject;
        }

        public async Task<Subject?> UpdateAsync(int id, Subject subject)
        {
            var existingSubject = await _context.Subjects
                .Include(s => s.Translations)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (existingSubject == null)
                return null;

            // Update translations
            existingSubject.Translations.Clear();
            foreach (var translation in subject.Translations)
            {
                existingSubject.Translations.Add(translation);
            }

            _context.Subjects.Update(existingSubject);
            await _context.SaveChangesAsync();
            return existingSubject;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var subject = await _context.Subjects.FirstOrDefaultAsync(s => s.Id == id);
            if (subject == null)
                return false;

            _context.Subjects.Remove(subject);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
