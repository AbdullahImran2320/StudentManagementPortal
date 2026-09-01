using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentAPI.Data;
using StudentAPI.DTOs;
using StudentAPI.Models;
using System.Security.Claims;

namespace StudentAPI.Controllers;

[ApiController]
[Route("api/academic")]
[Authorize]
public class AcademicController(AppDbContext db) : ControllerBase
{
    private bool IsAdmin() => User.IsInRole("Admin");
    private int CurrentUserId() => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;

    private async Task<bool> CanAccessSubject(int subjectId)
    {
        if (IsAdmin()) return true;
        if (!User.IsInRole("Teacher")) return false;
        var id = CurrentUserId();
        return id > 0 && await db.Subjects.AsNoTracking().AnyAsync(s => s.Id == subjectId && s.TeacherUserId == id);
    }

    [HttpGet("teachers")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Teachers() => Ok(await db.Users.AsNoTracking()
        .Where(u => u.Role == "Teacher" && u.IsApproved)
        .OrderBy(u => u.Name)
        .Select(u => new { u.Id, u.Name, u.Email })
        .ToListAsync());

    [HttpGet("classes")]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> Classes()
    {
        var query = db.AcademicClasses.AsNoTracking();
        if (!IsAdmin())
        {
            var teacherId = CurrentUserId();
            query = query.Where(c => c.Subjects.Any(s => s.TeacherUserId == teacherId));
        }
        return Ok(await query.OrderBy(x => x.Program).ThenBy(x => x.Semester).ThenBy(x => x.Section)
            .Select(x => new { x.Id, x.Program, x.Semester, x.Section, x.Session, StudentCount = x.Enrollments.Count, SubjectCount = x.Subjects.Count })
            .ToListAsync());
    }

    [HttpPost("classes")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateClass(CreateClassDto dto)
    {
        var program = dto.Program.Trim(); var section = dto.Section.Trim(); var session = dto.Session.Trim();
        if (await db.AcademicClasses.AnyAsync(x => x.Program == program && x.Semester == dto.Semester && x.Section == section && x.Session == session))
            return Conflict("This class, semester, section and session already exists.");
        var item = new AcademicClass { Program = program, Semester = dto.Semester, Section = section, Session = session };
        db.AcademicClasses.Add(item); await db.SaveChangesAsync();
        return Created($"api/academic/classes/{item.Id}", new { item.Id, item.Program, item.Semester, item.Section, item.Session });
    }

    [HttpGet("students")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Students() => Ok(await db.Students.AsNoTracking().Where(s => s.IsActive).OrderBy(s => s.Name)
        .Select(s => new { s.Id, s.Name, s.Email, s.Course, s.City }).ToListAsync());

    [HttpPost("classes/{classId:int}/students")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Enroll(int classId, EnrollStudentsDto dto)
    {
        if (!await db.AcademicClasses.AnyAsync(x => x.Id == classId)) return NotFound("Class not found.");
        var valid = await db.Students.Where(s => dto.StudentIds.Contains(s.Id) && s.IsActive).Select(s => s.Id).ToListAsync();
        var already = await db.ClassEnrollments.Where(x => x.AcademicClassId == classId && valid.Contains(x.StudentId)).Select(x => x.StudentId).ToListAsync();
        db.ClassEnrollments.AddRange(valid.Except(already).Select(id => new ClassEnrollment { AcademicClassId = classId, StudentId = id }));
        await db.SaveChangesAsync();
        return Ok(new { enrolled = valid.Count - already.Count });
    }

    [HttpGet("classes/{classId:int}/students")]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> Roster(int classId)
    {
        if (!IsAdmin() && !await db.AcademicClasses.AnyAsync(c => c.Id == classId && c.Subjects.Any(s => s.TeacherUserId == CurrentUserId()))) return Forbid();
        return Ok(await db.ClassEnrollments.AsNoTracking().Where(x => x.AcademicClassId == classId).OrderBy(x => x.Student.Name)
            .Select(x => new { x.StudentId, x.Student.Name, x.Student.Email, x.Student.Course }).ToListAsync());
    }

    [HttpGet("classes/{classId:int}/subjects")]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> Subjects(int classId)
    {
        var query = db.Subjects.AsNoTracking().Where(x => x.AcademicClassId == classId);
        if (!IsAdmin()) query = query.Where(s => s.TeacherUserId == CurrentUserId());
        return Ok(await query.OrderBy(x => x.Code).Select(x => new { x.Id, x.Code, x.Name, x.CreditHours, x.TeacherUserId, x.TeacherName }).ToListAsync());
    }

    [HttpPost("classes/{classId:int}/subjects")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateSubject(int classId, CreateSubjectDto dto)
    {
        if (!await db.AcademicClasses.AnyAsync(x => x.Id == classId)) return NotFound("Class not found.");
        var code = dto.Code.Trim();
        if (await db.Subjects.AnyAsync(x => x.AcademicClassId == classId && x.Code == code)) return Conflict("Subject code already exists in this class.");
        User? teacher = null;
        if (dto.TeacherUserId.HasValue)
        {
            teacher = await db.Users.FirstOrDefaultAsync(u => u.Id == dto.TeacherUserId.Value && u.Role == "Teacher" && u.IsApproved);
            if (teacher is null) return BadRequest("Selected teacher does not exist or is not approved.");
        }
        var subject = new Subject { AcademicClassId = classId, Code = code, Name = dto.Name.Trim(), CreditHours = dto.CreditHours, TeacherUserId = teacher?.Id, TeacherName = teacher?.Name ?? string.Empty };
        db.Subjects.Add(subject); await db.SaveChangesAsync();
        return Created($"api/academic/subjects/{subject.Id}", new { subject.Id, subject.Code, subject.Name, subject.CreditHours, subject.TeacherUserId, subject.TeacherName });
    }

    [HttpPut("subjects/{subjectId:int}/teacher")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AssignTeacher(int subjectId, [FromBody] int? teacherUserId)
    {
        var subject = await db.Subjects.FindAsync(subjectId); if (subject is null) return NotFound("Subject not found.");
        User? teacher = null;
        if (teacherUserId.HasValue) teacher = await db.Users.FirstOrDefaultAsync(u => u.Id == teacherUserId.Value && u.Role == "Teacher" && u.IsApproved);
        if (teacherUserId.HasValue && teacher is null) return BadRequest("Selected teacher does not exist or is not approved.");
        subject.TeacherUserId = teacher?.Id; subject.TeacherName = teacher?.Name ?? string.Empty; await db.SaveChangesAsync();
        return Ok(new { subject.Id, subject.TeacherUserId, subject.TeacherName });
    }

    [HttpGet("classes/{classId:int}/exams")]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> Exams(int classId)
    {
        if (!IsAdmin() && !await db.AcademicClasses.AnyAsync(c => c.Id == classId && c.Subjects.Any(s => s.TeacherUserId == CurrentUserId()))) return Forbid();
        return Ok(await db.Exams.AsNoTracking().Where(e => e.AcademicClassId == classId).OrderByDescending(e => e.ExamDate)
            .Select(e => new { e.Id, e.Name, e.ExamType, e.ExamDate, e.DefaultTotalMarks, e.IsPublished }).ToListAsync());
    }

    [HttpPost("classes/{classId:int}/exams")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateExam(int classId, CreateExamDto dto)
    {
        if (!await db.AcademicClasses.AnyAsync(c => c.Id == classId)) return NotFound("Class not found.");
        if (await db.Exams.AnyAsync(e => e.AcademicClassId == classId && e.Name == dto.Name.Trim())) return Conflict("Exam name already exists for this class.");
        var exam = new Exam { AcademicClassId = classId, Name = dto.Name.Trim(), ExamType = dto.ExamType.Trim(), ExamDate = dto.ExamDate, DefaultTotalMarks = dto.DefaultTotalMarks, IsPublished = false };
        db.Exams.Add(exam); await db.SaveChangesAsync(); return Created($"api/academic/exams/{exam.Id}", exam);
    }

    [HttpPut("exams/{examId:int}/publish")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> PublishExam(int examId, [FromBody] bool published)
    {
        var exam = await db.Exams.FindAsync(examId); if (exam is null) return NotFound("Exam not found."); exam.IsPublished = published; await db.SaveChangesAsync(); return Ok(new { exam.Id, exam.IsPublished });
    }

    [HttpGet("subjects/{subjectId:int}/attendance")]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> Attendance(int subjectId, [FromQuery] DateOnly? date)
    {
        if (!await CanAccessSubject(subjectId)) return Forbid();
        var subject = await db.Subjects.AsNoTracking().FirstOrDefaultAsync(s => s.Id == subjectId); if (subject is null) return NotFound("Subject not found.");
        var selectedDate = date ?? DateOnly.FromDateTime(DateTime.Today);
        var records = await db.AttendanceRecords.AsNoTracking().Where(x => x.SubjectId == subjectId && x.LectureDate == selectedDate)
            .ToDictionaryAsync(x => x.StudentId, x => x.Status);
        var roster = await db.ClassEnrollments.AsNoTracking().Where(e => e.AcademicClassId == subject.AcademicClassId).OrderBy(e => e.Student.Name)
            .Select(e => new { e.StudentId, StudentName = e.Student.Name, Email = e.Student.Email }).ToListAsync();
        return Ok(roster.Select(x => new { x.StudentId, x.StudentName, x.Email, LectureDate = selectedDate, Status = records.TryGetValue(x.StudentId, out var s) ? s : AttendanceStatus.NotMarked }).ToList());
    }

    [HttpPut("subjects/{subjectId:int}/attendance")]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> SaveAttendance(int subjectId, SaveAttendanceDto dto)
    {
        if (!await CanAccessSubject(subjectId)) return Forbid();
        var subject = await db.Subjects.AsNoTracking().FirstOrDefaultAsync(s => s.Id == subjectId); if (subject is null) return NotFound("Subject not found.");
        var classStudents = await db.ClassEnrollments.Where(x => x.AcademicClassId == subject.AcademicClassId).Select(x => x.StudentId).ToHashSetAsync();
        if (dto.Entries.Any(x => !classStudents.Contains(x.StudentId))) return BadRequest("Every attendance entry must belong to the selected class.");
        var existing = await db.AttendanceRecords.Where(x => x.SubjectId == subjectId && x.LectureDate == dto.LectureDate).ToListAsync();
        var byStudent = existing.ToDictionary(x => x.StudentId); var marker = User.FindFirstValue(ClaimTypes.Name) ?? "Teacher";
        foreach (var entry in dto.Entries)
        {
            if (entry.Status == AttendanceStatus.NotMarked)
            {
                if (byStudent.TryGetValue(entry.StudentId, out var remove)) db.AttendanceRecords.Remove(remove);
                continue;
            }
            if (byStudent.TryGetValue(entry.StudentId, out var record)) { record.Status = entry.Status; record.IsPresent = entry.Status == AttendanceStatus.Present; record.MarkedBy = marker; record.MarkedAt = DateTime.UtcNow; }
            else db.AttendanceRecords.Add(new AttendanceRecord { SubjectId = subjectId, StudentId = entry.StudentId, LectureDate = dto.LectureDate, Status = entry.Status, IsPresent = entry.Status == AttendanceStatus.Present, MarkedBy = marker, MarkedAt = DateTime.UtcNow });
        }
        await db.SaveChangesAsync(); return Ok(new { saved = dto.Entries.Count, date = dto.LectureDate });
    }

    [HttpGet("subjects/{subjectId:int}/marks")]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> Marks(int subjectId, [FromQuery] int examId)
    {
        if (!await CanAccessSubject(subjectId)) return Forbid();
        var subject = await db.Subjects.AsNoTracking().FirstOrDefaultAsync(s => s.Id == subjectId); if (subject is null) return NotFound("Subject not found.");
        var exam = await db.Exams.AsNoTracking().FirstOrDefaultAsync(e => e.Id == examId && e.AcademicClassId == subject.AcademicClassId); if (exam is null) return NotFound("Exam not found for this class.");
        var marks = await db.MarkRecords.AsNoTracking().Where(m => m.SubjectId == subjectId && m.ExamId == examId).ToDictionaryAsync(m => m.StudentId);
        var roster = await db.ClassEnrollments.AsNoTracking().Where(e => e.AcademicClassId == subject.AcademicClassId).OrderBy(e => e.Student.Name).Select(e => new { e.StudentId, StudentName = e.Student.Name, Email = e.Student.Email }).ToListAsync();
        return Ok(roster.Select(s => new { s.StudentId, s.StudentName, s.Email, ObtainedMarks = marks.TryGetValue(s.StudentId, out var m) ? m.ObtainedMarks : 0, TotalMarks = marks.TryGetValue(s.StudentId, out var n) ? n.TotalMarks : exam.DefaultTotalMarks, IsEntered = marks.ContainsKey(s.StudentId) }).ToList());
    }

    [HttpPut("subjects/{subjectId:int}/marks")]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> SaveMarks(int subjectId, SaveMarksDto dto)
    {
        if (!await CanAccessSubject(subjectId)) return Forbid();
        var subject = await db.Subjects.AsNoTracking().FirstOrDefaultAsync(s => s.Id == subjectId); if (subject is null) return NotFound("Subject not found.");
        var exam = await db.Exams.FirstOrDefaultAsync(e => e.Id == dto.ExamId && e.AcademicClassId == subject.AcademicClassId); if (exam is null) return BadRequest("Exam does not belong to the selected class.");
        var classStudents = await db.ClassEnrollments.Where(x => x.AcademicClassId == subject.AcademicClassId).Select(x => x.StudentId).ToHashSetAsync();
        if (dto.Entries.Any(x => !classStudents.Contains(x.StudentId) || x.ObtainedMarks > x.TotalMarks)) return BadRequest("Marks must belong to the class and obtained marks cannot exceed total marks.");
        var existing = await db.MarkRecords.Where(x => x.SubjectId == subjectId && x.ExamId == dto.ExamId).ToDictionaryAsync(x => x.StudentId);
        foreach (var entry in dto.Entries)
        {
            if (existing.TryGetValue(entry.StudentId, out var record)) { record.ObtainedMarks = entry.ObtainedMarks; record.TotalMarks = entry.TotalMarks; record.UpdatedAt = DateTime.UtcNow; }
            else db.MarkRecords.Add(new MarkRecord { SubjectId = subjectId, StudentId = entry.StudentId, ExamId = dto.ExamId, ObtainedMarks = entry.ObtainedMarks, TotalMarks = entry.TotalMarks });
        }
        await db.SaveChangesAsync(); return Ok(new { saved = dto.Entries.Count, examId = dto.ExamId });
    }

    [HttpGet("classes/{classId:int}/results")]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> Results(int classId, [FromQuery] int? examId)
    {
        if (!IsAdmin() && !await db.AcademicClasses.AnyAsync(c => c.Id == classId && c.Subjects.Any(s => s.TeacherUserId == CurrentUserId()))) return Forbid();
        var subjects = await db.Subjects.AsNoTracking().Where(s => s.AcademicClassId == classId).Select(s => new { s.Id, s.Code, s.Name, s.CreditHours }).ToListAsync();
        var students = await db.ClassEnrollments.AsNoTracking().Where(e => e.AcademicClassId == classId).OrderBy(e => e.Student.Name).Select(e => new { e.StudentId, e.Student.Name }).ToListAsync();
        var marksQuery = db.MarkRecords.AsNoTracking().Where(m => subjects.Select(s => s.Id).Contains(m.SubjectId)); if (examId.HasValue) marksQuery = marksQuery.Where(m => m.ExamId == examId.Value);
        var marks = await marksQuery.ToListAsync();
        return Ok(students.Select(student =>
        {
            var rows = subjects.Select(s => { var mark = marks.FirstOrDefault(m => m.StudentId == student.StudentId && m.SubjectId == s.Id); var percent = mark is null ? (decimal?)null : Math.Round(mark.ObtainedMarks * 100m / mark.TotalMarks, 2); return new { s.Id, s.Code, s.Name, s.CreditHours, ObtainedMarks = mark?.ObtainedMarks, TotalMarks = mark?.TotalMarks, Percent = percent, Grade = percent is null ? "Pending" : Letter(percent.Value), Point = percent is null ? 0m : GradePoint(percent.Value) }; }).ToList();
            var completed = rows.Where(r => r.Percent.HasValue).ToList(); var completedCredits = completed.Sum(r => r.CreditHours); var gpa = completedCredits == 0 ? 0 : Math.Round(completed.Sum(r => r.Point * r.CreditHours) / completedCredits, 2);
            return new { student.StudentId, student.Name, Subjects = rows, GPA = gpa, Status = completed.Count == subjects.Count && subjects.Count > 0 ? (gpa >= 2m ? "Pass" : "At risk") : "Incomplete" };
        }).ToList());
    }

    [HttpGet("me/results")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> MyResults([FromQuery] int? examId)
    {
        var email = User.FindFirstValue(ClaimTypes.Email); var student = await db.Students.FirstOrDefaultAsync(x => x.Email == email); if (student is null) return NotFound("No student profile is linked to this account.");
        var query = db.MarkRecords.AsNoTracking().Where(m => m.StudentId == student.Id); if (examId.HasValue) query = query.Where(m => m.ExamId == examId.Value);
        var rows = await query.Select(m => new { ExamId = m.ExamId, ExamName = m.Exam != null ? m.Exam.Name : "Legacy", m.Subject.Code, SubjectName = m.Subject.Name, m.Subject.CreditHours, m.ObtainedMarks, m.TotalMarks, IsPublished = m.Exam != null && m.Exam.IsPublished }).ToListAsync();
        var result = rows.Where(r => r.IsPublished || r.ExamId == null).Select(x => new { x.ExamId, x.ExamName, x.Code, x.SubjectName, x.CreditHours, x.ObtainedMarks, x.TotalMarks, Percent = Math.Round(x.ObtainedMarks * 100m / x.TotalMarks, 2), Grade = Letter(x.ObtainedMarks * 100m / x.TotalMarks), Point = GradePoint(x.ObtainedMarks * 100m / x.TotalMarks) }).ToList();
        var totalCredits = result.Sum(x => x.CreditHours); var gpa = totalCredits == 0 ? 0 : Math.Round(result.Sum(x => x.Point * x.CreditHours) / totalCredits, 2);
        return Ok(new { student.Id, student.Name, GPA = gpa, Subjects = result });
    }

    [HttpGet("me/attendance")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> MyAttendance()
    {
        var email = User.FindFirstValue(ClaimTypes.Email); var student = await db.Students.FirstOrDefaultAsync(x => x.Email == email); if (student is null) return NotFound("No student profile is linked to this account.");
        var rows = await db.AttendanceRecords.AsNoTracking().Where(x => x.StudentId == student.Id && x.Status != AttendanceStatus.NotMarked).GroupBy(x => new { x.Subject.Code, x.Subject.Name }).Select(g => new { g.Key.Code, g.Key.Name, TotalLectures = g.Count(), Present = g.Count(x => x.Status == AttendanceStatus.Present), Absent = g.Count(x => x.Status == AttendanceStatus.Absent) }).ToListAsync();
        return Ok(rows.Select(r => new { r.Code, r.Name, r.TotalLectures, r.Present, r.Absent, AttendancePercent = r.TotalLectures == 0 ? 0 : Math.Round(r.Present * 100m / r.TotalLectures, 2) }).ToList());
    }

    private static string Letter(decimal p) => p >= 85 ? "A" : p >= 80 ? "A-" : p >= 75 ? "B+" : p >= 70 ? "B" : p >= 65 ? "B-" : p >= 60 ? "C+" : p >= 55 ? "C" : p >= 50 ? "D" : "F";
    private static decimal GradePoint(decimal p) => p >= 85 ? 4m : p >= 80 ? 3.7m : p >= 75 ? 3.3m : p >= 70 ? 3m : p >= 65 ? 2.7m : p >= 60 ? 2.3m : p >= 55 ? 2m : p >= 50 ? 1.7m : 0m;
}
