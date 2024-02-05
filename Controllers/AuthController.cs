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
                    // byte[] passwordSalt = new byte[128 / 8];
                    // using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                    // {
                    //     rng.GetNonZeroBytes(passwordSalt);
                    // }

                    // byte[] passwordHash = GetPasswordHash(banderForRegistration.Password, passwordSalt);

                    // DynamicParameters insertParameters = new DynamicParameters();
                    // string insertBanderSql = @"INSERT INTO MicroAgeSchema.Auth 
                    //     ([Email], [PasswordHash], [PasswordSalt]) 
                    //     VALUES (
                    //         @EmailParam, @PasswordHashParam, @PasswordSaltParam
                    //     )";

                    // insertParameters.Add("@EmailParam", banderForRegistration.Email, DbType.String);
                    // insertParameters.Add("@PasswordHashParam", passwordHash, DbType.Binary);
                    // insertParameters.Add("@PasswordSaltParam", passwordSalt, DbType.Binary);

                    // if (_dapper.ExecuteSqlWithParameters(insertBanderSql, insertParameters))
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
                Console.WriteLine(passwordHash[i] + "-" + banderForConfirmation.PasswordHash[i]);
                if (passwordHash[i] != banderForConfirmation.PasswordHash[i])
                {
                    return StatusCode(403, "Incorrect password. Failure to log in.");
                }
            }

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

        private byte[] GetPasswordHash(string password, byte[] passwordSalt)
        {
            string passwordSaltPlusString = _config.GetSection("AppSettings:PasswordKey").Value +
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

    }
}