using GestionPedidosAV.Application.DTOs;

namespace GestionPedidosAV.Application.Interfaces;

public interface IUserService
{
    Task<UserDto?> GetUserByIdAsync(int id);
    Task<UserDto?> GetUserByEmailAsync(string email);
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<UserDto> CreateUserAsync(CreateUserDto createUserDto);
    Task<UserDto> UpdateUserAsync(UpdateUserDto updateUserDto);
    Task DeleteUserAsync(int id);
    Task<bool> ValidateUserCredentialsAsync(string email, string password);
    Task<bool> EmailExistsAsync(string email);
}