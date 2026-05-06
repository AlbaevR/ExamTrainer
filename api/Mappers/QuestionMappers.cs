using System.Linq;
using api.Models;
using api.DTOs.Questions;
using api.DTOs.Exams;

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
                IsCorrect = ao.IsCorrect,
                ImageUrl = ao.ImageUrl
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

    public static Question ToQuestionEntity(this UpdateQuestionDto dto, int subjectId)
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
                Id = ao.Id,
                IsCorrect = ao.IsCorrect,
                ImageUrl = ao.ImageUrl
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
                return new AnswerOptionDto { Id = a.Id, Text = at?.Text ?? string.Empty, IsCorrect = a.IsCorrect, ImageUrl = a.ImageUrl };
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

    public static QuestionWithTranslationsDto ToQuestionWithTranslationsDto(this Question question)
    {
        return new QuestionWithTranslationsDto
        {
            Id = question.Id,
            Translations = question.Translations.Select(t => new QuestionTranslationDto
            {
                LanguageCode = t.LanguageCode,
                Text = t.Text
            }).ToList(),
            AnswerOptions = question.AnswerOptions.Select(a => new AnswerOptionWithTranslationsDto
            {
                Id = a.Id,
                IsCorrect = a.IsCorrect,
                ImageUrl = a.ImageUrl,
                Translations = a.Translations.Select(at => new QuestionTranslationDto
                {
                    LanguageCode = at.LanguageCode,
                    Text = at.Text
                }).ToList()
            }).ToList()
        };
    }

    /// <summary>
    /// Maps a question to a multilingual DTO containing all translations keyed by language code.
    /// Answer options are shuffled once here so the order stays stable across language switches.
    /// </summary>
    public static MultilingualQuestionDto ToMultilingualQuestionDto(this Question question)
    {
        var answers = question.AnswerOptions
            .OrderBy(_ => Guid.NewGuid())   // shuffle once, order is stable in the DTO
            .Select(a => new MultilingualAnswerOptionDto
            {
                Id = a.Id,
                IsCorrect = a.IsCorrect,
                ImageUrl = a.ImageUrl,
                Translations = a.Translations.ToDictionary(t => t.LanguageCode, t => t.Text)
            })
            .ToList();

        return new MultilingualQuestionDto
        {
            Id = question.Id,
            Translations = question.Translations.ToDictionary(t => t.LanguageCode, t => t.Text),
            Answers = answers
        };
    }
}
