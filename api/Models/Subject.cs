namespace api.Models;

public partial class Subject
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
}