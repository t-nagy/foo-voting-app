CREATE PROCEDURE [dbo].[uspVerification_Insert]
	@UserId NVARCHAR(450),
	@Key NVARCHAR(1024)
AS
	SET NOCOUNT ON;

	INSERT INTO dbo.Verification (UserId, VerifyKey)
	VALUES (@UserId, @Key)
RETURN 0
