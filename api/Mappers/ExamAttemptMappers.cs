using api.Models;
using api.DTOs.Exams;

namespace api.Mappers;

public static class ExamAttemptMappers
{
    public static ExamAttemptHistoryDto ToHistoryDto(this ExamAttempt attempt, string languageCode)
    {
        var subjectTranslation = attempt.Subject.Translations
            .FirstOrDefault(t => t.LanguageCode == languageCode)
            ?? attempt.Subject.Translations.FirstOrDefault();

        return new ExamAttemptHistoryDto
        {
            Id = attempt.Id,
            SubjectName = subjectTranslation?.Name ?? string.Empty,
            SubjectId = attempt.SubjectId,
            Score = attempt.Score,
            TotalQuestions = attempt.TotalQuestions,
            Percentage = attempt.TotalQuestions > 0
                ? Math.Round((double)attempt.Score / attempt.TotalQuestions * 100, 2)
                : 0,
            TakenAt = attempt.TakenAt
        };
    }

    public static ExamAttemptDetailDto ToDetailDto(this ExamAttempt attempt, string languageCode)
    {
        var subjectTranslation = attempt.Subject.Translations
            .FirstOrDefault(t => t.LanguageCode == languageCode)
            ?? attempt.Subject.Translations.FirstOrDefault();

        return new ExamAttemptDetailDto
        {
            Id = attempt.Id,
            SubjectName = subjectTranslation?.Name ?? string.Empty,
            SubjectId = attempt.SubjectId,
            Score = attempt.Score,
            TotalQuestions = attempt.TotalQuestions,
            Percentage = attempt.TotalQuestions > 0
                ? Math.Round((double)attempt.Score / attempt.TotalQuestions * 100, 2)
                : 0,
            TakenAt = attempt.TakenAt,
            Answers = attempt.UserAnswers.Select(ua => ua.ToDetailDto(languageCode)).ToList()
        };
    }

    public static UserAnswerDetailDto ToDetailDto(this UserAnswer userAnswer, string languageCode)
    {
        var questionTranslation = userAnswer.Question.Translations
            .FirstOrDefault(t => t.LanguageCode == languageCode)
            ?? userAnswer.Question.Translations.FirstOrDefault();

        var selectedAnswerTranslation = userAnswer.SelectedAnswerOption.Translations
            .FirstOrDefault(t => t.LanguageCode == languageCode)
            ?? userAnswer.SelectedAnswerOption.Translations.FirstOrDefault();

        return new UserAnswerDetailDto
        {
            QuestionId = userAnswer.QuestionId,
            QuestionText = questionTranslation?.Text ?? string.Empty,
            SelectedAnswerOptionId = userAnswer.SelectedAnswerOptionId,
            SelectedAnswerText = selectedAnswerTranslation?.Text ?? string.Empty,
            SelectedAnswerImageUrl = userAnswer.SelectedAnswerOption.ImageUrl,
            IsCorrect = userAnswer.IsCorrect,
            AllOptions = userAnswer.Question.AnswerOptions.Select(ao =>
            {
                var aoTranslation = ao.Translations
                    .FirstOrDefault(t => t.LanguageCode == languageCode)
                    ?? ao.Translations.FirstOrDefault();

                return new AnswerOptionDetailDto
                {
                    Id = ao.Id,
                    Text = aoTranslation?.Text ?? string.Empty,
                    ImageUrl = ao.ImageUrl,
                    IsCorrect = ao.IsCorrect
                };
            }).ToList()
        };
    }
}
