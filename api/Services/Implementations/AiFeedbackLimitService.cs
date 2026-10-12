using api.Data;
using api.Models;
using api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace api.Services.Implementations
{
    public class AiFeedbackLimitService : IAiFeedbackLimitService
    {
        // Users are in Kyrgyzstan (UTC+6, no DST), so the daily limit resets at Bishkek midnight
        private static readonly TimeSpan DayOffset = TimeSpan.FromHours(6);

        private readonly ApplicationDbContext _context;

        public AiFeedbackLimitService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            DailyLimit = configuration.GetValue("AiFeedback:DailyLimit", 3);
        }

        public int DailyLimit { get; }

        public async Task<int> CountTodayAsync(string userId)
        {
            var startOfDayUtc = StartOfTodayUtc();
            return await _context.AiFeedbackRequests
                .CountAsync(r => r.UserId == userId && r.RequestedAt >= startOfDayUtc);
        }

        public async Task RecordAsync(string userId)
        {
            _context.AiFeedbackRequests.Add(new AiFeedbackRequest
            {
                UserId = userId,
                RequestedAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();
        }

        public DateTime NextResetUtc() => StartOfTodayUtc().AddDays(1);

        private static DateTime StartOfTodayUtc()
        {
            var localDate = (DateTime.UtcNow + DayOffset).Date;
            return DateTime.SpecifyKind(localDate - DayOffset, DateTimeKind.Utc);
        }
    }
}
