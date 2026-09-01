using System.ComponentModel.DataAnnotations;

namespace StudentAPI.Models;

public class Subject
{
    public int Id { get; set; }
    public int AcademicClassId { get; set; }
    public AcademicClass AcademicClass { get; set; } = null!;
    [Required, StringLength(20)] public string Code { get; set; } = string.Empty;
    [Required, StringLength(150)] public string Name { get; set; } = string.Empty;
    [Range(1, 6)] public int CreditHours { get; set; }
    [StringLength(100)] public string TeacherName { get; set; } = string.Empty;
    public int? TeacherUserId { get; set; }
    public User? TeacherUser { get; set; }
    public ICollection<AttendanceRecord> AttendanceRecords { get; set; } = new List<AttendanceRecord>();
    public ICollection<MarkRecord> MarkRecords { get; set; } = new List<MarkRecord>();
}
