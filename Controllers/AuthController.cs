using Microsoft.AspNetCore.Mvc;
using SnatchItAPI.Data;
using SnatchItAPI.Models;

namespace SnatchItAPI.Controllers
{
    public class AuthController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        private readonly IConfiguration _config;
        public AuthController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
            _config = config;
        }

        [HttpPost("Register")]
        public IActionResult Register(BanderForRegistrationDto banderForRegistration)
        {
            if (string.Equals(banderForRegistration.Password, banderForRegistration.PasswordConfirm) )
            {
                return Ok("You have registered");
            }
            return NotFound("Failure to register new bander");
        }

        [HttpPost("Login")]
        public IActionResult Login(BanderForLoginDto banderForLogin)
        {
            return Ok("You have Logged in");
        }
    }
}