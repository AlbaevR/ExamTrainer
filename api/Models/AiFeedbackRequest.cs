namespace api.Models;

// One row per AI feedback generation (a Groq call), used for the per-user daily limit.
public class AiFeedbackRequest
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;
    public virtual AppUser User { get; set; } = null!;

    public DateTime RequestedAt { get; set; }
}
