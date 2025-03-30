CREATE PROCEDURE [dbo].[uspAddElectionParticipant]
	@userId NVARCHAR(450),
	@electionId INT,
	@role INT
AS
	INSERT INTO [dbo].[ElectionAccess] (UserId, ElectionId, ElectionRole)
	VALUES (@userId, @electionId, @role);
RETURN 0
