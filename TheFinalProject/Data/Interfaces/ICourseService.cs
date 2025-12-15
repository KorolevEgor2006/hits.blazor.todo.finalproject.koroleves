using TheFinalProject.Data.Models;

namespace TheFinalProject.Data.Interfaces
{
    public interface ICourseService
    {
        Task<IEnumerable<Course>> GetAllCoursesAsync();
        Task<IEnumerable<Course>> GetPublishedCoursesAsync();
        Task<Course?> GetCourseByIdAsync(int id);
        Task<Course?> GetCourseWithElementsAsync(int id);
        Task SaveCourseAsync(Course course);
        Task DeleteCourseAsync(int id);

        Task<IEnumerable<CourseElement>> GetCourseElementsAsync(int courseId);
        Task<IEnumerable<Lesson>> GetCourseLessonsAsync(int courseId);
        Task<IEnumerable<Quiz>> GetCourseQuizzesAsync(int courseId);

        Task<CourseElement?> GetElementByIdAsync(int id);
        Task<Lesson?> GetLessonByIdAsync(int id);
        Task<Quiz?> GetQuizByIdAsync(int id);

        Task SaveElementAsync(CourseElement element);
        Task SaveLessonAsync(Lesson lesson);
        Task SaveQuizAsync(Quiz quiz);

        Task DeleteElementAsync(int id);

        Task<Enrollment?> EnrollUserInCourseAsync(string userId, int courseId);
        Task<bool> IsUserEnrolledAsync(string userId, int courseId);
        Task<IEnumerable<Course>> GetUserCoursesAsync(string userId);
        Task<Enrollment?> GetUserEnrollmentAsync(string userId, int courseId);

        Task UpdateLessonProgressAsync(string userId, int lessonId, bool isCompleted);
        Task<QuizAttempt?> SubmitQuizAnswerAsync(string userId, int quizId, int selectedAnswerIndex);

        Task<double> GetCourseProgressAsync(string userId, int courseId);

        Task<Student?> GetStudentByIdAsync(string userId);
        Task<Instructor?> GetInstructorByIdAsync(string userId);
        Task<IEnumerable<Student>> GetCourseStudentsAsync(int courseId);

        Task<Review?> AddReviewAsync(string userId, int courseId, int rating, string? comment);
        Task<IEnumerable<Review>> GetCourseReviewsAsync(int courseId);
    }
}