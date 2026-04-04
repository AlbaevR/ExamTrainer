namespace api.DTOs.Questions;

public class QuestionResponseDto
{
	public int Id { get; set; }
	public string Text { get; set; } = string.Empty;
	public List<AnswerOptionDto> Answers { get; set; } = new();
}

public class AnswerOptionDto
{
	public int Id { get; set; }
	public string Text { get; set; } = string.Empty;
	public bool IsCorrect { get; set; }
	public string? ImageUrl { get; set; }
}