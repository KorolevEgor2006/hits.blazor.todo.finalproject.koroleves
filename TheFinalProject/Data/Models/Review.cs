using System.ComponentModel.DataAnnotations;

namespace TheFinalProject.Data.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Рейтинг обязателен")]
        [Range(1, 5, ErrorMessage = "Рейтинг должен быть от 1 до 5")]
        public int Rating { get; set; }

        [StringLength(2000, ErrorMessage = "Комментарий не должен превышать 2000 символов")]
        public string? Comment { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }

        public int HelpfulCount { get; set; } = 0;

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

        public int CourseId { get; set; }
        public Course? Course { get; set; }

        public bool HasComment => !string.IsNullOrEmpty(Comment);

        public void MarkHelpful() => HelpfulCount++;
        public void UnmarkHelpful() => HelpfulCount = Math.Max(0, HelpfulCount - 1);
    }
}
