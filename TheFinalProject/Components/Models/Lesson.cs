using System.ComponentModel.DataAnnotations;

namespace TheFinalProject.Components.Models
{
    public class Lesson
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Название урока обязательно")]
        [StringLength(150, ErrorMessage = "Название не может быть длиннее 150 символов")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Содержание урока обязательно")]
        public string Content { get; set; } = string.Empty;

        [Required(ErrorMessage = "Порядковый номер обязателен")]
        [Range(1, 1000, ErrorMessage = "Порядковый номер должен быть от 1 до 1000")]
        public int Order { get; set; }
        
    }
}
