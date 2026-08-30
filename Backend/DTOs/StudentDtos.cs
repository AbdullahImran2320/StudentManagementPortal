using System.ComponentModel.DataAnnotations;

namespace StudentAPI.DTOs
{
    public class StudentDTOs
    {
        public class StudentResponseDto
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public double GPA { get; set; }
            public string City { get; set; } = string.Empty;
            public string Course { get; set; } = string.Empty;
            public DateTime EnrollDate { get; set; }
            public bool IsActive { get; set; } = true;
        }
    }

    public class CreateStudentDto
    {
        [Required(ErrorMessage ="Name is required!")]
        [StringLength (20, MinimumLength = 3, ErrorMessage ="Name should be 3-20 caharcters long")]
        public string Name { get; set; } = string.Empty ;

        [Required(ErrorMessage = "Email is required!")]
        [EmailAddress]
        public string Email { get; set;} = string.Empty;

        [Required]
        [Range(0.0, 4.0, ErrorMessage ="GPA should be between 0-4!")]
        public double GPA { get; set; }

        [Required]
        [StringLength(20, MinimumLength =3, ErrorMessage ="Invalid City name!")]
        public string City { get; set;} = string.Empty;
        [Required]
        [StringLength(20, MinimumLength =3, ErrorMessage ="Invalid Course name!")]
        public string Course { get; set;} = string.Empty;

    }

    public class UpdateStudentDto
    {
        [Required(ErrorMessage = "Name is required!")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Name should be 3-20 caharcters long")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(0.0, 4.0, ErrorMessage = "GPA should be between 0-4!")]
        public double GPA { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Invalid City name!")]
        public string City { get; set; } = string.Empty;
        [Required]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Invalid Course name!")]
        public string Course { get; set; } = string.Empty;


    }

}

