using System;
using VillaManager.Domain.DTOs.UsersDTO;
using Microsoft.AspNetCore.Identity;

public interface IUserService
{

        Task<IdentityResult> CreateUserAsync(AddUserDto addUserDto);
        Task<IdentityResult> EditUserAsync(string userId, EditUserDto editUserDto);
        Task<IdentityResult> DeleteUserAsync(string userId);
        Task<ShowUserDto> GetUserByIdAsync(string userId);
        Task<PagedResult<ShowUserDto>> GetAllUsersAsync(
                int? offset = 0,
                int? limit = 10,
                string? fullName = null, // Filter by full name
                string? userName = null, // Filter by username
                string? role = null,     // Filter by role
                bool? isActive = null);  // Filter by deletion status
}
