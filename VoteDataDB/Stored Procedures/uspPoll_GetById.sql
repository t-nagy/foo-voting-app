CREATE PROCEDURE [dbo].[uspPoll_GetById]
	@Id int
AS
BEGIN
	SET NOCOUNT ON;

	SELECT * 
	FROM [dbo].[Poll]
	WHERE [Id] = @Id;
END;
