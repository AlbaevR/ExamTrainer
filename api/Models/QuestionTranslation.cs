public class QuestionTranslation
{
    public int Id { get; set; }
    public int QuestionId { get; set; }
    public Question Question { get; set; } = null!;

    public string LanguageCode { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}