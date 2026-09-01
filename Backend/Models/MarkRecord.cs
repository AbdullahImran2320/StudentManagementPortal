namespace StudentAPI.Models;

public class MarkRecord
{
    public int Id { get; set; }
    public int SubjectId { get; set; }
    public Subject Subject { get; set; } = null!;
    public int StudentId { get; set; }
    public Student Student { get; set; } = null!;
    public int? ExamId { get; set; }
    public Exam? Exam { get; set; }
    public decimal ObtainedMarks { get; set; }
    public decimal TotalMarks { get; set; } = 100;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
