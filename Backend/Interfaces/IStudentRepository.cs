using Microsoft.IdentityModel.Tokens;
using StudentAPI.DTOs;
using static StudentAPI.DTOs.StudentDTOs;
namespace StudentAPI.Interfaces
{
    public interface IStudentRepository
    {
        Task<List<StudentResponseDto>> GetAllAsync();
        Task<StudentResponseDto> GetByIdAsync(int id);
        Task<List<StudentResponseDto>> SearchAsync(string name);
        Task<List<StudentResponseDto>> GetByCityAsync(string city);
        Task<List<StudentResponseDto>> GetByCourseAsync(string course);
        Task<List<StudentResponseDto>> GetTopStudentsAsync(int count);
        Task<StudentResponseDto> CreateAsync(CreateStudentDto dto);
        Task<StudentResponseDto> UpdateAsync(int id, UpdateStudentDto dto);
        Task DeleteAsync(int id);
        Task<bool> EmailExistsAsync(string email);

        // Dashboard stats
        Task<int> GetTotalCountAsync();
        Task<double> GetAverageGPAAsync();
        Task<int> GetActiveCountAsync();

    }
}
