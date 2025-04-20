CREATE PROCEDURE [dbo].[uspPoll_GetStatusByPoll]
	@PollId INT
AS
BEGIN
	SELECT [Status] FROM [dbo].[Poll]
	WHERE Id = @PollId;
END;
