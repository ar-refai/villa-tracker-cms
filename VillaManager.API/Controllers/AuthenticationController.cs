using AutoMapper;
using VillaManager.Domain.DTOs.AuthenticationDto;
using VillaManager.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace VillaManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IMapper _mapper;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthenticationController(IAuthenticationService authenticationService, RoleManager<IdentityRole> roleManager, IMapper mapper)
        {
            _authenticationService = authenticationService;
            _mapper = mapper;
            _roleManager = roleManager;
        }

        // POST api/users/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto register)
        {
            try
            {
                var result = await _authenticationService.RegisterUserAsync(register);
                if (result.IdentityResult.Succeeded)
                    return Ok(new { message = "User registered successfully!", Items = result });
                return BadRequest(result.IdentityResult.Errors);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message + " , Inner Exception: " + ex.InnerException });
            }
        }

        // POST api/users/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            try
            {
                var userInfo = await _authenticationService.AuthenticateAsync(model);
                return Ok(new
                {
                    Token = userInfo.Token,
                    UserInfo = userInfo.UserInfo,
                    Expiration = DateTime.Now.AddHours(8)
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }
    }
}
