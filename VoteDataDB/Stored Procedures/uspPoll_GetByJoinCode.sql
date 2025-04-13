CREATE PROCEDURE [dbo].[uspPoll_GetByJoinCode]
	@JoinCode NVARCHAR(128)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT * 
	FROM [dbo].[Poll]
	WHERE [JoinCode] = @JoinCode;
END;
