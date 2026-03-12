namespace api.DTOs;

public class QuestionResponseDto
{
	public int Id { get; set; }
	public string Text { get; set; } = null!;
	public List<string> Answers { get; set; } = new();
}