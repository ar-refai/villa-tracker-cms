using System;
using VillaManager.Domain.DTOs.AuthenticationDto;
using Microsoft.AspNetCore.Identity;

namespace VillaManager.Services.Interfaces;

public interface IAuthenticationService
{
    Task<ReturnedLoginDto> AuthenticateAsync(LoginDto model);
    Task<ReturnedRegisterDto> RegisterUserAsync(RegisterDto registerDto);

}
