using System.ComponentModel.DataAnnotations;

namespace TheFinalProject.Data.Models
{
    public class Administrator : ApplicationUser
    {
        [Required(ErrorMessage = "Отдел обязателен")]
        [StringLength(100, ErrorMessage = "Отдел не должен превышать 100 символов")]
        public string Department { get; set; } = "Администрация";

        public DateTime HireDate { get; set; } = DateTime.Now;

        [StringLength(50, ErrorMessage = "ID сотрудника не должен превышать 50 символов")]
        public string? EmployeeId { get; set; }

        public bool IsSuperAdmin { get; set; } = false;

        public int TotalUsersManaged { get; set; }
        public int TotalCoursesApproved { get; set; }
    }
}