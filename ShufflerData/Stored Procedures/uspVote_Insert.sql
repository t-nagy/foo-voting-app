CREATE PROCEDURE [dbo].[uspVote_Insert]
	@PollId INT,
	@EncryptedBallot NVARCHAR(2048),
	@AdminSignature NVARCHAR(2048)
AS
BEGIN
	INSERT INTO [dbo].[Vote] ([PollId], [EncryptedBallot], [AdminSignature])
	VALUES (@PollId, @EncryptedBallot, @AdminSignature);
END;
