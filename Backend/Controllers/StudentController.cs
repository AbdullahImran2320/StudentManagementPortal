
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentAPI.DTOs;
using StudentAPI.Interfaces;
namespace StudentAPI.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentController : ControllerBase
    {
        private readonly IStudentRepository _repo;
        private readonly ILogger<StudentController> _log;

        public StudentController(IStudentRepository repo, ILogger<StudentController> log) 
        {
            _log = log;
            _repo = repo;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Getall()
        {
            _log.LogInformation("Getting all students!");
            var st = await _repo.GetAllAsync() ;
            return Ok(st);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            _log.LogInformation($"Getting the student by Id: {id}");
            var st= await _repo.GetByIdAsync(id) ; return Ok(st);
        }

        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<IActionResult> Search([FromQuery]string name)
        {
            if (string.IsNullOrEmpty(name)) {
                return BadRequest("Name required!");
            }
            _log.LogInformation($"Searching the student: {name}");
            var st= await _repo.SearchAsync(name) ; return Ok(st);
        }

        [HttpGet("city/{city}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByCity(string city)
        {
            _log.LogInformation($"Getting th student by city:{city}");
            var st= await _repo.GetByCityAsync(city) ; return Ok(st);
        }

        [HttpGet("course/{course}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByCourse(string course)
        {
            _log.LogInformation($"Searching the student: {course}");
            var st= await _repo.GetByCourseAsync(course) ; return Ok(st);
        }

        [HttpGet("top")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTop([FromQuery]int count=5)
        {
            _log.LogInformation("Getting the Top students");
            var st= await _repo.GetTopStudentsAsync(count) ; return Ok(st);
        }

        [HttpGet("Stats")]
        [AllowAnonymous]
        public async Task<IActionResult> GetStats()
        {
            var total = await _repo.GetTotalCountAsync();
            var active = await _repo.GetActiveCountAsync();
            var avgGPA = await _repo.GetAverageGPAAsync() ; return Ok(new { 
            TotalStudents = total, 
            ActiveStudents = active, 
            AverageGPA = Math.Round(avgGPA, 2)
            });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateStudentDto dto)
        {
            _log.LogInformation($"Creating Student: {dto.Name}");
            var st = await _repo.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = st.Id }, st);
        }

        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateStudentDto dto)
        {
            _log.LogInformation($"Updating Student: {dto.Name}");
            var st = await _repo.UpdateAsync(id, dto);
            return Ok(st);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles="Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            _log.LogInformation($"Deleting students: {id}");
            await _repo.DeleteAsync(id);
            return NoContent();
        }
    }
}
