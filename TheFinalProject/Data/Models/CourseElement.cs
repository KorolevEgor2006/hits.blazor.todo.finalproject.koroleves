using System.ComponentModel.DataAnnotations;

namespace TheFinalProject.Data.Models
{
    public abstract class CourseElement
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Название обязательно")]
        [StringLength(200, ErrorMessage = "Название не должно превышать 200 символов")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Описание обязательно")]
        [StringLength(2000, ErrorMessage = "Описание не должно превышать 2000 символов")]
        public string Description { get; set; } = string.Empty;

        [Range(1, 1000, ErrorMessage = "Порядковый номер должен быть от 1 до 1000")]
        public int OrderNumber { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }

        public int CourseId { get; set; }
        public Course? Course { get; set; }

        public abstract double GetEstimatedDuration();
        public abstract bool CanBeCompleted();
        public abstract string GetElementType();
    }
}
