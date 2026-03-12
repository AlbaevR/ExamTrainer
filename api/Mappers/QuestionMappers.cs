using System.Linq;
using api.Models;
using api.DTOs.Questions;

namespace api.Mappers;

public static class QuestionMappers
{
    public static CreateQuestionDto ToCreateQuestionDto(this Question q)
    {
        // Not used currently - placeholder
        return new CreateQuestionDto();
    }

    public static Question ToQuestionEntity(this CreateQuestionDto dto, int subjectId)
    {
        var question = new Question
        {
            SubjectId = subjectId
        };

        foreach (var t in dto.Translations)
        {
            question.Translations.Add(new QuestionTranslation
            {
                LanguageCode = t.LanguageCode,
                Text = t.Text
            });
        }

        foreach (var ao in dto.AnswerOptions)
        {
            var option = new AnswerOption
            {
                IsCorrect = ao.IsCorrect
            };

            foreach (var aot in ao.Translations)
            {
                option.Translations.Add(new AnswerOptionTranslation
                {
                    LanguageCode = aot.LanguageCode,
                    Text = aot.Text
                });
            }

            question.AnswerOptions.Add(option);
        }

        return question;
    }

    public static QuestionResponseDto ToLocalizedQuestionResponseDto(this Question question, string languageCode)
    {
        var qTranslation = question.Translations.FirstOrDefault(t => t.LanguageCode == languageCode)
            ?? question.Translations.FirstOrDefault();

        var answers = question.AnswerOptions
            .Select(a =>
            {
                var at = a.Translations.FirstOrDefault(t => t.LanguageCode == languageCode) ?? a.Translations.FirstOrDefault();
                return new AnswerOptionDto { Id = a.Id, Text = at?.Text ?? string.Empty };
            })
            .OrderBy(a => Guid.NewGuid())
            .ToList();

        return new QuestionResponseDto
        {
            Id = question.Id,
            Text = qTranslation?.Text ?? string.Empty,
            Answers = answers
        };
    }

    public static api.DTOs.Questions.QuestionResponseDto ToQuestionDto(this Question question)
    {
        // default: pick first translation
        return question.ToLocalizedQuestionResponseDto(string.Empty);
    }
}
