namespace StudentAPI.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public double GPA { get; set; }
        public string City { get; set; } = string.Empty;
        public string Course {  get; set; } = string.Empty;
         public DateTime EnrollDate { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
