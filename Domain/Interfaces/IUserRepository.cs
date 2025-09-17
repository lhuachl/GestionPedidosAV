using GestionPedidosAV.Domain.Entities;

namespace GestionPedidosAV.Domain.Interfaces;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<bool> EmailExistsAsync(string email);
    Task<IEnumerable<User>> GetUsersByRoleAsync(string role);
}