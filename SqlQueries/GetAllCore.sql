DECLARE @RecordId INT;

SELECT * FROM MicroAgeSchema.Core AS Core
WHERE (@RecordId IS NULL OR Core.SheetId = @RecordId);