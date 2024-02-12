using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SnatchItAPI.Data;
using SnatchItAPI.Models;
using System.Data;
using System.IO;

namespace SnatchItAPI.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class RecordController : ControllerBase
{

    DataContextDapper _dapper;
    public RecordController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
    }

    // <summary>
    // Tests the connection to the database by retrieving the current server date and time.
    // </summary>
    // <returns>The current date and time from the database server.</returns>
    [AllowAnonymous]
    [HttpGet("TestConnection")]
    public IActionResult TestConnection()
    {
        try
        {
            DateTime dateTime = _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");
            return StatusCode(200, dateTime);
        }
        catch (Exception)
        {
            return StatusCode(500, "Failed to connect to database");
        }
    }

    [AllowAnonymous]
    [HttpGet("TestLive")]
    public IActionResult TestLive()
    {
        return StatusCode(200, "Site is Responsive");
    }


    // <summary>
    // Retrieves a collection of CaptureRecord objects from the database.
    // </summary>
    // <remarks>
    // This method fetches all records from the MicroAgeSchema.Core table if no specific sheetId is provided.
    // If a sheetId is specified, it filters the records to include only those associated with the given sheetId.
    // </remarks>
    // <param name="sheetId">The ID of the sheet used to filter records. If 0 or not provided, all records are retrieved.</param>
    // <returns>An IEnumerable of CaptureRecord objects, either filtered by sheetId or containing all records.</returns>
    [HttpGet("GetRecords")]
    public IEnumerable<CaptureRecord> GetRecords(int sheetId = 0){


        DynamicParameters sqlParameters = new DynamicParameters();

        string sql = 
            " SELECT * FROM MicroAgeSchema.Core AS Core ";

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
    
    
    [HttpGet("GetRecordSingle")]
    public IActionResult GetRecordSingle(int sheetId)
    {
        try
        {
            if (sheetId <= 0)
            {
                return BadRequest("Invalid SheetId.");
            }

            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@SheetIdParam", sheetId, DbType.Int32);

            string sql = "SELECT * FROM MicroAgeSchema.Core AS Core WHERE Core.SheetId = @SheetIdParam";

            CaptureRecord records = _dapper.LoadDataSingleWithParameters<CaptureRecord>(sql, sqlParameters);

            if (records == null)
            {
                return NotFound($"No records found for sheetId {sheetId}");
            }

            return Ok(records);
        }
        catch (Exception)
        {
            return StatusCode(500, $"An Error occured while processing your request for record of sheetId {sheetId}");
        }
    }


    [HttpPut("EditRecord")]
    public IActionResult EditRecord(CaptureRecord record)
    {
        string sql = 
            @"UPDATE MicroAgeSchema.Core
            SET BandSize = @BandSizeParam,
                Bander = @BanderParam,
                Scribe = @ScribeParam,
                SpeciesCommon = @SpeciesCommonParam,
                SpeciesAlpha = @SpeciesAlphaParam,
                SheetDate = @SheetDateParam,
                Station = @StationParam,
                Net = @NetParam,
                WingChord = @WingChordParam,
                Sex = @SexParam,
                AgeYear = @AgeYearParam,
                AgeWRP = @AgeWRPParam,
                BodyMass = @BodyMassParam,
                Notes = @NotesParam
            WHERE SheetId = @SheetIdParam;";

        DynamicParameters sqlParameters = new DynamicParameters();

        addCoreFields(sqlParameters, record);

        sqlParameters.Add("@SheetIdParam", record.SheetId);

        try
        {
            _dapper.ExecuteSqlWithParameters(sql, sqlParameters);
            return Ok($"Record {record.SheetId} has been successfully updated");
        }
        catch 
        {
            return NotFound($"Unable to update record {record.SheetId}");
        }
    }

    [HttpPost("AddRecord")]
    public IActionResult AddRecord(CaptureRecordDto record)
    {
        DynamicParameters sqlParameters = new DynamicParameters();

        string sql = 
            "INSERT INTO MicroAgeSchema.CORE ([BandNumber],[BandSize],[Bander],[Scribe]," +
            "[SpeciesCommon],[SpeciesAlpha],[SheetDate], [Station]," +
            "[Net],[WingChord],[Sex],[AgeYear],[AgeWRP],[BodyMass]," +
            "[Notes] ) VALUES (@BandNumberParam, @BandSizeParam, @BanderParam, @ScribeParam, " + 
            "@SpeciesCommonParam, @SpeciesAlphaParam, @SheetDateParam, @StationParam, @NetParam, " +
            "@WingChordParam, @SexParam, @AgeYearParam, @AgeWRPParam, @BodyMassParam, @NotesParam)";

        addCoreFields(sqlParameters, record);

        _dapper.ExecuteSqlWithParameters(sql, sqlParameters);
        return Ok();
    }

    [HttpDelete("DeleteRecord")]
    public IActionResult DeleteRecord(int recordId)
    {
        DynamicParameters sqlParameters = new DynamicParameters();
        sqlParameters.Add("@SheetIdParam", recordId, DbType.Int32);
        string sql = 
            "DELETE FROM MicroAgeSchema.Core WHERE SheetId = @SheetIdParam;";
        
        if (_dapper.ExecuteSqlWithParameters(sql, sqlParameters))
        {
            return Ok($"Removed record {recordId} from Core table");
        }

        return NotFound($"Failed to delete record {recordId.ToString()}, record by that id may not exist" );
    }

    private void addCoreFields(DynamicParameters sqlParameters, CaptureRecordDto record)
    {
        sqlParameters.Add("@BandNumberParam", record.BandNumber, DbType.Int32);
        sqlParameters.Add("@BandSizeParam", record.BandSize, DbType.String);
        sqlParameters.Add("@BanderParam", record.Bander, DbType.String);
        sqlParameters.Add("@ScribeParam", record.Scribe, DbType.String);
        sqlParameters.Add("@SpeciesCommonParam", record.SpeciesCommon, DbType.String);
        sqlParameters.Add("@SpeciesAlphaParam", record.SpeciesAlpha, DbType.String);
        sqlParameters.Add("@SheetDateParam", record.SheetDate, DbType.DateTime);
        sqlParameters.Add("@StationParam", record.Station, DbType.String);
        sqlParameters.Add("@NetParam", record.Net, DbType.String);
        sqlParameters.Add("@WingChordParam", record.WingChord, DbType.Int32);
        sqlParameters.Add("@SexParam", record.Sex, DbType.StringFixedLength);
        sqlParameters.Add("@AgeYearParam", record.AgeYear, DbType.String);
        sqlParameters.Add("@AgeWRPParam", record.AgeWRP, DbType.String);
        sqlParameters.Add("@BodyMassParam", record.BodyMass, DbType.Decimal);
        sqlParameters.Add("@NotesParam", record.Notes, DbType.String);
    } 

    // Helper method to create an HTTP request with JWT token
    // [ApiExplorerSettings(IgnoreApi = true)]
    // private async Task<IActionResult> TestGetRecordSingleWithJwt(int sheetId, string jwtToken)
    // {
    //     try
    //     {
    //         var requestUrl = $"{_apiBaseUrl}/[controller]/GetRecordSingle?sheetId={sheetId}";
    //         var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUrl);
    //         requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtToken);

    //         var response = await _httpClient.SendAsync(requestMessage);
    //         if (response.IsSuccessStatusCode)
    //         {
    //             var content = await response.Content.ReadAsStringAsync();
    //             return Ok(content); // You might need to deserialize the response
    //         }

    //         return StatusCode((int)response.StatusCode, "Failed to get record.");
    //     }
    //     catch (Exception ex)
    //     {
    //         return StatusCode(500, $"An error occurred: {ex.Message}");
    //     }
    // }
}