CREATE PROCEDURE [dbo].[uspPoll_GetVotingEndByPoll]
	@PollId INT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [VoteCollectionEndDate] FROM [dbo].[Poll]
	WHERE Id = @PollId;
END;
