CREATE PROCEDURE [dbo].[uspVote_GetValidationsByPoll]
	@PollId INT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [PollId], [EncryptedBallot], [EncyptionKey] FROM [dbo].[Vote]
	WHERE PollId = @PollId;
END;
