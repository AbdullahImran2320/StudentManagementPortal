namespace StudentAPI.Models;
public class ClassEnrollment { public int Id { get; set; } public int AcademicClassId { get; set; } public AcademicClass AcademicClass { get; set; } = null!; public int StudentId { get; set; } public Student Student { get; set; } = null!; public DateTime EnrolledAt { get; set; } = DateTime.UtcNow; }
