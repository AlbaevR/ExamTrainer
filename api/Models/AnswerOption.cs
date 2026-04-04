public class AnswerOption
{
    public int Id { get; set; }
    public int QuestionId { get; set; }
    public Question Question { get; set; } = null!;

    public bool IsCorrect { get; set; }
    public string? ImageUrl { get; set; }

    public ICollection<AnswerOptionTranslation> Translations { get; set; } = new List<AnswerOptionTranslation>();
}