using Microsoft.EntityFrameworkCore;
using StudentAPI.Models;
using Microsoft.Extensions.Options;

namespace StudentAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Student> Students { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Student>().HasData(
             new Student
             {
                 Id = 1,
                 Name = "Abdullah Imran",
                 Email = "abdullah@student.com",
                 GPA = 3.95,
                 City = "Lahore",
                 Course = "BSIT",
                 EnrollDate = new DateTime(2024, 1, 15),
                 IsActive = true
             },
                new Student
                {
                    Id = 2,
                    Name = "Ali Hassan",
                    Email = "ali@student.com",
                    GPA = 3.70,
                    City = "Karachi",
                    Course = "BSCS",
                    EnrollDate = new DateTime(2024, 1, 16),
                    IsActive = true
                },
                new Student
                {
                    Id = 3,
                    Name = "Sara Khan",
                    Email = "sara@student.com",
                    GPA = 3.85,
                    City = "Lahore",
                    Course = "BSIT",
                    EnrollDate = new DateTime(2024, 1, 17),
                    IsActive = true
                },
                new Student
                {
                    Id = 4,
                    Name = "Usman Malik",
                    Email = "usman@student.com",
                    GPA = 2.60,
                    City = "Islamabad",
                    Course = "BBA",
                    EnrollDate = new DateTime(2024, 1, 18),
                    IsActive = true
                },
                new Student
                {
                    Id = 5,
                    Name = "Ayesha Siddiqui",
                    Email = "ayesha@student.com",
                    GPA = 3.90,
                    City = "Karachi",
                    Course = "BSCS",
                    EnrollDate = new DateTime(2024, 1, 19),
                    IsActive = true
                },
                new Student
                {
                    Id = 6,
                    Name = "Bilal Ahmed",
                    Email = "bilal@student.com",
                    GPA = 3.40,
                    City = "Multan",
                    Course = "BSIT",
                    EnrollDate = new DateTime(2024, 1, 20),
                    IsActive = true
                },
                // Additional 29 Students
                new Student
                {
                    Id = 7,
                    Name = "Fatima Noor",
                    Email = "fatima@student.com",
                    GPA = 3.55,
                    City = "Faisalabad",
                    Course = "BSCS",
                    EnrollDate = new DateTime(2024, 2, 1),
                    IsActive = true
                },
                new Student
                {
                    Id = 8,
                    Name = "Hamza Ali",
                    Email = "hamza@student.com",
                    GPA = 2.80,
                    City = "Rawalpindi",
                    Course = "BSIT",
                    EnrollDate = new DateTime(2024, 2, 2),
                    IsActive = true
                },
                new Student
                {
                    Id = 9,
                    Name = "Zara Ahmed",
                    Email = "zara@student.com",
                    GPA = 3.10,
                    City = "Peshawar",
                    Course = "BBA",
                    EnrollDate = new DateTime(2024, 2, 3),
                    IsActive = true
                },
                new Student
                {
                    Id = 10,
                    Name = "Omar Farooq",
                    Email = "omar@student.com",
                    GPA = 2.40,
                    City = "Quetta",
                    Course = "BSIT",
                    EnrollDate = new DateTime(2024, 2, 4),
                    IsActive = true
                },
                new Student
                {
                    Id = 11,
                    Name = "Hina Rizvi",
                    Email = "hina@student.com",
                    GPA = 2.90,
                    City = "Lahore",
                    Course = "BSCS",
                    EnrollDate = new DateTime(2024, 2, 5),
                    IsActive = true
                },
                new Student
                {
                    Id = 12,
                    Name = "Rayan Khan",
                    Email = "rayan@student.com",
                    GPA = 3.20,
                    City = "Karachi",
                    Course = "BSIT",
                    EnrollDate = new DateTime(2024, 2, 6),
                    IsActive = true
                },
                new Student
                {
                    Id = 13,
                    Name = "Mahnoor Sheikh",
                    Email = "mahnoor@student.com",
                    GPA = 2.10,
                    City = "Islamabad",
                    Course = "BBA",
                    EnrollDate = new DateTime(2024, 2, 7),
                    IsActive = true
                },
                new Student
                {
                    Id = 14,
                    Name = "Zain Abbas",
                    Email = "zain@student.com",
                    GPA = 3.30,
                    City = "Multan",
                    Course = "BSCS",
                    EnrollDate = new DateTime(2024, 2, 8),
                    IsActive = true
                },
                new Student
                {
                    Id = 15,
                    Name = "Sana Tariq",
                    Email = "sana@student.com",
                    GPA = 2.70,
                    City = "Faisalabad",
                    Course = "BSIT",
                    EnrollDate = new DateTime(2024, 2, 9),
                    IsActive = true
                },
                new Student
                {
                    Id = 16,
                    Name = "Adeel Hussain",
                    Email = "adeel@student.com",
                    GPA = 3.58,
                    City = "Rawalpindi",
                    Course = "BBA",
                    EnrollDate = new DateTime(2024, 2, 10),
                    IsActive = true
                },
                new Student
                {
                    Id = 17,
                    Name = "Nadia Jamil",
                    Email = "nadia@student.com",
                    GPA = 2.50,
                    City = "Peshawar",
                    Course = "BSIT",
                    EnrollDate = new DateTime(2024, 2, 11),
                    IsActive = true
                },
                new Student
                {
                    Id = 18,
                    Name = "Shahid Afridi",
                    Email = "shahid@student.com",
                    GPA = 1.90,
                    City = "Quetta",
                    Course = "BSCS",
                    EnrollDate = new DateTime(2024, 2, 12),
                    IsActive = true
                },
                new Student
                {
                    Id = 19,
                    Name = "Alishba Akhtar",
                    Email = "alishba@student.com",
                    GPA = 3.00,
                    City = "Lahore",
                    Course = "BBA",
                    EnrollDate = new DateTime(2024, 2, 13),
                    IsActive = true
                },
                new Student
                {
                    Id = 20,
                    Name = "Haris Malik",
                    Email = "haris@student.com",
                    GPA = 2.30,
                    City = "Karachi",
                    Course = "BSIT",
                    EnrollDate = new DateTime(2024, 2, 14),
                    IsActive = true
                },
                new Student
                {
                    Id = 21,
                    Name = "Eman Fatima",
                    Email = "eman@student.com",
                    GPA = 3.15,
                    City = "Islamabad",
                    Course = "BSCS",
                    EnrollDate = new DateTime(2024, 2, 15),
                    IsActive = true
                },
                new Student
                {
                    Id = 22,
                    Name = "Raza Haider",
                    Email = "raza@student.com",
                    GPA = 2.85,
                    City = "Multan",
                    Course = "BSIT",
                    EnrollDate = new DateTime(2024, 2, 16),
                    IsActive = true
                },
                new Student
                {
                    Id = 23,
                    Name = "Iqra Nasir",
                    Email = "iqra@student.com",
                    GPA = 3.25,
                    City = "Faisalabad",
                    Course = "BBA",
                    EnrollDate = new DateTime(2024, 2, 17),
                    IsActive = true
                },
                new Student
                {
                    Id = 24,
                    Name = "Danish Raza",
                    Email = "danish@student.com",
                    GPA = 2.00,
                    City = "Rawalpindi",
                    Course = "BSCS",
                    EnrollDate = new DateTime(2024, 2, 18),
                    IsActive = true
                },
                new Student
                {
                    Id = 25,
                    Name = "Hira Batool",
                    Email = "hira@student.com",
                    GPA = 3.05,
                    City = "Peshawar",
                    Course = "BSIT",
                    EnrollDate = new DateTime(2024, 2, 19),
                    IsActive = true
                },
                new Student
                {
                    Id = 26,
                    Name = "Mohsin Ali",
                    Email = "mohsin@student.com",
                    GPA = 2.45,
                    City = "Quetta",
                    Course = "BBA",
                    EnrollDate = new DateTime(2024, 2, 20),
                    IsActive = true
                },
                new Student
                {
                    Id = 27,
                    Name = "Sadia Anwar",
                    Email = "sadia@student.com",
                    GPA = 3.35,
                    City = "Lahore",
                    Course = "BSCS",
                    EnrollDate = new DateTime(2024, 2, 21),
                    IsActive = true
                },
                new Student
                {
                    Id = 28,
                    Name = "Sufyan Ahmed",
                    Email = "sufyan@student.com",
                    GPA = 2.15,
                    City = "Karachi",
                    Course = "BSIT",
                    EnrollDate = new DateTime(2024, 2, 22),
                    IsActive = true
                },
                new Student
                {
                    Id = 29,
                    Name = "Aleena Khan",
                    Email = "aleena@student.com",
                    GPA = 2.75,
                    City = "Islamabad",
                    Course = "BBA",
                    EnrollDate = new DateTime(2024, 2, 23),
                    IsActive = true
                },
                new Student
                {
                    Id = 30,
                    Name = "Hassan Raza",
                    Email = "hassanr@student.com",
                    GPA = 1.80,
                    City = "Multan",
                    Course = "BSCS",
                    EnrollDate = new DateTime(2024, 2, 24),
                    IsActive = true
                },
                new Student
                {
                    Id = 31,
                    Name = "Mariam Asif",
                    Email = "mariam@student.com",
                    GPA = 3.50,
                    City = "Faisalabad",
                    Course = "BSIT",
                    EnrollDate = new DateTime(2024, 2, 25),
                    IsActive = true
                },
                new Student
                {
                    Id = 32,
                    Name = "Taha Ahmed",
                    Email = "taha@student.com",
                    GPA = 2.20,
                    City = "Rawalpindi",
                    Course = "BBA",
                    EnrollDate = new DateTime(2024, 2, 26),
                    IsActive = true
                },
                new Student
                {
                    Id = 33,
                    Name = "Mehak Aslam",
                    Email = "mehak@student.com",
                    GPA = 2.95,
                    City = "Peshawar",
                    Course = "BSCS",
                    EnrollDate = new DateTime(2024, 2, 27),
                    IsActive = true
                },
                new Student
                {
                    Id = 34,
                    Name = "Usama Chaudhry",
                    Email = "usamac@student.com",
                    GPA = 2.60,
                    City = "Quetta",
                    Course = "BSIT",
                    EnrollDate = new DateTime(2024, 2, 28),
                    IsActive = true
                },
                new Student
                {
                    Id = 35,
                    Name = "Hania Aamir",
                    Email = "hania@student.com",
                    GPA = 3.60,
                    City = "Lahore",
                    Course = "BBA",
                    EnrollDate = new DateTime(2024, 3, 1),
                    IsActive = true
                }
            );

            // Seed Admin User
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Name = "Admin",
                    Email = "admin@gmail.com",
                    PasswordHash = "$2b$11$Glz.i7DzU/P8b4/9n/Hwc.EndWg4drs7VofW3FEM4/5GghBZrYdwG",
                    Role = "Admin",
                    CreatedAt = new DateTime(2026, 07, 21)
                },
                new User
                {
                    Id = 2,
                    Name = "abdullah",
                    Email = "abdullah@gmail.com",
                    PasswordHash = "$2b$11$Glz.i7DzU/P8b4/9n/Hwc.EndWg4drs7VofW3FEM4/5GghBZrYdwG",
                    Role = "admin",
                    CreatedAt = new DateTime(2026, 06, 21)
                }
            );
        }
    }
}