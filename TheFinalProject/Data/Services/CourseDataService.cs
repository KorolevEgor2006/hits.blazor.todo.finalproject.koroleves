using Microsoft.EntityFrameworkCore;
using TheFinalProject.Data.Interfaces;
using TheFinalProject.Data.Models;

namespace TheFinalProject.Data.Services
{
    public class CourseDataService : ICourseService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CourseDataService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<Course>> GetAllCoursesAsync()
        {
            return await _context.Courses
                .Include(c => c.Instructor)
                .Include(c => c.Elements)
                .OrderByDescending(c => c.CreatedDate)
                .ToListAsync();
        }


        public async Task<IEnumerable<Course>> GetPublishedCoursesAsync()
        {
            try
            {
                var courses = await _context.Courses
                    .Where(c => c.IsPublished)
                    .Include(c => c.Instructor)
                    .Include(c => c.Elements)
                    .AsNoTracking() 
                    .OrderByDescending(c => c.CreatedDate)
                    .ToListAsync();

                Console.WriteLine($"Found {courses.Count} published courses");

                foreach (var course in courses)
                {
                    Console.WriteLine($"- Course: {course.Title}, Published: {course.IsPublished}, Elements: {course.Elements?.Count ?? 0}");
                }

                return courses;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetPublishedCoursesAsync: {ex.Message}");
                return new List<Course>();
            }
        }

