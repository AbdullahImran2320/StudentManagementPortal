using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models ;
using StudentAPI.Data;
using StudentAPI.Interfaces;
using StudentAPI.Middleware;
using StudentAPI.Models;
using StudentAPI.Repositories;
using StudentAPI.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.Configure<SmsSettings>(builder.Configuration.GetSection("SmsSettings"));

// ── JWT SETTINGS ──────────────────────────────
var secretKey = builder.Configuration[
    "JwtSettings:SecretKey"]!;
var issuer = builder.Configuration[
    "JwtSettings:Issuer"]!;
var audience = builder.Configuration[
    "JwtSettings:Audience"]!;
var keyBytes = Encoding.UTF8.GetBytes(secretKey);

// ── DATABASE ──────────────────────────────────
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration
               .GetConnectionString("DefaultConnection")
    )
);

// ── CORS ──────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:4200",
                "https://localhost:4200"
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// ── REPOSITORIES & SERVICES ───────────────────
builder.Services.AddScoped<IStudentRepository,
                            StudentRepositories>();
builder.Services.AddScoped<IAuthService,
                            AuthService>();
builder.Services.AddScoped<IEmailService,
                            EmailService>();
builder.Services.AddScoped<ISmsService, SmsService>();


// ── JWT AUTHENTICATION ────────────────────────
builder.Services
    .AddAuthentication(options => {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options => {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey =
                    new SymmetricSecurityKey(keyBytes),
                ClockSkew = TimeSpan.Zero
            };
    });

builder.Services.AddAuthorization();

// ── SWAGGER ───────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
    options.AddSecurityDefinition("Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter JWT token!"
        });
    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement {
            {
                new OpenApiSecurityScheme {
                    Reference = new OpenApiReference {
                        Type = ReferenceType.SecurityScheme,
                        Id   = "Bearer"
                    }
                },
                new string[] {}
            }
        });
});


