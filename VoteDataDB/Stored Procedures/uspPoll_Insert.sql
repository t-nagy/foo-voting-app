﻿CREATE PROCEDURE [dbo].[uspPoll_Insert]
	@Id INT OUTPUT,
	@CreatedDate DATETIME2 OUTPUT,
	@OwnerName NVARCHAR(256),
	@Title NVARCHAR(200),
	@Description NVARCHAR(500) = NULL,
	@VoteCollectionEndDate DATETIME2,
	@VoteValidationEndDate DATETIME2,
	@IsPublic BIT = 1,
	@JoinCode NVARCHAR(128) = NULL

AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[Poll] ([OwnerName], [Title], [Description], [VoteCollectionEndDate], [VoteValidationEndDate], [IsPublic], [JoinCode])
	VALUES (@OwnerName, @Title, @Description, @VoteCollectionEndDate, @VoteValidationEndDate, @IsPublic, @JoinCode);

	SELECT @Id = SCOPE_IDENTITY();
	SELECT @CreatedDate = GETUTCDATE();
END;
