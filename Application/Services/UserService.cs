using AutoMapper;
using GestionPedidosAV.Application.DTOs;
using GestionPedidosAV.Application.Interfaces;
using GestionPedidosAV.Domain.Entities;
using GestionPedidosAV.Domain.Interfaces;

namespace GestionPedidosAV.Application.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidationService _validationService;

    public UserService(IUnitOfWork unitOfWork, IMapper mapper, IValidationService validationService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validationService = validationService;
    }

    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        return user != null ? _mapper.Map<UserDto>(user) : null;
    }

    public async Task<UserDto?> GetUserByEmailAsync(string email)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(email);
        return user != null ? _mapper.Map<UserDto>(user) : null;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await _unitOfWork.Users.GetAllAsync();
        return _mapper.Map<IEnumerable<UserDto>>(users);
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
    {
        var validation = await _validationService.ValidateAsync(createUserDto);
        if (!validation.IsValid)
        {
            throw new ArgumentException(string.Join(", ", validation.Errors));
        }

        if (await _unitOfWork.Users.EmailExistsAsync(createUserDto.Email))
        {
            throw new ArgumentException("El email ya está en uso");
        }

        var user = _mapper.Map<User>(createUserDto);
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password);
        user.CreatedAt = DateTime.UtcNow;

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> UpdateUserAsync(UpdateUserDto updateUserDto)
    {
        var existingUser = await _unitOfWork.Users.GetByIdAsync(updateUserDto.Id);
        if (existingUser == null)
        {
            throw new ArgumentException("Usuario no encontrado");
        }

        // Check if email is being changed and if it already exists
        if (existingUser.Email != updateUserDto.Email)
        {
            if (await _unitOfWork.Users.EmailExistsAsync(updateUserDto.Email))
            {
                throw new ArgumentException("El email ya está en uso");
            }
        }

        _mapper.Map(updateUserDto, existingUser);
        existingUser.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Users.UpdateAsync(existingUser);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<UserDto>(existingUser);
    }

    public async Task DeleteUserAsync(int id)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        if (user == null)
        {
            throw new ArgumentException("Usuario no encontrado");
        }

        user.IsDeleted = true;
        user.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<bool> ValidateUserCredentialsAsync(string email, string password)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(email);
        if (user == null || !user.IsActive)
        {
            return false;
        }

        return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _unitOfWork.Users.EmailExistsAsync(email);
    }
}