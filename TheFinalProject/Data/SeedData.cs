using Microsoft.AspNetCore.Identity;
using TheFinalProject.Data.Models;
using System.Text.Json;

namespace TheFinalProject.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            await context.Database.EnsureCreatedAsync();

            string[] roles = { "Admin", "Instructor", "Student" };
            foreach (var role in roles)
            {
                if (await roleManager.RoleExistsAsync(role))
                {
                    continue;
                }
                await roleManager.CreateAsync(new IdentityRole(role));
            }

            var adminEmail = "admin@finalproject.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var admin = new Administrator
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Администратор",
                    LastName = "Системы",
                    EmailConfirmed = true,
                    Department = "IT",
                    EmployeeId = "ADM001",
                    IsSuperAdmin = true
                };

                var result = await userManager.CreateAsync(admin, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRolesAsync(admin, new[] { "Admin", "Instructor" });
                }
            }

            var instructorEmail = "instructor@finalproject.com";
            var instructorUser = await userManager.FindByEmailAsync(instructorEmail);

            if (instructorUser == null)
            {
                var instructor = new Instructor
                {
                    UserName = instructorEmail,
                    Email = instructorEmail,
                    FirstName = "Иван",
                    LastName = "Петров",
                    EmailConfirmed = true,
                    Specialization = "Программирование, Веб-разработка",
                    Qualifications = "PhD in Computer Science",
                    YearsOfExperience = 10
                };

                var result = await userManager.CreateAsync(instructor, "Instructor123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(instructor, "Instructor");

                    await CreateSampleCourses(context, instructor.Id);
                }
            }

            var studentEmail = "student@finalproject.com";
            var studentUser = await userManager.FindByEmailAsync(studentEmail);

            if (studentUser == null)
            {
                var student = new Student
                {
                    UserName = studentEmail,
                    Email = studentEmail,
                    FirstName = "Анна",
                    LastName = "Сидорова",
                    EmailConfirmed = true,
                    DateOfBirth = new DateTime(1995, 5, 15),
                    EducationLevel = "Высшее",
                    Institution = "Технический университет"
                };

                var result = await userManager.CreateAsync(student, "Student123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(student, "Student");
                }
            }
        }

        private static async Task CreateSampleCourses(ApplicationDbContext context, string instructorId)
        {
            if (!context.Courses.Any())
            {
                // Course 1
                var course1 = new Course
                {
                    Title = "Основы программирования на C#",
                    Description = "Полный курс по основам программирования на языке C#. Идеально для начинающих.",
                    ShortDescription = "Научитесь программировать на C# с нуля",
                    Category = "Программирование",
                    TagsJson = JsonSerializer.Serialize(new[] { "C#", "Начинающий", ".NET" }),
                    TotalDurationHours = 40,
                    InstructorId = instructorId,
                    Price = 0,
                    IsPublished = true,
                    IsFeatured = true,
                    Level = CourseLevel.Beginner,
                    Status = CourseStatus.Active,
                    Rating = 4.8,
                    ReviewCount = 42,
                    ImageUrl = "/images/csharp-course.jpg"
                };

                await context.Courses.AddAsync(course1);
                await context.SaveChangesAsync();

                
                var lesson1 = new Lesson
                {
                    Title = "Введение в C# и .NET",
                    Description = "Основные концепции языка C# и платформы .NET",
                    Content = "# Введение в C#\n\nC# (произносится как \"си шарп\") — современный объектно-ориентированный язык программирования, разработанный Microsoft.",
                    OrderNumber = 1,
                    CourseId = course1.Id,
                    VideoUrl = "https://www.youtube.com/embed/GhQdlIFylQ8"
                };

                var lesson2 = new Lesson
                {
                    Title = "Переменные и типы данных",
                    Description = "Работа с переменными и типами данных в C#",
                    Content = "# Переменные и типы данных\n\nПеременная — это именованная область памяти, которая хранит данные определенного типа.",
                    OrderNumber = 2,
                    CourseId = course1.Id,
                    PresentationUrl = "/materials/variables.pptx"
                };

                await context.Lessons.AddRangeAsync(lesson1, lesson2);
                await context.SaveChangesAsync();

                var quiz1 = new Quiz
                {
                    Title = "Тест по основам C#",
                    Description = "Проверьте свои знания основ C#",
                    Question = "Что такое C#?",
                    OptionsJson = JsonSerializer.Serialize(new List<string>
                    {
                        "Язык программирования от Microsoft",
                        "База данных",
                        "Фреймворк для веб-разработки",
                        "Операционная система"
                    }),
                    CorrectAnswerIndex = 0,
                    Points = 10,
                    Explanation = "C# — это объектно-ориентированный язык программирования, разработанный Microsoft.",
                    TimeLimitMinutes = 5,
                    OrderNumber = 1,
                    CourseId = course1.Id,
                    LessonId = lesson1.Id,
                    Type = QuizType.SingleChoice,
                    Difficulty = DifficultyLevel.Easy
                };

                await context.Quizzes.AddAsync(quiz1);

                var course2 = new Course
                {
                    Title = "Продвинутый C# и паттерны проектирования",
                    Description = "Углубленное изучение C# и современных паттернов проектирования.",
                    ShortDescription = "Станьте экспертом в C#",
                    Category = "Программирование",
                    TagsJson = JsonSerializer.Serialize(new[] { "C#", "Продвинутый", "Паттерны" }),
                    TotalDurationHours = 60,
                    InstructorId = instructorId,
                    Price = 2999,
                    IsPublished = true,
                    Level = CourseLevel.Advanced,
                    Status = CourseStatus.Active,
                    Rating = 4.9,
                    ReviewCount = 28,
                    ImageUrl = "/images/advanced-csharp.jpg"
                };

                await context.Courses.AddAsync(course2);

                await context.SaveChangesAsync();
            }
        }
    }
}
