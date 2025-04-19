CREATE PROCEDURE [dbo].[uspVerification_GetByUser]
	@UserId NVARCHAR(450)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT *
	FROM dbo.Verification
	WHERE UserId = @UserId;

END;
