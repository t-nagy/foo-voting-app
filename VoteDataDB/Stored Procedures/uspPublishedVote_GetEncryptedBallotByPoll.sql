CREATE PROCEDURE [dbo].[uspPublishedVote_GetEncryptedBallotByPoll]
	@PollId INT
AS
BEGIN
	SELECT [PollId], [EncryptedBallot] FROM [dbo].[PublishedVote]
	WHERE [PollId] = @PollId;
END;