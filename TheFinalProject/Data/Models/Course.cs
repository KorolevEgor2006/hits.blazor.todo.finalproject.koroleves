using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace TheFinalProject.Data.Models
{
    public class Course
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Название курса обязательно")]
        [StringLength(200, ErrorMessage = "Название не должно превышать 200 символов")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Описание курса обязательно")]
        [StringLength(5000, ErrorMessage = "Описание не должно превышать 5000 символов")]
        public string Description { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Краткое описание не должно превышать 500 символов")]
        public string? ShortDescription { get; set; }

        [StringLength(100, ErrorMessage = "Категория не должна превышать 100 символов")]
        public string? Category { get; set; }

        public string? TagsJson { get; set; }

        [Range(1, 1000, ErrorMessage = "Длительность должна быть от 1 до 1000 часов")]
        public int TotalDurationHours { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }
        public DateTime? PublishedDate { get; set; }

        [Required(ErrorMessage = "Преподаватель обязателен")]
        public string InstructorId { get; set; } = string.Empty;
        public Instructor? Instructor { get; set; }

        [Url(ErrorMessage = "Неверный формат URL изображения")]
        public string? ImageUrl { get; set; }

        [Url(ErrorMessage = "Неверный формат URL видео")]
        public string? PreviewVideoUrl { get; set; }

        [Range(0, 1000000, ErrorMessage = "Цена должна быть от 0 до 1,000,000")]
        public decimal Price { get; set; }

        public bool IsPublished { get; set; } = false;
        public bool IsFeatured { get; set; } = false;

        [Range(0, 5, ErrorMessage = "Рейтинг должен быть от 0 до 5")]
        public double Rating { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Количество отзывов не может быть отрицательным")]
        public int ReviewCount { get; set; }

        public CourseLevel Level { get; set; } = CourseLevel.Beginner;
        public CourseStatus Status { get; set; } = CourseStatus.Draft;

        public virtual ICollection<CourseElement> Elements { get; set; } = new List<CourseElement>();
        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

        public List<string> Tags
        {
            get => string.IsNullOrEmpty(TagsJson)
                ? new List<string>()
                : JsonSerializer.Deserialize<List<string>>(TagsJson) ?? new List<string>();
            set => TagsJson = JsonSerializer.Serialize(value);
        }

        public int StudentCount => Enrollments.Count;
        public int LessonCount => Elements.OfType<Lesson>().Count();
        public int QuizCount => Elements.OfType<Quiz>().Count();

        public double ActualDurationHours =>
            Math.Round(Elements.Sum(e => e.GetEstimatedDuration()), 1);

        public double CompletionRate
        {
            get
            {
                if (StudentCount == 0) return 0;
                var completedStudents = Enrollments.Count(e => e.CompletionDate.HasValue);
                return (double)completedStudents / StudentCount * 100;
            }
        }

        public Course()
        {
            Elements = new List<CourseElement>();
            Enrollments = new List<Enrollment>();
            Reviews = new List<Review>();
            Tags = new List<string>();
        }
        public IEnumerable<Lesson> GetLessons() =>
            Elements.OfType<Lesson>().OrderBy(l => l.OrderNumber);

        public IEnumerable<Quiz> GetQuizzes() =>
            Elements.OfType<Quiz>().OrderBy(q => q.OrderNumber);

        public void Publish()
        {
            if (!IsPublished)
            {
                IsPublished = true;
                PublishedDate = DateTime.Now;
                Status = CourseStatus.Active;
            }
        }

        public void Unpublish()
        {
            if (IsPublished)
            {
                IsPublished = false;
                Status = CourseStatus.Draft;
            }
        }

        public bool CanEnroll(Student student)
        {
            if (!IsPublished) return false;
            if (Enrollments.Any(e => e.UserId == student.Id)) return false;
            return true;
        }
    }

    public enum CourseLevel
    {
        Beginner,
        Intermediate,
        Advanced
    }

    public enum CourseStatus
    {
        Draft,
        Review,
        Active,
        Archived
    }
}