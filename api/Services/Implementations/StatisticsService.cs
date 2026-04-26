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

        public async Task<string> GenerateExamFeedbackAsync(int attemptId, string languageCode = "ru", bool force = false)
        {
            // Fetch the exam attempt with all necessary details
            var examAttempt = await _examAttemptRepository.GetByIdWithDetailsAsync(attemptId);

            if (examAttempt == null)
            {
                throw new Exception($"Exam attempt with ID {attemptId} not found.");
            }

            // Check if feedback already exists (skip if force-refresh requested)
            if (!force && !string.IsNullOrEmpty(examAttempt.AiFeedback))
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

            // Build the prompt for Groq
            var prompt = BuildPrompt(incorrectAnswers, languageCode);

            // Call Groq API
            var apiKey = _configuration["Groq:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new Exception("Groq API key is not configured.");
            }

            var geminiResponse = await CallGroqApiAsync(prompt, apiKey);

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
                sb.AppendLine("\"Академиялык устат сыяктуу иш-аракет кылыңыз. 3-5 логикалык кадамдан турган изилдөө планын жазыңыз." +
                              " Ар бир кадамдан кийин, эгерде студенттин кемчиликтери болсо, терең изилдөө үчүн белгилүү бир темаларды белгилеген" +
                              " \"Эмнеге көңүл буруу керек\" бөлүмүн кошуңуз.\n\nМисал: Эгерде студент Моголстандын негизделген күнүн билбесе," +
                              " анда ал 14-кылымдагы Чагатай улусунун саясий кризисин изилдеши керек.\n\nҮн: Кесипкөй, структураланган. \"");
                sb.AppendLine();
                sb.AppendLine("Каталар:");
            }
            else // Russian
            {
                sb.AppendLine("Студент сдал экзамен и допустил некоторые ошибки.");
                sb.AppendLine("Для каждой ошибки дай краткое объяснение в 1-3 предложения.");
                sb.AppendLine("Действуй как академический наставник. Напиши дорожную карту исследования из 3-5 логических этапов." +
                              " После каждого этапа добавь блок \"На что обратить внимание\", где распиши конкретные темы для глубокого изучения, если у студента есть пробелы." +
                              "\n\nПример: Если студент не знает дату основания Моголистана, ему нужно изучить политический кризис в Чагатайском улусе в XIV веке.\n\n" +
                              "Тон: профессиональный, структурированный.");
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
                sb.AppendLine("Жооп төмөндөгү JSON форматында берилет. explanation жана studyRoadmap талааларында маркдаун колдонсоңуз болот.");
            }
            else
            {
                sb.AppendLine("Ответ верни строго в следующем JSON-формате. В полях explanation и studyRoadmap можно использовать маркдаун.");
            }

            sb.AppendLine(@"{
  ""mistakes"": [
    {
      ""questionId"": <число>,
      ""explanation"": ""<краткое объяснение в 1-3 предложения>""
    }
  ],
  ""studyRoadmap"": ""<дорожная карта>""
}");

            return sb.ToString();
        }

        private async Task<string> CallGroqApiAsync(string prompt, string apiKey)
        {
            var httpClient = _httpClientFactory.CreateClient();

            var requestBody = new
            {
                model = "meta-llama/llama-4-scout-17b-16e-instruct",
                messages = new[]
                {
                    new { role = "user", content = prompt }
                },
                temperature = 0.7,
                max_tokens = 8192,
                response_format = new { type = "json_object" }
            };

            var requestJson = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            var url = "https://api.groq.com/openai/v1/chat/completions";

            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var response = await httpClient.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Groq API request failed: {response.StatusCode} - {errorContent}");
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Groq API Response: {responseJson}");

            var groqResponse = JsonSerializer.Deserialize<GroqApiResponse>(responseJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (groqResponse?.Choices == null || groqResponse.Choices.Length == 0)
                throw new Exception($"Groq API returned empty response. Raw: {responseJson}");

            var textContent = groqResponse.Choices[0].Message.Content.Trim();

            // Strip ```json ... ``` fences if the model adds them anyway
            if (textContent.StartsWith("```"))
            {
                var firstNewline = textContent.IndexOf('\n');
                if (firstNewline > 0) textContent = textContent[(firstNewline + 1)..];
                var lastFence = textContent.LastIndexOf("```");
                if (lastFence > 0) textContent = textContent[..lastFence];
                textContent = textContent.Trim();
            }

            // Normalize studyRoadmap array → string if the model returns a list
            textContent = NormalizeStudyRoadmap(textContent);

            try
            {
                var deserializeOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var feedback = JsonSerializer.Deserialize<ExamFeedbackDto>(textContent, deserializeOptions)
                    ?? throw new Exception("Failed to deserialize feedback.");

                var serializeOptions = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                return JsonSerializer.Serialize(feedback, serializeOptions);
            }
            catch (JsonException ex)
            {
                throw new Exception($"Groq API returned invalid JSON. Error: {ex.Message}. Content: {textContent}");
            }
        }

        private static string NormalizeStudyRoadmap(string json)
        {
            try
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                if (!root.TryGetProperty("studyRoadmap", out var roadmapEl)
                    || roadmapEl.ValueKind != JsonValueKind.Array)
                    return json;

                var sb = new StringBuilder();
                int stageIndex = 0;

                foreach (var el in roadmapEl.EnumerateArray())
                {
                    stageIndex++;

                    if (el.ValueKind == JsonValueKind.String)
                    {
                        // Simple array of strings: "1. Step one"
                        sb.AppendLine($"{stageIndex}. {el.GetString()}");
                    }
                    else if (el.ValueKind == JsonValueKind.Object)
                    {
                        // Structured object: { stage, description, attention[] }
                        var stage = el.TryGetProperty("stage", out var s) ? s.GetString() ?? "" : "";
                        var desc = el.TryGetProperty("description", out var d) ? d.GetString() ?? "" : "";

                        sb.AppendLine($"### {stageIndex}. {stage}");
                        if (!string.IsNullOrWhiteSpace(desc))
                            sb.AppendLine(desc);

                        if (el.TryGetProperty("attention", out var attention)
                            && attention.ValueKind == JsonValueKind.Array)
                        {
                            sb.AppendLine();
                            sb.AppendLine("**На что обратить внимание:**");
                            foreach (var item in attention.EnumerateArray())
                            {
                                var text = item.ValueKind == JsonValueKind.String
                                    ? item.GetString()
                                    : item.ToString();
                                sb.AppendLine($"- {text}");
                            }
                        }

                        sb.AppendLine();
                    }
                }

                var mistakesRaw = root.GetProperty("mistakes").GetRawText();
                var escapedRoadmap = JsonSerializer.Serialize(sb.ToString().Trim());
                return $"{{\"mistakes\":{mistakesRaw},\"studyRoadmap\":{escapedRoadmap}}}";
            }
            catch { return json; }
        }

        // Groq response shape (OpenAI-compatible)
        private class GroqApiResponse
        {
            public GroqChoice[] Choices { get; set; } = Array.Empty<GroqChoice>();
        }

        private class GroqChoice
        {
            public GroqMessage Message { get; set; } = new GroqMessage();
        }

        private class GroqMessage
        {
            public string Content { get; set; } = string.Empty;
        }
    }
}
