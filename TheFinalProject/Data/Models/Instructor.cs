using System.ComponentModel.DataAnnotations;

namespace TheFinalProject.Data.Models
{
    public class Instructor : ApplicationUser
    {
        [StringLength(200, ErrorMessage = "Специализация не должна превышать 200 символов")]
        public string? Specialization { get; set; }

        [StringLength(500, ErrorMessage = "Квалификация не должна превышать 500 символов")]
        public string? Qualifications { get; set; }

        [Range(0, 100, ErrorMessage = "Опыт должен быть от 0 до 100 лет")]
        public int YearsOfExperience { get; set; }

        [Url(ErrorMessage = "Неверный формат URL")]
        public string? LinkedInUrl { get; set; }

        [Url(ErrorMessage = "Неверный формат URL")]
        public string? PortfolioUrl { get; set; }

        public int TotalCoursesCreated => CreatedCourses.Count;
        public int PublishedCoursesCount => CreatedCourses.Count(c => c.IsPublished);

        public double AverageCourseRating => CreatedCourses
            .Where(c => c.Rating > 0)
            .Select(c => c.Rating)
            .DefaultIfEmpty(0)
            .Average();

        public IEnumerable<Course> GetActiveCourses() =>
            CreatedCourses.Where(c => c.IsPublished && c.Status == CourseStatus.Active);

        public IEnumerable<Course> GetDraftCourses() =>
            CreatedCourses.Where(c => !c.IsPublished);
    }
}