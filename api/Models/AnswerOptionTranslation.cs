public class AnswerOptionTranslation
{
    public int Id { get; set; }
    public int AnswerOptionId { get; set; }
    public AnswerOption AnswerOption { get; set; } = null!;

    public string LanguageCode { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}