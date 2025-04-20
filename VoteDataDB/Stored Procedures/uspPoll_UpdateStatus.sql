CREATE PROCEDURE [dbo].[uspPoll_UpdateStatus]
	@PollId INT,
	@NewStatus INT
AS
BEGIN
	UPDATE [dbo].[Poll]
	SET [Status] = @NewStatus
	WHERE [Id] = @PollId;
END;
