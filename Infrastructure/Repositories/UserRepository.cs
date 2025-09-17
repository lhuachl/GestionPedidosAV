using Microsoft.EntityFrameworkCore;
using GestionPedidosAV.Domain.Entities;
using GestionPedidosAV.Domain.Interfaces;
using GestionPedidosAV.Infrastructure.Data;

namespace GestionPedidosAV.Infrastructure.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _dbSet.AnyAsync(u => u.Email == email && !u.IsDeleted);
    }

    public async Task<IEnumerable<User>> GetUsersByRoleAsync(string role)
    {
        if (Enum.TryParse<Domain.Enums.UserRole>(role, out var userRole))
        {
            return await _dbSet.Where(u => u.Role == userRole && !u.IsDeleted).ToListAsync();
        }
        return new List<User>();
    }

    public override async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _dbSet.Where(u => !u.IsDeleted).ToListAsync();
    }

    public override async Task<User?> GetByIdAsync(int id)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
    }
}