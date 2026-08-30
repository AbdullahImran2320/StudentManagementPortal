using Microsoft.EntityFrameworkCore;
using StudentAPI.Data;
using StudentAPI.DTOs;
using StudentAPI.Exceptions;
using StudentAPI.Interfaces;
using StudentAPI.Models;

using static StudentAPI.DTOs.StudentDTOs;
using static StudentAPI.Exceptions.AppExceptions;

namespace StudentAPI.Repositories
{
    public class StudentRepositories : IStudentRepository
    {
        private readonly AppDbContext _db;
        public StudentRepositories(AppDbContext db) 
        {
           _db = db;
        }

        private StudentResponseDto Maptodo(Student s) => new StudentResponseDto
        {
            Id = s.Id,
            Name = s.Name,
            Email = s.Email,
            City = s.City, 
            GPA = s. GPA,
            Course = s.Course,
            EnrollDate = s.EnrollDate,
            IsActive = s.IsActive,
        };

        public async Task<List<StudentResponseDto>> GetAllAsync()
        {
            var students = await _db.Students.Where(s => s.IsActive).OrderBy(s => s.Id).ToListAsync();
            return students.Select(s=> Maptodo(s)).ToList();
        }


        public async Task<StudentResponseDto> GetByIdAsync(int id)
        {
            var students = await _db.Students.FindAsync(id);
            if (students == null)
                throw new NotFoundException("Student", id);
            return Maptodo(students);
        }
        public async Task<List<StudentResponseDto>> SearchAsync(string name)
        {
            var students = await _db.Students.Where(s => s.Name.ToLower().Contains(name.ToLower())&& s.IsActive).ToListAsync();
            return students.Select(s => Maptodo(s)).ToList();
        }


        public async Task<List<StudentResponseDto>>  GetByCityAsync(string city)
        {
            var students = await _db.Students.Where(s => s.City.ToLower() == city.ToLower() && s.IsActive).ToListAsync();
            return students.Select(s => Maptodo(s)).ToList();
        }
        public async Task<List<StudentResponseDto>>  GetByCourseAsync(string course)
        {
            var students = await _db.Students.Where(s => s.Course.ToLower() == course.ToLower() && s.IsActive).ToListAsync();
            return students.Select(s => Maptodo(s)).ToList();
        }
        public async Task<List<StudentResponseDto>> GetTopStudentsAsync(int count)
        {
            var students = await  _db.Students.Where(s => s.IsActive).OrderByDescending(s => s.GPA).Take(count).ToListAsync();
            return students.Select(s => Maptodo(s)).ToList();
        }

        public async Task<StudentResponseDto> CreateAsync(CreateStudentDto dto)
        {
            if (await EmailExistsAsync(dto.Email))
                throw new ConflictException($"Email {dto.Email} already registered!");

            var student = new Student{
                Name       = dto.Name,
                Email      = dto.Email,
                GPA        = dto.GPA,
                City       = dto.City,
                Course     = dto.Course,
                EnrollDate = DateTime.Now,
                IsActive   = true
            };

            _db.Students.Add(student);
            await _db.SaveChangesAsync();

            return Maptodo(student);

        }

        public async Task<StudentResponseDto> UpdateAsync(int id, UpdateStudentDto dto)
        {
            var student = await _db.Students.FindAsync(id);
            if (student == null)
                throw new NotFoundException("Student", id);

            student.Name = dto.Name;
            student.GPA = dto.GPA;
            student.City = dto.City;
            student.Course = dto.Course;

            await _db.SaveChangesAsync();
            return Maptodo(student);
        }
        public async Task DeleteAsync(int id)
        {
            var student = await _db.Students.FindAsync(id);
            if (student == null)
                throw new NotFoundException("Student", id);

            student.IsActive = false;
            await _db.SaveChangesAsync();
        }
        public async Task<bool> EmailExistsAsync(
         string email)
        {
            return await _db.Students
                .AnyAsync(s => s.Email == email);
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _db.Students.CountAsync();
        }

        public async Task<double> GetAverageGPAAsync()
        {
            if (!await _db.Students.AnyAsync())
                return 0;
            return await _db.Students
                .AverageAsync(s => s.GPA);
        }

        public async Task<int> GetActiveCountAsync()
        {
            return await _db.Students
                .CountAsync(s => s.IsActive);
        }


    }
}
