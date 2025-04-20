CREATE PROCEDURE [dbo].[uspPublishedVote_GetVotesByPoll]
	@PollId INT
AS
BEGIN
	SELECT * FROM [dbo].[PublishedVote]
	WHERE [PollId] = @PollId;
END;