CREATE PROCEDURE [dbo].[uspCreateElection]
	@ownerName NVARCHAR(256),
	@title NVARCHAR(100),
	@description NVARCHAR(500) = NULL,
	@voteCollectionEndDate DATETIME2,
	@voteValidationEndDate DATETIME2,
	@isPublic BIT = 1,
	@electionCode NVARCHAR(128) = NULL

AS
	INSERT INTO dbo.Election (OwnerName, Title, ElectionDescription, VoteCollectionEndDate, VoteValidationEndDate, IsPublic, ElectionCode)
	VALUES (@ownerName, @title, @description, @voteCollectionEndDate, @voteValidationEndDate, @isPublic, @electionCode);
RETURN 0
