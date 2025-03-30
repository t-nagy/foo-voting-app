CREATE PROCEDURE [dbo].[uspGetElections]
	@userId NVARCHAR(450),
	@publicEnabled BIT = 0,
	@ownerFilter NVARCHAR(256) = '',
	@titleFilter NVARCHAR(100) = '',
	@statusFilter INT = -1
AS
	IF (@statusFilter = -1)
	BEGIN
		SELECT * 
		FROM dbo.Election INNER JOIN dbo.ElectionAccess ON ElectionId = Id
		WHERE '%'+@ownerFilter+'%' LIKE OwnerName AND '%'+@titleFilter+'%' LIKE Title AND IsPublic <= @publicEnabled;
	END;
	ELSE
	BEGIN
		SELECT * 
		FROM dbo.Election INNER JOIN dbo.ElectionAccess ON ElectionId = Id
		WHERE '%'+@ownerFilter+'%' LIKE OwnerName AND '%'+@titleFilter+'%' LIKE Title AND IsPublic <= @publicEnabled AND @statusFilter = ElectionStatus;
	END;
		
RETURN 0
