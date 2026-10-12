namespace api.Services.Interfaces
{
    public interface IAiFeedbackLimitService
    {
        int DailyLimit { get; }

        Task<int> CountTodayAsync(string userId);

        Task RecordAsync(string userId);

        // When the current day (Bishkek time) ends, in UTC
        DateTime NextResetUtc();
    }
}
