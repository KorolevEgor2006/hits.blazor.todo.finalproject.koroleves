using System.ComponentModel.DataAnnotations;

namespace TheFinalProject.Components.Models
{
    public class Course
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Название курса обязательно")]
        [StringLength(150, ErrorMessage = "Название не может быть длиннее 150 символов")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Описание обязательно")]
        [StringLength(1000, ErrorMessage = "Описание не может быть длиннее 1000 символов")]
        public string Description { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;

        [Required(ErrorMessage = "Автор обязателен")]
        public int AuthorId { get; set; }

        public User Author { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsPublished { get; set; }

        public List<Lesson> Lessons { get; set; } = new();
    }
}
