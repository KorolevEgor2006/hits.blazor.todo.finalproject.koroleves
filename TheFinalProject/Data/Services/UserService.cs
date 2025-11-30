using Microsoft.EntityFrameworkCore;
using System;
using TheFinalProject.Components.Models;

namespace TheFinalProject.Data.Services
{
    public class UserService
    {
        private readonly EduDbContext _context;

        public UserService(EduDbContext context) => _context = context;
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<List<User>> GetUsersByRoleAsync(string role)
        {
            return await _context.Users
                .Where(u => u.Role == role)
                .ToListAsync();
        }

        public async Task CreateUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        // Обновить пользователя
        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        // Удалить пользователя
        public async Task DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        // Заполнить тестовыми данными
        public async Task SeedUsersAsync()
        {
            if (!_context.Users.Any())
            {
                _context.Users.AddRange(
                    new User { Username = "admin", Email = "admin@platform.ru", PasswordHash = "admin123", Role = "Admin" },
                    new User { Username = "teacher1", Email = "teacher@platform.ru", PasswordHash = "teacher123", Role = "Teacher" },
                    new User { Username = "student1", Email = "student@platform.ru", PasswordHash = "student123", Role = "Student" }
                );
                await _context.SaveChangesAsync();
            }
        }
    }
}
