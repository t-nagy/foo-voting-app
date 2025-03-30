CREATE PROCEDURE [dbo].[uspPoll_GetAll]
	@userId NVARCHAR(450)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT * 
	FROM [dbo].[Poll] INNER JOIN [dbo].[Participant] ON [PollId] = Id
	WHERE [UserId] = @userId OR [IsPublic] = 1;
END;