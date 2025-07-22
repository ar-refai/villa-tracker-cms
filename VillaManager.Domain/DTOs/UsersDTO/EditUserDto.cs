using System;

namespace VillaManager.Domain.DTOs.UsersDTO;

public class EditUserDto
{
    public string FullName { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public bool IsActive { get; set; }
}
