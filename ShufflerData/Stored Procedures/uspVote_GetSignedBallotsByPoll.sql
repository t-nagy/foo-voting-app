CREATE PROCEDURE [dbo].[uspVote_GetSignedBallotsByPoll]
	@PollId INT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [PollId], [EncryptedBallot], [AdminSignature] FROM [dbo].[Vote]
	WHERE PollId = @PollId;
END;