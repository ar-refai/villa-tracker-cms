using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VillaManager.Domain.DTOs.UsersDTO;
using Microsoft.AspNetCore.Identity;

namespace VillaManager.Domain.DTOs.AuthenticationDto
{
    public class ReturnedRegisterDto
    {
        public IdentityResult IdentityResult { get; set; }
        public ShowUserDto UserInfo { get; set; }
    }
}