public class SubjectTranslation
{
    public int Id { get; set; }

    public int SubjectId { get; set; }
    public Subject Subject { get; set; } = null!;

    public string LanguageCode { get; set; } = string.Empty; // "en", "ru", "ky"
    public string Name { get; set; } = string.Empty;
}