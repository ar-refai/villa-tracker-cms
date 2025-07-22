using System;

namespace VillaManager.Domain.DTOs.AuthenticationDto
{
    public class RegisterDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; } = "CLIENT";

        // Computed property to return full name
        public string Name => $"{FirstName} {LastName}".Trim();
    }
}
