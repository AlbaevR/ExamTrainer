using api.Models;

public class Question
{
    public int Id { get; set; }
    public int SubjectId { get; set; }
    public Subject Subject { get; set; } = null!;

    public ICollection<QuestionTranslation> Translations { get; set; } = new List<QuestionTranslation>();
    public ICollection<AnswerOption> AnswerOptions { get; set; } = new List<AnswerOption>();
}