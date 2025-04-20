CREATE PROCEDURE [dbo].[uspVote_UpdateValidation]
	@PollId INT,
	@EncryptedBallot NVARCHAR(2048),
	@EncryptionKey NVARCHAR(2048)
AS
BEGIN
	UPDATE [dbo].[Vote]
	SET [EncryptionKey] = @EncryptionKey
	WHERE [PollId] = @PollId AND [EncryptedBallot] = @EncryptedBallot;
END;