using api.Models;

public class Subject
{
    public int Id { get; set; }

    public ICollection<SubjectTranslation> Translations { get; set; } = new List<SubjectTranslation>();
    public ICollection<Question> Questions { get; set; } = new List<Question>();
}