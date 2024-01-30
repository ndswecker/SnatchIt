using Microsoft.AspNetCore.Mvc;
using SnatchItAPI.Models;
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
        string sql = System.IO.File.ReadAllText("SqlQueries/GetAllCore.sql");
        Console.WriteLine(sql);
        return _dapper.LoadData<CaptureRecord>(sql);
    }
}