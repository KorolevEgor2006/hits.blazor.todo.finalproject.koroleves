using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace TheFinalProject.Data.Models
{
    public class Quiz : CourseElement
    {
        [Required(ErrorMessage = "Вопрос обязателен")]
        public string Question { get; set; } = string.Empty;

        [Required(ErrorMessage = "Варианты ответов обязательны")]
        public string OptionsJson { get; set; } = string.Empty;

        [Range(0, 100, ErrorMessage = "Индекс правильного ответа должен быть от 0 до 100")]
        public int CorrectAnswerIndex { get; set; }

        [Range(1, 100, ErrorMessage = "Баллы должны быть от 1 до 100")]
        public int Points { get; set; } = 10;

        public string? Explanation { get; set; }

        [Range(1, 180, ErrorMessage = "Лимит времени должен быть от 1 до 180 минут")]
        public int TimeLimitMinutes { get; set; } = 10;

        public bool AllowMultipleAttempts { get; set; } = true;

        [Range(1, 10, ErrorMessage = "Максимальное количество попыток должно быть от 1 до 10")]
        public int MaxAttempts { get; set; } = 3;

        public QuizType Type { get; set; } = QuizType.SingleChoice;
        public DifficultyLevel Difficulty { get; set; } = DifficultyLevel.Medium;

        public int? LessonId { get; set; }
        public Lesson? Lesson { get; set; }

        public virtual ICollection<QuizAttempt> QuizAttempts { get; set; } = new List<QuizAttempt>();

        public List<string> Options
        {
            get
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(OptionsJson))
                        return new List<string> { "Вариант 1", "Вариант 2", "Вариант 3", "Вариант 4" };

                    return JsonSerializer.Deserialize<List<string>>(OptionsJson) ??
                           new List<string> { "Вариант 1", "Вариант 2", "Вариант 3", "Вариант 4" };
                }
                catch
                {
                    return new List<string> { "Вариант 1", "Вариант 2", "Вариант 3", "Вариант 4" };
                }
            }
            set => OptionsJson = JsonSerializer.Serialize(value);
        }

        public int AttemptCount => QuizAttempts.Count;

        public double SuccessRate
        {
            get
            {
                if (AttemptCount == 0) return 0;
                return (double)QuizAttempts.Count(qa => qa.IsCorrect) / AttemptCount * 100;
            }
        }

        public override double GetEstimatedDuration() => TimeLimitMinutes / 60.0;

        public override bool CanBeCompleted() => true;

        public override string GetElementType() => "Тест";

        public bool ValidateAnswer(int selectedIndex) => selectedIndex == CorrectAnswerIndex;

        public int GetUserAttemptCount(string userId) =>
            QuizAttempts.Count(qa => qa.Enrollment?.UserId == userId);

        public bool CanUserAttempt(string userId)
        {
            if (AllowMultipleAttempts) return true;
            return GetUserAttemptCount(userId) < MaxAttempts;
        }

    }

    public enum QuizType
    {
        SingleChoice,
        MultipleChoice,
        TrueFalse,
        Matching,
        FillInBlank
    }

    public enum DifficultyLevel
    {
        Easy,
        Medium,
        Hard
    }

    public class QuizAttempt
    {
        public int Id { get; set; }

        public DateTime AttemptDate { get; set; } = DateTime.Now;

        [Range(0, 100, ErrorMessage = "Выбранный ответ должен быть от 0 до 100")]
        public int SelectedAnswerIndex { get; set; }

        public bool IsCorrect { get; set; }

        [Range(0, 100, ErrorMessage = "Заработанные баллы должны быть от 0 до 100")]
        public int PointsEarned { get; set; }

        public TimeSpan TimeSpent { get; set; }

        public int EnrollmentId { get; set; }
        public Enrollment? Enrollment { get; set; }

        public int QuizId { get; set; }
        public Quiz? Quiz { get; set; }
    }
}