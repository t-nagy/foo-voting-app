CREATE PROCEDURE [dbo].[uspPoll_GetValidationEndByPoll]
	@PollId INT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [VoteValidationEndDate] FROM [dbo].[Poll]
	WHERE Id = @PollId;
END;
