CREATE PROCEDURE [dbo].[uspVote_UpdateIsSubmitted]
	@PollId INT
AS
BEGIN
	UPDATE [dbo].[Vote]
	SET [IsSubmitted] = 1
	WHERE [PollId] = @PollId;
END;