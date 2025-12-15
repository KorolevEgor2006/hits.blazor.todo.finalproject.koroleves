using System.ComponentModel.DataAnnotations;

namespace TheFinalProject.Data.Models
{
    public class Student : ApplicationUser
    {
        public DateTime? DateOfBirth { get; set; }

        [StringLength(100, ErrorMessage = "Уровень образования не должен превышать 100 символов")]
        public string? EducationLevel { get; set; }

        [StringLength(200, ErrorMessage = "Учебное заведение не должно превышать 200 символов")]
        public string? Institution { get; set; }

        public int TotalEnrollments => Enrollments.Count;
        public int CompletedCourses => Enrollments.Count(e => e.CompletionDate.HasValue);

        public decimal AverageGrade => Enrollments
            .Where(e => e.Grade.HasValue)
            .Select(e => e.Grade!.Value)
            .DefaultIfEmpty(0)
            .Average();

        public int TotalStudyHours => Enrollments
            .SelectMany(e => e.Course?.Elements.OfType<Lesson>() ?? new List<Lesson>())
            .Count(l => l.LessonProgresses.Any(lp => lp.LessonId == l.Id && lp.IsCompleted));

        public bool IsEnrolledInCourse(int courseId) =>
            Enrollments.Any(e => e.CourseId == courseId);

        public double GetCourseProgress(int courseId)
        {
            var enrollment = Enrollments.FirstOrDefault(e => e.CourseId == courseId);
            if (enrollment == null) return 0;

            return enrollment.ProgressPercentage;
        }
    }
}
