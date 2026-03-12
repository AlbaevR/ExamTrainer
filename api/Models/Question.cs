namespace api.Models;

public partial class Question
{
    public int Id { get; set; }

    public int SubjectId { get; set; }

    public string Text { get; set; } = null!;

    public string CorrectAnswer { get; set; } = null!;

    public string IncorrectAnswer1 { get; set; } = null!;

    public string IncorrectAnswer2 { get; set; } = null!;

    public string IncorrectAnswer3 { get; set; } = null!;

    public virtual Subject Subject { get; set; } = null!;
}