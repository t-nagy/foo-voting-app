CREATE PROCEDURE [dbo].[uspPoll_GetMinimal]
	@UserId NVARCHAR(450)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [Id], [OwnerName], [Title], [Status], [IsPublic], [CreatedDate], [VoteCollectionEndDate], [VoteValidationEndDate]
	FROM [dbo].[Poll] INNER JOIN [dbo].[Participant] ON [PollId] = Id
	WHERE [UserId] = @UserId OR [IsPublic] = 1;
END;
