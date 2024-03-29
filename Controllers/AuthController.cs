using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SnatchItAPI.Data;
using SnatchItAPI.Models;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SnatchItAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        private readonly IConfiguration _config;
        public AuthController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
            _config = config;
        }

        [AllowAnonymous]
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
                    if (InsertNewAuth(banderForRegistration))
                    {
                        if (InsertNewBander(banderForRegistration))
                        {
                            return Ok($"Thank you {banderForRegistration.Email}! You are now registered :)");
                        }
                        return StatusCode(500, "Server failed to create new bander");
                        
                    }
                    return StatusCode(500, "Server failed to register new user.");
                }
                return StatusCode(409, "Bander with that email already exists.");
            }
            return StatusCode(400, "Failure to register new bander, passwords do not match");
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login(BanderForLoginDto banderForLogin)
        {
            DynamicParameters parameters = new DynamicParameters();
            string sqlForHashAndSalt = @"SELECT [PasswordHash], [PasswordSalt] 
                FROM MicroAgeSchema.Auth WHERE Email = @EmailParam";
            parameters.Add("@EmailParam", banderForLogin.Email, DbType.String);

            BanderForLoginConfirmation banderForConfirmation = _dapper.LoadDataSingleWithParameters<BanderForLoginConfirmation>(sqlForHashAndSalt, parameters);

            byte[] passwordHash = GetPasswordHash(banderForLogin.Password, banderForConfirmation.PasswordSalt);

            for (int i = 0; i < passwordHash.Length; i++)
            {
                if (passwordHash[i] != banderForConfirmation.PasswordHash[i])
                {
                    return StatusCode(403, "Incorrect password. Failure to log in.");
                }
            }

            DynamicParameters banderIdSqlParameters = new DynamicParameters();
            banderIdSqlParameters.Add("@EmailParam", banderForLogin.Email, DbType.String);
            string banderIdSql = "SELECT BanderId FROM MicroAgeSchema.Banders WHERE Email = @EmailParam";

            int banderId = _dapper.LoadDataSingleWithParameters<int>(banderIdSql, banderIdSqlParameters);

            return StatusCode(200, new Dictionary<string, string>{
                        {"token", CreateToken(banderId)}
            });
        }

        
        [HttpGet("RefreshToken")]
        public string RefreshToken()
        {
            string userIdSql = @"
                SELECT BanderId FROM MicroAgeSchema.Banders WHERE BanderId = '" +
                User.FindFirst("banderId")?.Value + "'";
            
            int userId = _dapper.LoadDataSingle<int>(userIdSql);

            return CreateToken(userId);
        }
 
        // HELPER METHODS //

        private byte[] GetPasswordHash(string password, byte[] passwordSalt)
        {
            string passwordSaltPlusString = _config.GetSection("AppSettings:PasswordKey").Value +
                Convert.ToBase64String(passwordSalt);
            // string passwordSaltPlusString = Environment.GetEnvironmentVariable("PasswordKey") +
                Convert.ToBase64String(passwordSalt);

            return KeyDerivation.Pbkdf2(
                password: password,
                salt: Encoding.ASCII.GetBytes(passwordSaltPlusString),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 1000000,
                numBytesRequested: 256 / 8
            );
        }

        private bool InsertNewBander(BanderForRegistrationDto banderForRegistration)
        {
            DynamicParameters insertBanderParameters = new DynamicParameters();
            string addBanderSql = @"INSERT INTO MicroAgeSchema.Banders 
            (FirstName, LastName, Email) 
            VALUES (@FirstNameParam, @LastNameParam, @EmailParam)";

            insertBanderParameters.Add("@FirstNameParam", banderForRegistration.FirstName, DbType.String);
            insertBanderParameters.Add("@LastNameParam", banderForRegistration.LastName, DbType.String);
            insertBanderParameters.Add("@EmailParam", banderForRegistration.Email, DbType.String);

            if (_dapper.ExecuteSqlWithParameters(addBanderSql, insertBanderParameters))
            {
                return true;
            }
            return false;
        }

        private bool InsertNewAuth(BanderForRegistrationDto banderForRegistration){

            byte[] passwordSalt = new byte[128 / 8];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetNonZeroBytes(passwordSalt);
            }

            byte[] passwordHash = GetPasswordHash(banderForRegistration.Password, passwordSalt);

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
                return true;
            }

            return false;
        }

        private string CreateToken(int banderId)
        {
            Claim[] claims = new Claim[] {
                new Claim("BanderId", banderId.ToString())
            };

            string? tokenKeyString = _config.GetSection("AppSettings:TokenKey").Value;
            // string? tokenKeyString = Environment.GetEnvironmentVariable("TokenKey") ?? "Your fallback connection string here";

            SymmetricSecurityKey tokenKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(tokenKeyString != null ? tokenKeyString : "")
            );

            SigningCredentials credentials = new SigningCredentials(tokenKey, SecurityAlgorithms.HmacSha512);

            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor()
                {
                    Subject = new ClaimsIdentity(claims),
                    SigningCredentials = credentials,
                    Expires = DateTime.Now.AddDays(1)
                };
            
            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

            SecurityToken token = jwtSecurityTokenHandler.CreateToken(descriptor);

            return jwtSecurityTokenHandler.WriteToken(token);
        }

    }
}