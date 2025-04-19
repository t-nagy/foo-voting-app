CREATE PROCEDURE [dbo].[uspVote_DeleteByPoll]
	@PollId INT
AS
BEGIN
	DELETE FROM [dbo].[Vote]
	WHERE PollId = @PollId;
END;
