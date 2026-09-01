namespace StudentAPI.Models;

public class AttendanceRecord
{
    public int Id { get; set; }
    public int SubjectId { get; set; }
    public Subject Subject { get; set; } = null!;
    public int StudentId { get; set; }
    public Student Student { get; set; } = null!;
    public DateOnly LectureDate { get; set; }
    public AttendanceStatus Status { get; set; } = AttendanceStatus.NotMarked;
    // Kept for backward compatibility with existing installations.
    public bool IsPresent { get; set; }
    public string MarkedBy { get; set; } = string.Empty;
    public DateTime MarkedAt { get; set; } = DateTime.UtcNow;
}
