CREATE PROCEDURE [dbo].[uspParticipant_Insert]
	@UserId NVARCHAR(450),
	@PollId INT,
	@Role INT
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[Participant] ([UserId], [PollId], [ParticipantRole])
	VALUES (@UserId, @PollId, @Role);
END;
