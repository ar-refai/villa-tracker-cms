using AutoMapper;
using VillaManager.Data.EntityModel;
using VillaManager.Data.Interfaces;
using VillaManager.Domain.DTOs.AuthenticationDto;
using VillaManager.Domain.DTOs.UsersDTO;
using VillaManager.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
namespace VillaManager.Services.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<User> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;


    // constructor for the object
    public AuthenticationService(
        UserManager<User> userManager,
        IMapper mapper,
        IConfiguration configuration,
        IUnitOfWork unitOfWork
        )
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _configuration = configuration;
        _mapper = mapper;
    }

    // Login 
    public async Task<ReturnedLoginDto> AuthenticateAsync(LoginDto model)
    {

        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == model.Username);

        if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
        {
            // Generate JWT token
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials
            );

            var resultToken = new JwtSecurityTokenHandler().WriteToken(token);

            return new ReturnedLoginDto
            {
                Token = resultToken,
                UserInfo = _mapper.Map<ShowUserDto>(user)
            };
        }
        throw new UnauthorizedAccessException("Invalid username or password");
    }



    // // Registeration Logic
    // public async Task<ReturnedRegisterDto> RegisterUserAsync(RegisterDto registerDto)
    // {
    //     // Check if username or email already exists
    //     if (await _userManager.Users.AnyAsync(u => u.UserName == registerDto.Username || u.Email == registerDto.Email))
    //     {
    //         throw new InvalidOperationException("Username or email already exists.");
    //     }

    //     // Create Entity Object
    //     var user = new User
    //     {
    //         UserName = registerDto.Username,
    //         Name = registerDto.Name,
    //         PhoneNumber = registerDto.Phone,
    //         Email = registerDto.Email,
    //     };

    //     var registeredGroupDTO = new AddGroupDTO
    //     {
    //         Name = "Dummy",
    //         Description = "Dummy",
    //         CountryOfOriginId = 1,


    //     };

    //     // Add The Record
    //     var result = await _userManager.CreateAsync(user, registerDto.Password);

    //     // Add The Role
    //     if (result.Succeeded)
    //     {
    //         // Assign roles if necessary
    //         registerDto.Role = "CLIENT";
    //         await _userManager.AddToRoleAsync(user, registerDto.Role);
    //     }

    //     return new ReturnedRegisterDto
    //     {
    //         IdentityResult = result,
    //         UserInfo = _mapper.Map<ShowUserDto>(user)
    //     };
    // }


    public async Task<ReturnedRegisterDto> RegisterUserAsync(RegisterDto registerDto)
    {
        // Check if username or email already exists
        if (await _userManager.Users.AnyAsync(u => u.UserName == registerDto.Username || u.Email == registerDto.Email))
        {
            throw new InvalidOperationException("Username or email already exists.");
        }

        // Create User Entity
        var user = new User
        {
            UserName = registerDto.Username,
            Name = registerDto.Name,
            PhoneNumber = registerDto.Phone,
            Email = registerDto.Email,
        };

        // Start transaction
        bool success = await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            // Add The User
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                return false;
            }


            // Assign role
            registerDto.Role = "CLIENT";
            await _userManager.AddToRoleAsync(user, registerDto.Role);

            
            await _unitOfWork.SaveAsync();
            Console.WriteLine("UserClient record created successfully");

            return true;
        });

        if (!success)
        {
            throw new ApplicationException("Failed to register user.");
        }

        return new ReturnedRegisterDto
        {
            IdentityResult = IdentityResult.Success,
            UserInfo = _mapper.Map<ShowUserDto>(user)
        };
    }


}
