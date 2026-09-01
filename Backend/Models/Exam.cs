using System.ComponentModel.DataAnnotations;

namespace StudentAPI.Models;

public class Exam
{
    public int Id { get; set; }
    public int AcademicClassId { get; set; }
    public AcademicClass AcademicClass { get; set; } = null!;
    [Required, StringLength(100)] public string Name { get; set; } = string.Empty;
    [Required, StringLength(30)] public string ExamType { get; set; } = string.Empty;
    public DateOnly ExamDate { get; set; }
    public decimal DefaultTotalMarks { get; set; } = 100;
    public bool IsPublished { get; set; }
    public ICollection<MarkRecord> MarkRecords { get; set; } = new List<MarkRecord>();
}
