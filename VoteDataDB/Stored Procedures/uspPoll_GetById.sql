CREATE PROCEDURE [dbo].[uspGetPoll_ById]
	@userId NVARCHAR(450),
	@pollId int
AS
BEGIN
	SET NOCOUNT ON;

	SELECT * 
		FROM [dbo].[Poll] INNER JOIN [dbo].[Participant] ON [PollId] = Id
		WHERE ([UserId] = @userId OR [IsPublic] = 1) AND [Id] = @pollId;
END;
