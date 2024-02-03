using Dapper;
using Microsoft.AspNetCore.Mvc;
using SnatchItAPI.Data;
using SnatchItAPI.Models;
using System.Data;

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
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@EmailParam", banderForRegistration.Email, DbType.String);
                string sqlCheckBanderExists = "SELECT Email FROM MicroAgeSchema.Auth WHERE Email = @EmailParam ";

                IEnumerable<string> existingBanders = _dapper.LoadDataWithParameters<string>(sqlCheckBanderExists, parameters);

                if (existingBanders.Count() == 0)
                {
                    return Ok($"Thank you {banderForRegistration.Email}! You are now registered :)");
                }
                return StatusCode(409, "Bander with that email already exists.");
            }
            return StatusCode(400, "Failure to register new bander, passwords do not match");
        }

        [HttpPost("Login")]
        public IActionResult Login(BanderForLoginDto banderForLogin)
        {
            return Ok("You have Logged in");
        }
    }
}