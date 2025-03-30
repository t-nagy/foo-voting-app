CREATE PROCEDURE [dbo].[uspParticipant_Insert]
	@userId NVARCHAR(450),
	@pollId INT,
	@role INT
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[Participant] ([UserId], [PollId], [ParticipantRole])
	VALUES (@userId, @pollId, @role);
END;
