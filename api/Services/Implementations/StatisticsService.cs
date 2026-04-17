using api.Data;
using api.DTOs.Exams;
using api.Models;
using api.Repositories.Interfaces;
using api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;

namespace api.Services.Implementations
{
    public class StatisticsService : IStatisticsService
    {
        private readonly IExamAttemptRepository _examAttemptRepository;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public StatisticsService(
            IExamAttemptRepository examAttemptRepository,
            ApplicationDbContext context,
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory)
        {
            _examAttemptRepository = examAttemptRepository;
            _context = context;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> GenerateExamFeedbackAsync(int attemptId, string languageCode = "ru")
        {
            // Fetch the exam attempt with all necessary details
            var examAttempt = await _examAttemptRepository.GetByIdWithDetailsAsync(attemptId);

            if (examAttempt == null)
            {
                throw new Exception($"Exam attempt with ID {attemptId} not found.");
            }

            // Check if feedback already exists
            if (!string.IsNullOrEmpty(examAttempt.AiFeedback))
            {
                return examAttempt.AiFeedback;
            }

            // Filter incorrect answers
            var incorrectAnswers = examAttempt.UserAnswers.Where(ua => !ua.IsCorrect).ToList();

            if (incorrectAnswers.Count == 0)
            {
                // Perfect score - no mistakes to analyze
                var perfectFeedback = new ExamFeedbackDto
                {
                    Mistakes = new List<MistakeExplanationDto>(),
                    StudyRoadmap = languageCode == "ky"
                        ? "Сиз бардык суроолорго туура жооп бердиңиз! Мыкты натыйжа!"
                        : "Вы ответили на все вопросы правильно! Отличный результат!"
                };
                var camelCaseOptions = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                var feedbackJson = JsonSerializer.Serialize(perfectFeedback, camelCaseOptions);

                examAttempt.AiFeedback = feedbackJson;
                examAttempt.AiFeedbackGeneratedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return feedbackJson;
            }

            // Build the prompt for Gemini
            var prompt = BuildPrompt(incorrectAnswers, languageCode);

            // Call Gemini API
            var apiKey = _configuration["Gemini:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new Exception("Gemini API key is not configured.");
            }

            var geminiResponse = await CallGeminiApiAsync(prompt, apiKey);

            // Store the feedback
            examAttempt.AiFeedback = geminiResponse;
            examAttempt.AiFeedbackGeneratedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return geminiResponse;
        }

        private string BuildPrompt(List<UserAnswer> incorrectAnswers, string languageCode)
        {
            var sb = new StringBuilder();

            if (languageCode == "ky")
            {
                sb.AppendLine("Студент имтихан тапшырды жана кээ бир каталарды кетирди.");
                sb.AppendLine("Ар бир ката үчүн 1-3 сүйлөм чамасында кыска түшүндүрмө бер.");
                sb.AppendLine("Окуу жол картасы үчүн 3-5 пункт жаз.");
                sb.AppendLine();
                sb.AppendLine("Каталар:");
            }
            else // Russian
            {
                sb.AppendLine("Студент сдал экзамен и допустил некоторые ошибки.");
                sb.AppendLine("Для каждой ошибки дай краткое объяснение в 1-3 предложения.");
                sb.AppendLine("Для учебной дорожной карты напиши 3-5 пунктов.");
                sb.AppendLine();
                sb.AppendLine("Ошибки:");
            }

            for (int i = 0; i < incorrectAnswers.Count; i++)
            {
                var userAnswer = incorrectAnswers[i];
                var question = userAnswer.Question;

                // Get question text in the specified language
                var questionTranslation = question.Translations
                    .FirstOrDefault(t => t.LanguageCode == languageCode);
                var questionText = questionTranslation?.Text ?? question.Translations.FirstOrDefault()?.Text ?? "N/A";

                // Get selected (wrong) answer text
                var selectedAnswerTranslation = userAnswer.SelectedAnswerOption.Translations
                    .FirstOrDefault(t => t.LanguageCode == languageCode);
                var selectedAnswerText = selectedAnswerTranslation?.Text ??
                    userAnswer.SelectedAnswerOption.Translations.FirstOrDefault()?.Text ?? "N/A";

                // Get correct answer text
                var correctOption = question.AnswerOptions.FirstOrDefault(ao => ao.IsCorrect);
                var correctAnswerTranslation = correctOption?.Translations
                    .FirstOrDefault(t => t.LanguageCode == languageCode);
                var correctAnswerText = correctAnswerTranslation?.Text ??
                    correctOption?.Translations.FirstOrDefault()?.Text ?? "N/A";

                sb.AppendLine();
                sb.AppendLine($"Ошибка #{i + 1}:");
                sb.AppendLine($"ID вопроса: {question.Id}");
                sb.AppendLine($"Вопрос: {questionText}");
                sb.AppendLine($"Выбранный (неправильный) ответ: {selectedAnswerText}");
                sb.AppendLine($"Правильный ответ: {correctAnswerText}");
            }

            sb.AppendLine();
            if (languageCode == "ky")
            {
                sb.AppendLine("JSON форматында жооп бериңиз. studyRoadmap маани жай строк болуу керек, тизме эмес жок.");
            }
            else
            {
                sb.AppendLine("Отвечайте в формате JSON. studyRoadmap должна быть одной строкой, не массивом.");
            }

            sb.AppendLine(@"{
  ""mistakes"": [
    {
      ""questionId"": <число>,
      ""explanation"": ""<краткое объяснение в 1-3 предложения>""
    }
  ],
  ""studyRoadmap"": ""1. ... 2. ... 3. ...""
}");

            return sb.ToString();
        }

        private async Task<string> CallGeminiApiAsync(string prompt, string apiKey)
        {
            var httpClient = _httpClientFactory.CreateClient();

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                },
                generationConfig = new
                {
                    temperature = 0.7,
                    maxOutputTokens = 8192
                }
            };

