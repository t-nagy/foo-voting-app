CREATE PROCEDURE [dbo].[uspParticipant_UpdateVoted]
	@UserId NVARCHAR(450),
	@PollId INT
AS
BEGIN
	SET NOCOUNT ON;

	UPDATE dbo.Participant 
	SET [HasVoted] = 1
	WHERE UserId = @UserId AND PollId = @PollId;
END;

