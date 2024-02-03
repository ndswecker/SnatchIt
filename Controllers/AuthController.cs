using Dapper;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using SnatchItAPI.Data;
using SnatchItAPI.Models;
using System.Data;
using System.Security.Cryptography;
using System.Text;

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
                    byte[] passwordSalt = new byte[128 / 8];
                    using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                    {
                        rng.GetNonZeroBytes(passwordSalt);
                    }

                    string passwordSaltPlusString = _config.GetSection("AppSettings:PasswordKey").Value +
                        Convert.ToBase64String(passwordSalt); 

                    byte[] passwordHash = KeyDerivation.Pbkdf2(
                        password: banderForRegistration.Password,
                        salt: Encoding.ASCII.GetBytes(passwordSaltPlusString),
                        prf: KeyDerivationPrf.HMACSHA256,
                        iterationCount: 25600,
                        numBytesRequested: 256 / 8
                    );

                    DynamicParameters insertParameters = new DynamicParameters();
                    string insertBanderSql = @"INSERT INTO MicroAgeSchema.Auth 
                        ([Email], [PasswordHash], [PasswordSalt]) 
                        VALUES (
                            @EmailParam, @PasswordHashParam, @PasswordSaltParam
                        )";
                    insertParameters.Add("@EmailParam", banderForRegistration.Email, DbType.String);
                    insertParameters.Add("@PasswordHashParam", passwordHash, DbType.Binary);
                    insertParameters.Add("@PasswordSaltParam", passwordSalt, DbType.Binary);

                    if (_dapper.ExecuteSqlWithParameters(insertBanderSql, insertParameters))
                    {
                        return Ok($"Thank you {banderForRegistration.Email}! You are now registered :)");
                    }
                    return StatusCode(500, "Server failed to create new user.");
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

        // private bool SetPassword(BanderForRegistrationDto banderForRegistration)
        // {
        //     byte[] passwordSalt = new byte[128 / 8];
        //     using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        //     {
        //         rng.GetNonZeroBytes(passwordSalt);
        //     }

        //     byte[] passwordHash = GetPasswordHash(userForSetPassword.Password, passwordSalt);

        //     string sqlAddAuth = "EXEC TutorialAppSchema.spRegistration_Upsert " +
        //         "@Email = @EmailParam, @PasswordHash = @PasswordHashParam, @PasswordSalt = @PasswordSaltParam";
                
        //     DynamicParameters sqlParameters = new DynamicParameters();
            
        //     sqlParameters.Add("@EmailParam", userForSetPassword.Email, DbType.String);
        //     sqlParameters.Add("@PasswordHashParam", passwordHash, DbType.Binary);
        //     sqlParameters.Add("@PasswordSaltParam", passwordSalt, DbType.Binary);

        //     return _dapper.ExecuteSqlWithParameters(sqlAddAuth, sqlParameters);
        // }

        // public byte[] GetPasswordHash(string password, byte[] passwordSalt)
        // {
        //     string passwordSaltPlusString = _config.GetSection("AppSettings:PasswordKey").Value +
        //         Convert.ToBase64String(passwordSalt);

        //     return KeyDerivation.Pbkdf2(
        //         password: password,
        //         salt: Encoding.ASCII.GetBytes(passwordSaltPlusString),
        //         prf: KeyDerivationPrf.HMACSHA256,
        //         iterationCount: 1000000,
        //         numBytesRequested: 256 / 8
        //     );
        // }
    }
}