using Microsoft.AspNetCore.Mvc;

namespace SnatchItAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class RecordController : ControllerBase
{
    public RecordController()
    {

    }

    [HttpGet("getRecords")]
    public string[] GetRecords(){

        string[] records = [
            "Bird", "Birdie Bird", "Byrd's bird"
        ];
        return records;
    }
}