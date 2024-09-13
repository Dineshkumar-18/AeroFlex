using AeroFlex.Data;
using AeroFlex.Repository.Contracts;
using Microsoft.EntityFrameworkCore;
using AeroFlex.Models;

namespace AeroFlex.Repository.Implementations
{
    public class ClassRepository : IClassRepository
    {
        private readonly ApplicationDbContext _context;

        public ClassRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Class>> GetAllClassesAsync()
        {
            return await _context.Classes.ToListAsync();
        }

        public async Task<Class> GetClassByNameAsync(string className)
        {
            var classNameLower = className.ToLower();
            var existingClass = await _context.Classes
                .Where(c => c.ClassName.ToLower() == classNameLower)
                .FirstOrDefaultAsync();
            return existingClass;
        }
    }

}
