using System;
using VillaManager.Domain.DTOs.UsersDTO;

namespace VillaManager.Domain.DTOs.AuthenticationDto;

public class ReturnedLoginDto
{
    public string Token { get; set; }
    public ShowUserDto UserInfo {get; set;}
}
