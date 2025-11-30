using System;
using TheFinalProject.Components.Models;
using Microsoft.EntityFrameworkCore;

namespace TheFinalProject.Data.Services
{
    public class CourseService
    {
        private readonly EduDbContext _context;

        public CourseService(EduDbContext context) => _context = context;

        public async Task<List<Course>> GetAllCoursesAsync()
        {
            return await _context.Courses
                .Include(c => c.Author)
                .Include(c => c.Lessons)
                .ToListAsync();
        }

        public async Task<List<Course>> GetPublishedCoursesAsync()
        {
            return await _context.Courses
                .Where(c => c.IsPublished)
                .Include(c => c.Author)
                .Include(c => c.Lessons)
                .ToListAsync();
        }

        public async Task<List<Course>> GetCoursesByAuthorAsync(int authorId)
        {
            return await _context.Courses
                .Where(c => c.AuthorId == authorId)
                .Include(c => c.Author)
                .Include(c => c.Lessons)
                .ToListAsync();
        }

        public async Task<Course?> GetCourseByIdAsync(int id)
        {
            return await _context.Courses
                .Include(c => c.Author)
                .Include(c => c.Lessons)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task CreateCourseAsync(Course course)
        {
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCourseAsync(Course course)
        {
            _context.Courses.Update(course);
            await _context.SaveChangesAsync();
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

        public async Task SeedCoursesAsync()
        {
            if (!_context.Courses.Any())
            {
                var teacher = await _context.Users.FirstOrDefaultAsync(u => u.Role == "Teacher");

                if (teacher != null)
                {
                    _context.Courses.AddRange(
                        new Course
                        {
                            Title = "Основы C#",
                            Description = "Изучение основ языка программирования C#",
                            AuthorId = teacher.Id,
                            IsPublished = true,
                            Lessons = new List<Lesson>
                            {
                                new Lesson {
                                    Title = "Введение в C#",
                                    Content = "C# - это современный объектно-ориентированный язык программирования...",
                                    Order = 1
                                },
                                new Lesson {
                                    Title = "Переменные и типы данных",
                                    Content = "В C# есть различные типы данных: int, string, bool, double...",
                                    Order = 2
                                }
                            }
                        },
                        new Course
                        {
                            Title = "Web разработка с Blazor",
                            Description = "Создание веб-приложений с использованием Blazor",
                            AuthorId = teacher.Id,
                            IsPublished = true,
                            Lessons = new List<Lesson>
                            {
                                new Lesson {
                                    Title = "Что такое Blazor?",
                                    Content = "Blazor - это фреймворк для построения интерактивных веб-UI с помощью C#...",
                                    Order = 1
                                }
                            }
                        }
                    );
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}
