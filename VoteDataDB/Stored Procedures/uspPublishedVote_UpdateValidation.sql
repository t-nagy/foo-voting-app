CREATE PROCEDURE [dbo].[uspPublishedVote_UpdateValidation]
	@PollId INT,
	@EncryptedBallot NVARCHAR(2048),
	@EncryptionKey NVARCHAR(2048),
	@DecryptedBallot INT
AS
BEGIN
	UPDATE [dbo].[PublishedVote]
	SET [EncryptionKey] = @EncryptionKey, [DecryptedBallot] = @DecryptedBallot
	WHERE [PollId] = @PollId AND [EncryptedBallot] = @EncryptedBallot;
END;
