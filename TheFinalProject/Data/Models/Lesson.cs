using System.ComponentModel.DataAnnotations;

namespace TheFinalProject.Data.Models
{
    public class Lesson : CourseElement
    {
        [Required(ErrorMessage = "Содержание урока обязательно")]
        public string Content { get; set; } = string.Empty;

        [Url(ErrorMessage = "Неверный формат URL видео")]
        public string? VideoUrl { get; set; }

        [Url(ErrorMessage = "Неверный формат URL презентации")]
        public string? PresentationUrl { get; set; }

        [Url(ErrorMessage = "Неверный формат URL материалов")]
        public string? AdditionalMaterialsUrl { get; set; }

        public bool HasVideo => !string.IsNullOrEmpty(VideoUrl);
        public bool HasPresentation => !string.IsNullOrEmpty(PresentationUrl);
        public bool HasAdditionalMaterials => !string.IsNullOrEmpty(AdditionalMaterialsUrl);

        public virtual ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
        public virtual ICollection<LessonProgress> LessonProgresses { get; set; } = new List<LessonProgress>();

        public override double GetEstimatedDuration()
        {
            double duration = 0.5; 

            if (HasVideo) duration += 1.0;
            if (HasPresentation) duration += 0.5;
            if (Quizzes.Any()) duration += 0.25;

            int textLength = Content?.Length ?? 0;
            duration += textLength / 2000.0;

            return Math.Round(duration, 1);
        }

        public override bool CanBeCompleted() => true;

        public override string GetElementType() => "Урок";

        public int GetQuizCount() => Quizzes.Count;
    }
    public class LessonProgress
    {
        public int Id { get; set; }

        public DateTime StartedAt { get; set; } = DateTime.Now;
        public DateTime? CompletedAt { get; set; }
        public bool IsCompleted { get; set; } = false;

        public int EnrollmentId { get; set; }
        public Enrollment? Enrollment { get; set; }

        public int LessonId { get; set; }
        public Lesson? Lesson { get; set; }

        public TimeSpan? TimeSpent => IsCompleted && CompletedAt.HasValue
            ? CompletedAt.Value - StartedAt
            : null;
        
    }
}