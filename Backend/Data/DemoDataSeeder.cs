using StudentAPI.Data;
using StudentAPI.Models;

namespace StudentAPI.Data
{
    /// <summary>
    /// One-time programmatic demo-data generator.
    /// Creates: 8 semesters x 2 sections (Morning-A / Morning-B) x 25 students = 400 students,
    /// 12 teacher accounts, 5 subjects per class assigned round-robin to teachers,
    /// and enrollment records linking students to their class.
    ///
    /// ASSUMPTIONS (confirm against your real Models before running):
    ///  - AcademicClass has: Program, Semester, Section, Session
    ///  - Subject has: Name, Code, AcademicClassId, TeacherUserId
    ///  - ClassEnrollment has: AcademicClassId, StudentId
    ///  - Student has: Name, Email, GPA, City, Course, EnrollDate, IsActive
    ///  - User has: Name, Email, PasswordHash, Role, RequestedRole, IsApproved, CreatedAt, Phone
    /// If any property name differs in your actual classes, this won't compile —
    /// send me Student.cs / User.cs / AcademicClass.cs / Subject.cs / ClassEnrollment.cs
    /// and I'll correct the field names.
    /// </summary>
    public static class DemoDataSeeder
    {
        // All demo accounts (teachers) use this password: Admin@12345
        private const string DemoPasswordHash =
            "$2b$12$h2f4V88gAEzLB3FZXTPhc.OzrKnKMUkRp3ayIuRalNW1JTzNlosD2";

        private static readonly string[] FirstNames = {
            "Ahmed","Ali","Hassan","Hussain","Bilal","Usman","Omar","Zain","Hamza","Faisal",
            "Kamran","Salman","Tariq","Adeel","Waqas","Imran","Rizwan","Junaid","Saad","Shahid",
            "Fatima","Ayesha","Sara","Zara","Hina","Mahnoor","Sana","Nadia","Iqra","Hira",
            "Mariam","Mehak","Hania","Alishba","Eman","Aleena","Sadia","Rabia","Amna","Laiba",
            "Danish","Sufyan","Taha","Haris","Raza","Mohsin","Asad","Fahad","Naveed","Yasir"
        };

        private static readonly string[] LastNames = {
            "Khan","Malik","Sheikh","Ahmed","Raza","Hussain","Abbas","Chaudhry","Farooq","Rizvi",
            "Siddiqui","Anwar","Aslam","Nasir","Butt","Iqbal","Javed","Qureshi","Baig","Shah",
            "Akhtar","Bhatti","Gill","Warraich","Cheema","Ansari","Mirza","Dar","Awan","Sial"
        };

        private static readonly string[] SubjectPool = {
            "Programming Fundamentals","Data Structures","Database Systems","Operating Systems",
            "Computer Networks","Software Engineering","Web Development","Algorithms",
            "Artificial Intelligence","Discrete Mathematics","Linear Algebra","Statistics",
            "Object Oriented Programming","Computer Architecture","Compiler Construction"
        };

        public static void Seed(AppDbContext db)
        {
            // Guard: don't reseed if demo classes already exist
            if (db.AcademicClasses.Any(c => c.Session == "2024-Demo"))
                return;

            var rnd = new Random(42); // fixed seed => reproducible data
            var nameCounter = 0;

            string NextFullName()
            {
                var fn = FirstNames[nameCounter % FirstNames.Length];
                var ln = LastNames[(nameCounter / FirstNames.Length) % LastNames.Length];
                nameCounter++;
                return $"{fn} {ln}";
            }

            // ---------- 1. Teachers (12 User accounts, Role = Teacher) ----------
            var teachers = new List<User>();
            for (int i = 1; i <= 12; i++)
            {
                var name = NextFullName();
                var user = new User
                {
                    Name = name,
                    Email = $"teacher{i}@school.com",
                    Phone = $"03000{i:D6}",
                    PasswordHash = DemoPasswordHash,
                    Role = "Teacher",
                    RequestedRole = "Teacher",
                    IsApproved = true,
                    CreatedAt = DateTime.UtcNow
                };
                teachers.Add(user);
            }
            db.Users.AddRange(teachers);
            db.SaveChanges(); // need teacher Ids before assigning to subjects

            // ---------- 2. Classes: 8 semesters x 2 sections ----------
            var classes = new List<AcademicClass>();
            for (int sem = 1; sem <= 8; sem++)
            {
                foreach (var section in new[] { "Morning-A", "Morning-B" })
                {
                    classes.Add(new AcademicClass
                    {
                        Program = "BSCS",
                        Semester = sem,
                        Section = section,
                        Session = "2024-Demo"
                    });
                }
            }
            db.AcademicClasses.AddRange(classes);
            db.SaveChanges(); // need class Ids for subjects/enrollments

            // ---------- 3. Subjects: 5 per class, teachers assigned round-robin ----------
            var subjects = new List<Subject>();
            int teacherIdx = 0;
            foreach (var cls in classes)
            {
                var shuffled = SubjectPool.OrderBy(_ => rnd.Next()).Take(5).ToList();
                foreach (var subjName in shuffled)
                {
                    var code = $"{cls.Program}{cls.Semester}{cls.Section[8]}{subjects.Count % 100:D2}";
                    var teacher = teachers[teacherIdx % teachers.Count];
                    subjects.Add(new Subject
                    {
                        Name = subjName,
                        Code = code,
                        CreditHours = 3,
                        AcademicClassId = cls.Id,
                        TeacherUserId = teacher.Id,
                        TeacherName = teacher.Name
                    });
                    teacherIdx++;
                }
            }
            db.Subjects.AddRange(subjects);

            // ---------- 4. Students: 25 per section (50 per semester, 400 total) ----------
            var students = new List<Student>();
            var enrollments = new List<ClassEnrollment>();
            var cities = new[] { "Lahore", "Karachi", "Islamabad", "Rawalpindi", "Faisalabad", "Multan", "Peshawar", "Quetta" };

            foreach (var cls in classes)
            {
                for (int i = 0; i < 25; i++)
                {
                    var name = NextFullName();
                    var emailSafe = name.ToLower().Replace(" ", ".") + (students.Count);
                    var student = new Student
                    {
                        Name = name,
                        Email = $"{emailSafe}@student.com",
                        GPA = Math.Round(2.0 + rnd.NextDouble() * 2.0, 2), // 2.00 - 4.00
                        City = cities[rnd.Next(cities.Length)],
                        Course = cls.Program,
                        EnrollDate = new DateTime(2024, 1, 1).AddDays(rnd.Next(0, 300)),
                        IsActive = true
                    };
                    students.Add(student);
                }
            }
            db.Students.AddRange(students);
            db.SaveChanges(); // need student Ids for enrollments

            // Re-walk classes/students in the same order to build enrollments
            int studentPointer = 0;
            foreach (var cls in classes)
            {
                for (int i = 0; i < 25; i++)
                {
                    enrollments.Add(new ClassEnrollment
                    {
                        AcademicClassId = cls.Id,
                        StudentId = students[studentPointer].Id
                    });
                    studentPointer++;
                }
            }
            db.ClassEnrollments.AddRange(enrollments);

            db.SaveChanges();
        }
    }
}