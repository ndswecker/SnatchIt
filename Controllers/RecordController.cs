using Dapper;
using Microsoft.AspNetCore.Mvc;
using SnatchItAPI.Models;
using System.Data;
using System.IO;

namespace SnatchItAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class RecordController : ControllerBase
{

    DataContextDapper _dapper;
    public RecordController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
        Console.WriteLine(config.GetConnectionString("DefaultConnection"));
    }

    [HttpGet("TestConnection")]
    public DateTime TestConnection()
    {
        return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");
    }

    [HttpGet("getRecords")]
    public IEnumerable<CaptureRecord> GetRecords(int sheetId = 0){


        DynamicParameters sqlParameters = new DynamicParameters();

        // string sql = System.IO.File.ReadAllText("SqlQueries/GetAllCore.sql");
        string sql = 
            " SELECT * FROM MicroAgeSchema.Core AS Core ";
            
        Console.WriteLine(sql);

        if (sheetId != 0)
        {
            sql += " WHERE (@RecordId IS NULL OR Core.SheetId = @RecordId);";
            sqlParameters.Add("@RecordId", sheetId, DbType.Int32);
        }
        else
        {
            return _dapper.LoadData<CaptureRecord>(sql);
        }

        IEnumerable<CaptureRecord> records = _dapper.LoadDataWithParameters<CaptureRecord>(sql, sqlParameters);
        return records;
    }
}