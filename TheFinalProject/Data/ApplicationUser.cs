using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TheFinalProject.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = "Имя обязательно")]
        [StringLength(50, ErrorMessage = "Имя не должно превышать 50 символов")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Фамилия обязательна")]
        [StringLength(50, ErrorMessage = "Фамилия не должна превышать 50 символов")]
        public string LastName { get; set; } = string.Empty;

        public DateTime RegistrationDate { get; set; } = DateTime.Now;
        public string? ProfilePictureUrl { get; set; }

        [StringLength(500, ErrorMessage = "Биография не должна превышать 500 символов")]
        public string? Bio { get; set; }

        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public virtual ICollection<Course> CreatedCourses { get; set; } = new List<Course>();

        public string FullName => $"{FirstName} {LastName}";
    }
}
