CREATE PROCEDURE [dbo].[uspPublishedVote_Insert]
	@PollId INT,
	@EncryptedBallot NVARCHAR(2048)
AS
BEGIN
	INSERT INTO [dbo].[PublishedVote] ([PollId], [EncryptedBallot])
	VALUES (@PollId, @EncryptedBallot);
END;