// ── BUILD ─────────────────────────────────────
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.Migrate();

        // Idempotent upgrade script for installations that already have the academic module.
        db.Database.ExecuteSqlRaw("""
            IF OBJECT_ID(N'[AcademicClasses]', N'U') IS NULL
            BEGIN
              CREATE TABLE [AcademicClasses] (
                [Id] int IDENTITY(1,1) NOT NULL PRIMARY KEY,
                [Program] nvarchar(100) NOT NULL, [Semester] int NOT NULL,
                [Section] nvarchar(20) NOT NULL, [Session] nvarchar(20) NOT NULL
              );
              CREATE UNIQUE INDEX [UX_AcademicClasses_Key] ON [AcademicClasses]([Program],[Semester],[Section],[Session]);
            END;
            IF OBJECT_ID(N'[Subjects]', N'U') IS NULL
            BEGIN
              CREATE TABLE [Subjects] (
                [Id] int IDENTITY(1,1) NOT NULL PRIMARY KEY, [AcademicClassId] int NOT NULL,
                [Code] nvarchar(20) NOT NULL, [Name] nvarchar(150) NOT NULL,
                [CreditHours] int NOT NULL, [TeacherName] nvarchar(100) NOT NULL,
                [TeacherUserId] int NULL,
                CONSTRAINT [FK_Subjects_AcademicClasses] FOREIGN KEY ([AcademicClassId]) REFERENCES [AcademicClasses]([Id]) ON DELETE CASCADE,
                CONSTRAINT [FK_Subjects_TeacherUser] FOREIGN KEY ([TeacherUserId]) REFERENCES [Users]([Id]) ON DELETE SET NULL
              );
              CREATE UNIQUE INDEX [UX_Subjects_Class_Code] ON [Subjects]([AcademicClassId],[Code]);
              CREATE INDEX [IX_Subjects_TeacherUserId] ON [Subjects]([TeacherUserId]);
            END;
            IF OBJECT_ID(N'[ClassEnrollments]', N'U') IS NULL
            BEGIN
              CREATE TABLE [ClassEnrollments] (
                [Id] int IDENTITY(1,1) NOT NULL PRIMARY KEY, [AcademicClassId] int NOT NULL,
                [StudentId] int NOT NULL, [EnrolledAt] datetime2 NOT NULL,
                CONSTRAINT [FK_ClassEnrollments_AcademicClasses] FOREIGN KEY ([AcademicClassId]) REFERENCES [AcademicClasses]([Id]) ON DELETE CASCADE,
                CONSTRAINT [FK_ClassEnrollments_Students] FOREIGN KEY ([StudentId]) REFERENCES [Students]([Id]) ON DELETE CASCADE
              );
              CREATE UNIQUE INDEX [UX_ClassEnrollments_Class_Student] ON [ClassEnrollments]([AcademicClassId],[StudentId]);
            END;
            IF OBJECT_ID(N'[AttendanceRecords]', N'U') IS NULL
            BEGIN
              CREATE TABLE [AttendanceRecords] (
                [Id] int IDENTITY(1,1) NOT NULL PRIMARY KEY, [SubjectId] int NOT NULL, [StudentId] int NOT NULL,
                [LectureDate] date NOT NULL, [Status] tinyint NOT NULL CONSTRAINT [DF_Attendance_Status] DEFAULT 0,
                [IsPresent] bit NOT NULL CONSTRAINT [DF_Attendance_IsPresent] DEFAULT 0,
                [MarkedBy] nvarchar(max) NOT NULL, [MarkedAt] datetime2 NOT NULL,
                CONSTRAINT [FK_AttendanceRecords_Subjects] FOREIGN KEY ([SubjectId]) REFERENCES [Subjects]([Id]) ON DELETE CASCADE,
                CONSTRAINT [FK_AttendanceRecords_Students] FOREIGN KEY ([StudentId]) REFERENCES [Students]([Id]) ON DELETE CASCADE
              );
              CREATE UNIQUE INDEX [UX_AttendanceRecords_Subject_Student_Date] ON [AttendanceRecords]([SubjectId],[StudentId],[LectureDate]);
            END;
            IF OBJECT_ID(N'[Exams]', N'U') IS NULL
            BEGIN
              CREATE TABLE [Exams] (
                [Id] int IDENTITY(1,1) NOT NULL PRIMARY KEY, [AcademicClassId] int NOT NULL,
                [Name] nvarchar(100) NOT NULL, [ExamType] nvarchar(30) NOT NULL,
                [ExamDate] date NOT NULL, [DefaultTotalMarks] decimal(18,2) NOT NULL,
                [IsPublished] bit NOT NULL CONSTRAINT [DF_Exams_IsPublished] DEFAULT 0,
                CONSTRAINT [FK_Exams_AcademicClasses] FOREIGN KEY ([AcademicClassId]) REFERENCES [AcademicClasses]([Id]) ON DELETE CASCADE
              );
              CREATE UNIQUE INDEX [UX_Exams_Class_Name] ON [Exams]([AcademicClassId],[Name]);
            END;
            IF OBJECT_ID(N'[MarkRecords]', N'U') IS NULL
            BEGIN
              CREATE TABLE [MarkRecords] (
                [Id] int IDENTITY(1,1) NOT NULL PRIMARY KEY, [SubjectId] int NOT NULL, [StudentId] int NOT NULL,
                [ExamId] int NULL, [ObtainedMarks] decimal(18,2) NOT NULL, [TotalMarks] decimal(18,2) NOT NULL, [UpdatedAt] datetime2 NOT NULL,
                CONSTRAINT [FK_MarkRecords_Subjects] FOREIGN KEY ([SubjectId]) REFERENCES [Subjects]([Id]) ON DELETE CASCADE,
                CONSTRAINT [FK_MarkRecords_Students] FOREIGN KEY ([StudentId]) REFERENCES [Students]([Id]) ON DELETE CASCADE,
                CONSTRAINT [FK_MarkRecords_Exams] FOREIGN KEY ([ExamId]) REFERENCES [Exams]([Id]) ON DELETE SET NULL
              );
              CREATE UNIQUE INDEX [UX_MarkRecords_Exam_Subject_Student] ON [MarkRecords]([ExamId],[SubjectId],[StudentId]);
            END;

            IF COL_LENGTH('Subjects', 'TeacherUserId') IS NULL ALTER TABLE [Subjects] ADD [TeacherUserId] int NULL;
            IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Subjects_TeacherUser') ALTER TABLE [Subjects] ADD CONSTRAINT [FK_Subjects_TeacherUser] FOREIGN KEY ([TeacherUserId]) REFERENCES [Users]([Id]) ON DELETE SET NULL;
            IF COL_LENGTH('AttendanceRecords', 'Status') IS NULL ALTER TABLE [AttendanceRecords] ADD [Status] tinyint NOT NULL CONSTRAINT [DF_Attendance_Status_Upgrade] DEFAULT 0;
            IF COL_LENGTH('MarkRecords', 'ExamId') IS NULL ALTER TABLE [MarkRecords] ADD [ExamId] int NULL;
            IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('MarkRecords') AND name = 'UX_MarkRecords_Subject_Student') DROP INDEX [UX_MarkRecords_Subject_Student] ON [MarkRecords];
            IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('MarkRecords') AND name = 'IX_MarkRecords_SubjectId_StudentId') DROP INDEX [IX_MarkRecords_SubjectId_StudentId] ON [MarkRecords];
            IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_MarkRecords_Exams') ALTER TABLE [MarkRecords] ADD CONSTRAINT [FK_MarkRecords_Exams] FOREIGN KEY ([ExamId]) REFERENCES [Exams]([Id]) ON DELETE SET NULL;
            IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('MarkRecords') AND name = 'UX_MarkRecords_Exam_Subject_Student') CREATE UNIQUE INDEX [UX_MarkRecords_Exam_Subject_Student] ON [MarkRecords]([ExamId],[SubjectId],[StudentId]);
            IF COL_LENGTH('Users', 'RequestedRole') IS NULL ALTER TABLE [Users] ADD [RequestedRole] nvarchar(20) NOT NULL CONSTRAINT [DF_Users_RequestedRole] DEFAULT 'Student';
            IF COL_LENGTH('Users', 'IsApproved') IS NULL ALTER TABLE [Users] ADD [IsApproved] bit NOT NULL CONSTRAINT [DF_Users_IsApproved] DEFAULT 0;
            UPDATE [AttendanceRecords] SET [Status] = CASE WHEN [IsPresent] = 1 THEN 1 ELSE 2 END WHERE [Status] = 0 AND [IsPresent] IS NOT NULL;
            UPDATE [Users] SET [Role] = 'Admin' WHERE LOWER([Role]) = 'admin' AND [Role] <> 'Admin';
            UPDATE [Users] SET [Role] = 'Teacher' WHERE LOWER([Role]) = 'teacher' AND [Role] <> 'Teacher';
            UPDATE [Users] SET [Role] = 'Student' WHERE LOWER([Role]) = 'student' AND [Role] <> 'Student';
            """);

        // Ensure the standard administrator exists and is usable on every installation.
        var admin = db.Users.FirstOrDefault(u => u.Email == "admin@portal.local");
        if (admin is null)
        {
            db.Users.Add(new User { Name = "Principal", Email = "admin@portal.local", Role = "Admin", RequestedRole = "Admin", IsApproved = true, PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin 123"), CreatedAt = DateTime.UtcNow });
        }
        else
        {
            admin.Name = "Principal"; admin.Role = "Admin"; admin.RequestedRole = "Admin"; admin.IsApproved = true;
            if (!BCrypt.Net.BCrypt.Verify("admin 123", admin.PasswordHash)) admin.PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin 123");
        }
        db.SaveChanges();

        // ── Sample academic data (idempotent — safe to re-run on every startup) ──
        // 12 teacher accounts, pre-approved so they can log in immediately.
        var teacherNames = new[] {
            "Usman Tariq","Ayesha Siddiqui","Bilal Ahmed","Sana Malik","Hamza Sheikh","Farah Naz",
            "Imran Qureshi","Nadia Aslam","Kashif Iqbal","Rabia Yousaf","Adnan Khalid","Mehwish Farooq"
        };
        var teacherUsers = new List<User>();
        foreach (var tName in teacherNames)
        {
            var tEmail = tName.ToLowerInvariant().Replace(" ", ".") + "@bayheights.edu.pk";
            var t = db.Users.FirstOrDefault(u => u.Email == tEmail);
            if (t is null)
            {
                t = new User { Name = tName, Email = tEmail, Phone = "03000000000", Role = "Teacher", RequestedRole = "Teacher", IsApproved = true, PasswordHash = BCrypt.Net.BCrypt.HashPassword("teacher123"), CreatedAt = DateTime.UtcNow };
                db.Users.Add(t);
            }
            teacherUsers.Add(t);
        }
        db.SaveChanges();

        // Pad the student roster up to 50 (35 already ship via migration seed data).
        var extraStudentNames = new[] {
            "Hassan Raza","Kiran Shahid","Waqas Anwar","Amna Riaz","Junaid Baig","Sadia Perveen",
            "Owais Chaudhry","Rida Fatima","Salman Yousuf","Nimra Aftab","Talha Mahmood","Zoya Khan",
            "Faizan Butt","Anum Shakeel","Arslan Nazir"
        };
        var cities = new[] { "Lahore", "Karachi", "Islamabad", "Multan", "Faisalabad", "Rawalpindi" };
        var rnd = new Random(42);
        var studentCount = db.Students.Count();
        for (int i = 0; i < extraStudentNames.Length && studentCount < 50; i++)
        {
            var sName = extraStudentNames[i];
            var sEmail = sName.ToLowerInvariant().Replace(" ", ".") + "@student.com";
            if (!db.Students.Any(s => s.Email == sEmail))
            {
                db.Students.Add(new Student { Name = sName, Email = sEmail, GPA = Math.Round(2.8 + rnd.NextDouble() * 1.2, 2), City = cities[i % cities.Length], Course = "BSIT", EnrollDate = DateTime.UtcNow.AddMonths(-rnd.Next(1, 24)), IsActive = true });
                studentCount++;
            }
        }
        db.SaveChanges();

        // 8 semesters × 2 morning sections (Morning-A / Morning-B) for session 2026.
        const string session = "2026", program = "BSIT";
        var classes = new List<AcademicClass>();
        for (int sem = 1; sem <= 8; sem++)
        {
            foreach (var section in new[] { "Morning-A", "Morning-B" })
            {
                var cls = db.AcademicClasses.FirstOrDefault(c => c.Program == program && c.Semester == sem && c.Section == section && c.Session == session);
                if (cls is null)
                {
                    cls = new AcademicClass { Program = program, Semester = sem, Section = section, Session = session };
                    db.AcademicClasses.Add(cls);
                    db.SaveChanges();
                }
                classes.Add(cls);
            }
        }

        // Enroll every active student into one of the 16 classes, spread evenly.
        var allStudents = db.Students.Where(s => s.IsActive).OrderBy(s => s.Id).ToList();
        for (int i = 0; i < allStudents.Count; i++)
        {
            var cls = classes[i % classes.Count];
            if (!db.ClassEnrollments.Any(e => e.AcademicClassId == cls.Id && e.StudentId == allStudents[i].Id))
                db.ClassEnrollments.Add(new ClassEnrollment { AcademicClassId = cls.Id, StudentId = allStudents[i].Id, EnrolledAt = DateTime.UtcNow });
        }
        db.SaveChanges();

        // Give every class one subject taught by a rotating teacher, so all 12 teachers have real assignments.
        var subjectCatalog = new[] { "Programming Fundamentals","Data Structures","Database Systems","Web Engineering","Operating Systems","Software Engineering","Computer Networks","Artificial Intelligence" };
        for (int i = 0; i < classes.Count; i++)
        {
            var cls = classes[i];
            var code = $"CS-{100 + cls.Semester * 10}";
            if (!db.Subjects.Any(s => s.AcademicClassId == cls.Id && s.Code == code))
            {
                var teacher = teacherUsers[i % teacherUsers.Count];
                db.Subjects.Add(new Subject { AcademicClassId = cls.Id, Code = code, Name = subjectCatalog[(cls.Semester - 1) % subjectCatalog.Length], CreditHours = 3, TeacherUserId = teacher.Id, TeacherName = teacher.Name });
            }
        }
        db.SaveChanges();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[Startup] Database initialization failed: {ex.Message}");
    }
}

app.UseGlobalExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseDefaultFiles();
    app.UseStaticFiles();
}

if (!app.Environment.IsDevelopment()) app.UseHttpsRedirection();
app.UseCors("AllowAngular");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
if (!app.Environment.IsDevelopment()) app.MapFallbackToFile("index.html");
app.Run();
