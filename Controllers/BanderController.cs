using System.Data;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using SnatchItAPI.Data;
using SnatchItAPI.Models;

namespace SnatchItAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BanderController : ControllerBase
    {
        DataContextDapper _dapper;

        public BanderController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        [HttpGet("GetBanders")]
        public IEnumerable<Bander> GetBanders(int banderId)
        {
            DynamicParameters dynamicParameters = new DynamicParameters();

            string sql = "SELECT * FROM MicroAgeSchema.Banders As Banders";

            if (banderId !=0)
            {
                sql += " WHERE (@BanderIdParam IS NULL OR Banders.BanderId = @BanderIdParam)";
                dynamicParameters.Add("@BanderIdParam", banderId, DbType.Int32);
            }
            else
            {
                return _dapper.LoadData<Bander>(sql);
            }

            IEnumerable<Bander> banders = _dapper.LoadDataWithParameters<Bander>(sql, dynamicParameters);
            return banders;
        }
    }
}