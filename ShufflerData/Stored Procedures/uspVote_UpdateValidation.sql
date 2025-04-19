CREATE PROCEDURE [dbo].[uspVote_UpdateValidation]
	@PollId INT,
	@EncryptedBallot NVARCHAR(1024),
	@EncryptionKey NVARCHAR(1024)
AS
BEGIN
	UPDATE [dbo].[Vote]
	SET [EncyptionKey] = @EncryptionKey
	WHERE [PollId] = @PollId AND [EncryptedBallot] = @EncryptedBallot;
END;