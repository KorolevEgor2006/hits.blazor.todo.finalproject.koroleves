using System.ComponentModel.DataAnnotations;

namespace TheFinalProject.Data.Models
{
    public class CourseDto
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

        public string? TagsInput { get; set; }

        [Required(ErrorMessage = "Длительность обязательна")]
        [Range(1, 1000, ErrorMessage = "Длительность должна быть от 1 до 1000 часов")]
        public int TotalDurationHours { get; set; }

        [Required(ErrorMessage = "Цена обязательна")]
        [Range(0, 1000000, ErrorMessage = "Цена должна быть от 0 до 1,000,000")]
        public decimal Price { get; set; }

        [Url(ErrorMessage = "Неверный формат URL изображения")]
        public string? ImageUrl { get; set; }

        [Url(ErrorMessage = "Неверный формат URL видео")]
        public string? PreviewVideoUrl { get; set; }

        public bool IsPublished { get; set; } = false;
        public bool IsFeatured { get; set; } = false;

        public CourseLevel Level { get; set; } = CourseLevel.Beginner;
        public CourseStatus Status { get; set; } = CourseStatus.Draft;

        public string? InstructorId { get; set; }

        public Course ToCourse()
        {
            var course = new Course
            {
                Id = this.Id,
                Title = this.Title,
                Description = this.Description,
                ShortDescription = this.ShortDescription,
                Category = this.Category,
                TotalDurationHours = this.TotalDurationHours,
                Price = this.Price,
                ImageUrl = this.ImageUrl,
                PreviewVideoUrl = this.PreviewVideoUrl,
                IsPublished = this.IsPublished,
                IsFeatured = this.IsFeatured,
                Level = this.Level,
                Status = this.Status,
                InstructorId = this.InstructorId ?? string.Empty
            };
            if (!string.IsNullOrWhiteSpace(this.TagsInput))
            {
                course.Tags = this.TagsInput.Split(',')
                    .Select(t => t.Trim())
                    .Where(t => !string.IsNullOrEmpty(t))
                    .ToList();
            }

            return course;
        }
    }
}