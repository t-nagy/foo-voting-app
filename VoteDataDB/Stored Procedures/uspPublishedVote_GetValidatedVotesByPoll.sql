CREATE PROCEDURE [dbo].[uspPublishedVote_GetValidatedVotesByPoll]
	@PollId INT
AS
BEGIN
	SELECT * FROM [dbo].[PublishedVote]
	WHERE [PollId] = @PollId AND [EncryptionKey] IS NOT NULL;
END;