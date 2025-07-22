using VillaManager.Data.EntityModel;
using VillaManager.Domain.DTOs.UsersDTO;
using VillaManager.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Runtime.InteropServices;
using VillaManager.Data.Interfaces;

namespace VillaManager.Services.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;
        // Constructor
        public UserService(UserManager<User> userManager, IMapper mapper, IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
        }

        // All Users Service 
        public async Task<PagedResult<ShowUserDto>> GetAllUsersAsync(
            int? offset = 0,
            int? limit = 10,
            string? fullName = null, // Filter by full name
            string? userName = null, // Filter by username
            string? role = null,     // Filter by role
            bool? isActive = null)  // Filter by deletion status
        {
            offset ??= 0;
            limit ??= 10;
            if (limit <= 0) limit = 10;

            // Base query with filtering
            var query = _userManager.Users.Where(u =>
                u.IsDeleted != true &&
                (isActive == null || u.isActive == isActive) &&
                (string.IsNullOrEmpty(fullName) || u.Name.Contains(fullName)) &&
                (string.IsNullOrEmpty(userName) || u.UserName.Contains(userName))
            );

            // Role filtering
            if (!string.IsNullOrEmpty(role))
            {
                var usersInRole = await _userManager.GetUsersInRoleAsync(role);
                var roleUserIds = usersInRole.Select(r => r.Id).ToList();
                query = query.Where(u => roleUserIds.Contains(u.Id));
            }


            // Total count before pagination
            int totalCount = await query.CountAsync();

            // Apply pagination
            var pagedUsers = await query
                .Skip(offset.Value)
                .Take(limit.Value)
                .ToListAsync();

            // Map users to DTOs
            var userDtos = _mapper.Map<List<ShowUserDto>>(pagedUsers);

            // roles for each user
            for (int i = 0; i < pagedUsers.Count; i++)
            {
                var roles = await _userManager.GetRolesAsync(pagedUsers[i]);
                userDtos[i].Role = roles.FirstOrDefault() ?? ""; // Assign role or default to an empty string
            }

            // Return the paged result
            return new PagedResult<ShowUserDto>
            {
                Items = userDtos,
                TotalCount = totalCount,
                PageNumber = offset.Value / limit.Value + 1, // Calculate the page number
                PageSize = limit.Value,
                TotalPages = (int)Math.Ceiling((double)totalCount / limit.Value)
            };
        }



        // Show User Service
        public async Task<ShowUserDto> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return null;

            var userDto = _mapper.Map<ShowUserDto>(user);

            // Get User Roles
            var roles = await _userManager.GetRolesAsync(user);
            userDto.Role = roles.FirstOrDefault() ?? "";

            // Map User Permissions

            return userDto;
        }


        // Create User Service
        public async Task<IdentityResult> CreateUserAsync(AddUserDto addUserDto)
        {
            var user = _mapper.Map<User>(addUserDto);

           

            var result = await _userManager.CreateAsync(user, addUserDto.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, addUserDto.Role);
            }

            return result;
        }


        // Edit User Service 
        public async Task<IdentityResult> EditUserAsync(string userId, EditUserDto editUserDto)
        {
            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });

            try
            {
                // **Update Basic User Info**
                user.Name = editUserDto.FullName;
                user.PhoneNumber = editUserDto.PhoneNumber;
                user.Email = editUserDto.Email;
                user.isActive = editUserDto.IsActive;

                // **Update User Roles**
                if (!string.IsNullOrEmpty(editUserDto.Role))
                {
                    var currentRoles = await _userManager.GetRolesAsync(user);
                    var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                    if (!removeResult.Succeeded)
                        return IdentityResult.Failed(new IdentityError { Description = "Failed to remove current roles" });

                    var addRoleResult = await _userManager.AddToRoleAsync(user, editUserDto.Role);
                    if (!addRoleResult.Succeeded)
                        return IdentityResult.Failed(new IdentityError { Description = "Failed to add new role" });
                }

                // **Save Changes**
                var result = await _userManager.UpdateAsync(user);
                return result.Succeeded ? result : IdentityResult.Failed(new IdentityError { Description = "Failed to update user" });
            }
            catch (DbUpdateConcurrencyException)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Concurrency conflict occurred. Please try again." });
            }
        }

        // Delete Service 
        public async Task<IdentityResult> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return IdentityResult.Failed(new IdentityError { Description = "User not found" });
            user.IsDeleted = true;
            var result = await _userManager.UpdateAsync(user);
            return result;
        }

    }
}
