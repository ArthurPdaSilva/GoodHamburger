using Domain.Entities;
using Domain.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Domain.Repositories
{
    public class MenuItemRepository : IMenuItemRepository
    {
        private readonly ApplicationDbContext _context;

        public MenuItemRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IList<MenuItem>> GetAllAsync()
        {
            return await _context.MenuItems.OrderByDescending(x => x.CreatedAt).AsNoTracking().ToListAsync();
        }

        public async Task<MenuItem?> GetByIdAsync(Guid id)
        {
            return await _context.MenuItems
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}