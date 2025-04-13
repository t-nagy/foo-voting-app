CREATE PROCEDURE [dbo].[uspParticipant_GetByPollAndUserId]
	@UserId NVARCHAR(450),
	@PollId int
AS
BEGIN
	SET NOCOUNT ON;

	SELECT *
	FROM [dbo].[Participant]
	WHERE [UserId] = @UserId AND [PollId] = @PollId;
END;