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
    }

    [HttpGet("TestConnection")]
    public DateTime TestConnection()
    {
        return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");
    }

    [HttpGet("getRecords")]
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

    [HttpPut]
    public IActionResult EditRecord()
    {
        string sql = 
            " ";
        return Ok();
    }

    [HttpPost]
    public IActionResult AddRecord(CaptureRecordDto record)
    {
        DynamicParameters sqlParameters = new DynamicParameters();

        string sql = 
        "INSERT INTO MicroAgeSchema.CORE ([BandNumber],[BandSize],[Scribe]," +
        "[SpeciesCommon],[SpeciesAlpha],[SheetDate], [Station]," +
        "[Net],[WingChord],[Sex],[AgeYear],[AgeWRP],[BodyMass]," +
        "[Notes] ) VALUES (@BandNumberParam, @BandSizeParam, @ScribeParam, " + 
        "@SpeciesCommonParam, @SpeciesAlphaParam, @SheetDateParam, @StationParam, @NetParam, " +
        "@WingChordParam, @SexParam, @AgeYearParam, @AgeWRPParam, @BodyMassParam, @NotesParam)";

        sqlParameters.Add("@BandNumberParam", record.BandNumber, DbType.Int32);
        sqlParameters.Add("@BandSizeParam", record.BandSize, DbType.String);
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


        _dapper.ExecuteSqlWithParameters(sql, sqlParameters);
        return Ok();
    }
}