using System.ComponentModel.DataAnnotations;
using StudentAPI.Models;

namespace StudentAPI.DTOs;

public record CreateClassDto([Required] string Program, [Range(1, 12)] int Semester, [Required] string Section, [Required] string Session);
public record CreateSubjectDto([Required] string Code, [Required] string Name, [Range(1, 6)] int CreditHours, int? TeacherUserId);
public record EnrollStudentsDto([Required] List<int> StudentIds);
public record AttendanceEntryDto([Range(1, int.MaxValue)] int StudentId, AttendanceStatus Status);
public record SaveAttendanceDto(DateOnly LectureDate, [Required] List<AttendanceEntryDto> Entries);
public record CreateExamDto([Required] string Name, [Required] string ExamType, DateOnly ExamDate, [Range(typeof(decimal), "1", "10000")] decimal DefaultTotalMarks);
public record MarkEntryDto([Range(1, int.MaxValue)] int StudentId, [Range(typeof(decimal), "0", "10000")] decimal ObtainedMarks, [Range(typeof(decimal), "1", "10000")] decimal TotalMarks);
public record SaveMarksDto([Range(1, int.MaxValue)] int ExamId, [Required] List<MarkEntryDto> Entries);
