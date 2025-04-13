CREATE PROCEDURE [dbo].[uspPollOption_Insert]
	@Id INT OUTPUT,
	@OptionText NVARCHAR(200),
	@PollId INT
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[PollOption] ([OptionText], [PollId])
	VALUES (@OptionText, @PollId)

	SELECT @Id = SCOPE_IDENTITY();
END;
