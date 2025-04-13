CREATE PROCEDURE [dbo].[uspParticipant_GetByPoll]
	@PollId INT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT *
	FROM [dbo].[Participant]
	WHERE [PollId] = @PollId;
END;