namespace StudentAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string RequestedRole { get; set; } = "Student";
        public bool IsApproved { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public ICollection<Subject> TeachingSubjects { get; set; } = new List<Subject>();

    }
}
