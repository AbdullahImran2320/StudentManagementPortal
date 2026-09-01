using System.ComponentModel.DataAnnotations;

namespace StudentAPI.Models;

public class AcademicClass
{
    public int Id { get; set; }
    [Required, StringLength(100)] public string Program { get; set; } = string.Empty;
    [Range(1, 12)] public int Semester { get; set; }
    [Required, StringLength(20)] public string Section { get; set; } = string.Empty;
    [Required, StringLength(20)] public string Session { get; set; } = string.Empty;
    public ICollection<Subject> Subjects { get; set; } = new List<Subject>();
    public ICollection<ClassEnrollment> Enrollments { get; set; } = new List<ClassEnrollment>();
}
