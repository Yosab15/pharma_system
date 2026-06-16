using DAL.Database;
using DAL.Entities;
using DAL.Repo.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repo.Impelementation
{
    public class CategoryRepo : ICategoryRepo
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetAllAsync()
        => await _context.Categories.ToListAsync();

        public async Task<Category?> GetByIdAsync(int id)
         => await _context.Categories
        .Include(c => c.Products)
        .FirstOrDefaultAsync(c => c.Id == id);
        public async Task AddAsync(Category category)
            => await _context.Categories.AddAsync(category);

        public void Delete(Category category)
            => _context.Categories.Remove(category);

        public async Task SaveChangesAsync()
            => await _context.SaveChangesAsync();
    }
}
