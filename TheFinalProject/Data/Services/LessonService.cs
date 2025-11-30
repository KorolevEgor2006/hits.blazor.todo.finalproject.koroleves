using System;
using TheFinalProject.Components.Models;
using Microsoft.EntityFrameworkCore;

namespace TheFinalProject.Data.Services
{
    public class LessonService
    {
        private readonly EduDbContext _context;

        public LessonService(EduDbContext context) => _context = context;

        public async Task CreateLessonAsync(Lesson lesson)
        {
            _context.Lessons.Add(lesson);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateLessonAsync(Lesson lesson)
        {
            _context.Lessons.Update(lesson);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteLessonAsync(int id)
        {
            var lesson = await _context.Lessons.FindAsync(id);
            if (lesson != null)
            {
                _context.Lessons.Remove(lesson);
                await _context.SaveChangesAsync();
            }
        }
    }
}