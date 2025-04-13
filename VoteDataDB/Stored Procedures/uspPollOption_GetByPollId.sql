CREATE PROCEDURE [dbo].[uspPollOption_GetByPollId]
	@PollId INT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT *
	FROM [dbo].[PollOption]
	WHERE [PollId] = @PollId;
END;