        public async Task<Course?> GetCourseByIdAsync(int id)
        {
            return await _context.Courses
                .Include(c => c.Instructor)
                .Include(c => c.Elements)
                .Include(c => c.Reviews)
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Course?> GetCourseWithElementsAsync(int id)
        {
            return await _context.Courses
                .Include(c => c.Instructor)
                .Include(c => c.Elements)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task SaveCourseAsync(Course course)
        {
            try
            {
                if (course.Id == 0)
                {
                    course.CreatedDate = DateTime.Now;

                    if (course.IsPublished && !course.PublishedDate.HasValue)
                    {
                        course.PublishedDate = DateTime.Now;
                    }

                    await _context.Courses.AddAsync(course);
                }
                else
                {
                    course.UpdatedDate = DateTime.Now;

                    if (course.IsPublished && !course.PublishedDate.HasValue)
                    {
                        course.PublishedDate = DateTime.Now;
                    }

                    var existingCourse = await _context.Courses
                        .FirstOrDefaultAsync(c => c.Id == course.Id);

                    if (existingCourse != null)
                    {
                        _context.Entry(existingCourse).CurrentValues.SetValues(course);

                        existingCourse.TagsJson = course.TagsJson;
                    }
                    else
                    {
                        throw new InvalidOperationException($"Course with id {course.Id} not found.");
                    }
                }

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Database update error: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                throw;
            }
        }

        public async Task DeleteCourseAsync(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<CourseElement>> GetCourseElementsAsync(int courseId)
        {
            return await _context.CourseElements
                .Where(e => e.CourseId == courseId)
                .OrderBy(e => e.OrderNumber)
                .ToListAsync();
        }

        public async Task<IEnumerable<Lesson>> GetCourseLessonsAsync(int courseId)
        {
            return await _context.Lessons
                .Where(l => l.CourseId == courseId)
                .OrderBy(l => l.OrderNumber)
                .ToListAsync();
        }

        public async Task<IEnumerable<Quiz>> GetCourseQuizzesAsync(int courseId)
        {
            return await _context.Quizzes
                .Where(q => q.CourseId == courseId)
                .OrderBy(q => q.OrderNumber)
                .ToListAsync();
        }

        public async Task<CourseElement?> GetElementByIdAsync(int id)
        {
            return await _context.CourseElements.FindAsync(id);
        }

        public async Task<Lesson?> GetLessonByIdAsync(int id)
        {
            return await _context.Lessons
                .Include(l => l.Quizzes)
                .Include(l => l.Course)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<Quiz?> GetQuizByIdAsync(int id)
        {
            return await _context.Quizzes
                .Include(q => q.Lesson)
                .FirstOrDefaultAsync(q => q.Id == id);
        }

        public async Task SaveElementAsync(CourseElement element)
        {
            if (element.Id == 0)
            {
                element.CreatedDate = DateTime.Now;
                await _context.CourseElements.AddAsync(element);
            }
            else
            {
                element.UpdatedDate = DateTime.Now;
                _context.CourseElements.Update(element);
            }
            await _context.SaveChangesAsync();
        }

        public async Task SaveLessonAsync(Lesson lesson)
        {
            await SaveElementAsync(lesson);
        }

        public async Task SaveQuizAsync(Quiz quiz)
        {
            await SaveElementAsync(quiz);
        }

        public async Task DeleteElementAsync(int id)
        {
            var element = await _context.CourseElements.FindAsync(id);
            if (element != null)
            {
                _context.CourseElements.Remove(element);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<Enrollment?> EnrollUserInCourseAsync(string userId, int courseId)
        {
            var existingEnrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId);

            if (existingEnrollment != null)
                return null;

            var enrollment = new Enrollment
            {
                UserId = userId,
                CourseId = courseId,
                EnrollmentDate = DateTime.Now,
                StartDate = DateTime.Now,
                Status = EnrollmentStatus.Active
            };

            await _context.Enrollments.AddAsync(enrollment);
            await _context.SaveChangesAsync();

            return enrollment;
        }

        public async Task<bool> IsUserEnrolledAsync(string userId, int courseId)
        {
            return await _context.Enrollments
                .AnyAsync(e => e.UserId == userId && e.CourseId == courseId);
        }

        public async Task<IEnumerable<Course>> GetUserCoursesAsync(string userId)
        {
            return await _context.Courses
                .Where(c => c.Enrollments.Any(e => e.UserId == userId))
                .Include(c => c.Enrollments.Where(e => e.UserId == userId))
                .Include(c => c.Elements)
                .ToListAsync();
        }

        public async Task<Enrollment?> GetUserEnrollmentAsync(string userId, int courseId)
        {
            return await _context.Enrollments
                .Include(e => e.LessonProgresses)
                .Include(e => e.QuizAttempts)
                .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId);
        }

        public async Task UpdateLessonProgressAsync(string userId, int lessonId, bool isCompleted)
        {
            var lesson = await _context.Lessons
                .Include(l => l.Course)
                .FirstOrDefaultAsync(l => l.Id == lessonId);

            if (lesson == null || lesson.Course == null)
                return;

            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == lesson.Course.Id);

            if (enrollment == null)
                return;

            var progress = await _context.LessonProgresses
                .FirstOrDefaultAsync(lp => lp.EnrollmentId == enrollment.Id && lp.LessonId == lessonId);

            if (progress == null)
            {
                progress = new LessonProgress
                {
                    EnrollmentId = enrollment.Id,
                    LessonId = lessonId,
                    StartedAt = DateTime.Now,
                    IsCompleted = isCompleted,
                    CompletedAt = isCompleted ? DateTime.Now : null
                };
                await _context.LessonProgresses.AddAsync(progress);
            }
            else
            {
                progress.IsCompleted = isCompleted;
                progress.CompletedAt = isCompleted ? DateTime.Now : null;
                _context.LessonProgresses.Update(progress);
            }

            enrollment.LastActivityDate = DateTime.Now;
            _context.Enrollments.Update(enrollment);

            await _context.SaveChangesAsync();
        }

        public async Task<QuizAttempt?> SubmitQuizAnswerAsync(string userId, int quizId, int selectedAnswerIndex)
        {
            var quiz = await GetQuizByIdAsync(quizId);
            if (quiz == null || quiz.Lesson?.Course == null)
                return null;

            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == quiz.Lesson.Course.Id);

            if (enrollment == null)
                return null;

            var isCorrect = quiz.ValidateAnswer(selectedAnswerIndex);
            var pointsEarned = isCorrect ? quiz.Points : 0;

            var attempt = new QuizAttempt
            {
                EnrollmentId = enrollment.Id,
                QuizId = quizId,
                SelectedAnswerIndex = selectedAnswerIndex,
                IsCorrect = isCorrect,
                PointsEarned = pointsEarned,
                AttemptDate = DateTime.Now
            };

            await _context.QuizAttempts.AddAsync(attempt);

            enrollment.LastActivityDate = DateTime.Now;
            _context.Enrollments.Update(enrollment);

            await _context.SaveChangesAsync();

            return attempt;
        }

        public async Task<double> GetCourseProgressAsync(string userId, int courseId)
        {
            var enrollment = await GetUserEnrollmentAsync(userId, courseId);
            if (enrollment == null)
                return 0;

            return enrollment.ProgressPercentage;
        }
        public async Task<Student?> GetStudentByIdAsync(string userId)
        {
            return await _context.Students
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Course)
                .FirstOrDefaultAsync(s => s.Id == userId);
        }

        public async Task<Instructor?> GetInstructorByIdAsync(string userId)
        {
            return await _context.Instructors
                .Include(i => i.CreatedCourses)
                .FirstOrDefaultAsync(i => i.Id == userId);
        }

        public async Task<IEnumerable<Student>> GetCourseStudentsAsync(int courseId)
        {
            return await _context.Students
                .Where(s => s.Enrollments.Any(e => e.CourseId == courseId))
                .Include(s => s.Enrollments.Where(e => e.CourseId == courseId))
                .ToListAsync();
        }
        public async Task<Review?> AddReviewAsync(string userId, int courseId, int rating, string? comment)
        {
            var existingReview = await _context.Reviews
                .FirstOrDefaultAsync(r => r.UserId == userId && r.CourseId == courseId);

            if (existingReview != null)
                return null;

            var review = new Review
            {
                UserId = userId,
                CourseId = courseId,
                Rating = rating,
                Comment = comment,
                CreatedDate = DateTime.Now
            };

            await _context.Reviews.AddAsync(review);

            var course = await _context.Courses.FindAsync(courseId);
            if (course != null)
            {
                var reviews = await _context.Reviews
                    .Where(r => r.CourseId == courseId)
                    .ToListAsync();

                course.ReviewCount = reviews.Count;
                course.Rating = reviews.Average(r => r.Rating);
                _context.Courses.Update(course);
            }

            await _context.SaveChangesAsync();

            return review;
        }

        public async Task<IEnumerable<Review>> GetCourseReviewsAsync(int courseId)
        {
            return await _context.Reviews
                .Where(r => r.CourseId == courseId)
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedDate)
                .ToListAsync();
        }
    }
}
