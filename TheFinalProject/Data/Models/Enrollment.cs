using System.ComponentModel.DataAnnotations;

namespace TheFinalProject.Data.Models
{
    public class Enrollment
    {
        public int Id { get; set; }

        public DateTime EnrollmentDate { get; set; } = DateTime.Now;
        public DateTime? StartDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public DateTime? LastActivityDate { get; set; }

        [Range(0, 100, ErrorMessage = "Оценка должна быть от 0 до 100")]
        public decimal? Grade { get; set; }

        public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Active;

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

        public int CourseId { get; set; }
        public Course? Course { get; set; }

        public virtual ICollection<QuizAttempt> QuizAttempts { get; set; } = new List<QuizAttempt>();
        public virtual ICollection<LessonProgress> LessonProgresses { get; set; } = new List<LessonProgress>();

        public bool IsCompleted => CompletionDate.HasValue;

        public TimeSpan? Duration => CompletionDate.HasValue
            ? CompletionDate.Value - (StartDate ?? EnrollmentDate)
            : null;

        public double ProgressPercentage
        {
            get
            {
                if (Course == null) return 0;
                var totalElements = Course.Elements.Count(e => e.CanBeCompleted());
                if (totalElements == 0) return 0;
                var completedElements = GetCompletedElements().Count;
                return (double)completedElements / totalElements * 100;
            }
        }
        public int GetQuizAttemptCount(int quizId)
        {
            return QuizAttempts?.Count(qa => qa.QuizId == quizId) ?? 0;
        }
        public void CompleteCourse(decimal? grade = null)
        {
            if (!IsCompleted)
            {
                CompletionDate = DateTime.Now;
                Status = EnrollmentStatus.Completed;
                Grade = grade;
                LastActivityDate = DateTime.Now;
            }
        }

        public List<CourseElement> GetCompletedElements()
        {
            var completedElements = new List<CourseElement>();

            var completedLessons = LessonProgresses
                .Where(lp => lp.IsCompleted && lp.Lesson != null)
                .Select(lp => lp.Lesson as CourseElement);
            completedElements.AddRange(completedLessons);

            var completedQuizzes = QuizAttempts
                .Where(qa => qa.IsCorrect && qa.Quiz != null)
                .Select(qa => qa.Quiz as CourseElement)
                .Distinct();
            completedElements.AddRange(completedQuizzes);

            return completedElements;
        }

        public bool IsLessonCompleted(int lessonId) =>
            LessonProgresses.Any(lp => lp.LessonId == lessonId && lp.IsCompleted);

        public bool IsQuizPassed(int quizId) =>
            QuizAttempts.Any(qa => qa.QuizId == quizId && qa.IsCorrect);
    }

    public enum EnrollmentStatus
    {
        Active,
        Completed,
        Dropped,
        Inactive
    }
}
