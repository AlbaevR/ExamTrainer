namespace api.Services.Interfaces
{
    public interface IStatisticsService
    {
        Task<string> GenerateExamFeedbackAsync(int attemptId, string languageCode = "ru", bool force = false);
    }
}
