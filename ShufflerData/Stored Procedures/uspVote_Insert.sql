CREATE PROCEDURE [dbo].[uspVote_Insert]
	@PollId INT,
	@EncryptedBallot NVARCHAR(1024),
	@AdminSignature NVARCHAR(1024)
AS
BEGIN
	INSERT INTO [dbo].[Vote] ([PollId], [EncryptedBallot], [AdminSignature])
	VALUES (@PollId, @EncryptedBallot, @AdminSignature);
END;