            var requestJson = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            // Gemini API URL with API key as query parameter
            var url = $"https://generativelanguage.googleapis.com/v1/models/gemini-2.5-flash:generateContent?key={apiKey}";

            var response = await httpClient.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Gemini API request failed: {response.StatusCode} - {errorContent}");
            }

            var responseJson = await response.Content.ReadAsStringAsync();

            // Log the raw response for debugging
            Console.WriteLine($"Gemini API Response: {responseJson}");

            var geminiResponse = JsonSerializer.Deserialize<GeminiApiResponse>(responseJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (geminiResponse?.Candidates == null || geminiResponse.Candidates.Length == 0)
            {
                throw new Exception($"Gemini API returned empty response. Raw response: {responseJson}");
            }

            if (geminiResponse.Candidates[0]?.Content?.Parts == null || geminiResponse.Candidates[0].Content.Parts.Length == 0)
            {
                throw new Exception($"Gemini response has no content parts. Raw response: {responseJson}");
            }

            // Extract the text content from Gemini's response
            var textContent = geminiResponse.Candidates[0].Content.Parts[0].Text;

            // Remove markdown code block markers if present (```json ... ```)
            textContent = textContent.Trim();
            if (textContent.StartsWith("```"))
            {
                // Remove the first line (```json or ```)
                var firstNewline = textContent.IndexOf('\n');
                if (firstNewline > 0)
                {
                    textContent = textContent.Substring(firstNewline + 1);
                }

                // Remove the last line (```)
                var lastBackticks = textContent.LastIndexOf("```");
                if (lastBackticks > 0)
                {
                    textContent = textContent.Substring(0, lastBackticks);
                }
            }

            textContent = textContent.Trim();

            // Normalize studyRoadmap from array → string if Gemini returned a list
            textContent = PreprocessGeminiJson(textContent);

            // Validate that it's valid JSON and normalize to camelCase for the frontend
            try
            {
                var deserializeOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var feedback = JsonSerializer.Deserialize<ExamFeedbackDto>(textContent, deserializeOptions);
                if (feedback == null)
                {
                    throw new Exception("Failed to deserialize feedback.");
                }

                // Re-serialize with camelCase so keys match the frontend TypeScript types
                var serializeOptions = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                return JsonSerializer.Serialize(feedback, serializeOptions);
            }
            catch (JsonException ex)
            {
                throw new Exception($"Gemini API returned invalid JSON response. Error: {ex.Message}. Content: {textContent}");
            }
        }

        // If Gemini returned studyRoadmap as a JSON array instead of a string,
        // join the items into a single numbered string so deserialization succeeds.
        private static string PreprocessGeminiJson(string json)
        {
            try
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                if (!root.TryGetProperty("studyRoadmap", out var roadmapEl)
                    || roadmapEl.ValueKind != JsonValueKind.Array)
                {
                    return json; // already a string — nothing to fix
                }

                // Join array items into a numbered string
                var lines = roadmapEl.EnumerateArray()
                    .Select((el, i) => $"{i + 1}. {el.GetString() ?? string.Empty}")
                    .ToArray();
                var roadmapStr = string.Join(" ", lines);

                // Rebuild JSON preserving mistakes as-is
                var mistakesRaw = root.GetProperty("mistakes").GetRawText();
                var escapedRoadmap = JsonSerializer.Serialize(roadmapStr);
                return $"{{\"mistakes\":{mistakesRaw},\"studyRoadmap\":{escapedRoadmap}}}";
            }
            catch
            {
                return json; // return original if anything goes wrong
            }
        }

        // Helper classes for Gemini API
        private class GeminiApiResponse
        {
            public Candidate[] Candidates { get; set; } = Array.Empty<Candidate>();
        }

        private class Candidate
        {
            public Content Content { get; set; } = new Content();
        }

        private class Content
        {
            public Part[] Parts { get; set; } = Array.Empty<Part>();
        }

        private class Part
        {
            public string Text { get; set; } = string.Empty;
        }
    }
}
