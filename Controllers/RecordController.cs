using Microsoft.AspNetCore.Mvc;

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

    [HttpGet("getRecords")]
    public string[] GetRecords(string additionalBird="Default Bird"){

        string[] records = [
            "Bird", "Birdie Bird", "Byrd's bird", additionalBird
        ];
        return records;
    }
}