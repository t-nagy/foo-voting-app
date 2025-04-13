CREATE TABLE [dbo].[Poll]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY,
	[OwnerName] NVARCHAR(256) NOT NULL,
	[Title] NVARCHAR(200) NOT NULL,
	[Description] NVARCHAR(500) NULL,
	[CreatedDate] DATETIME2 NOT NULL DEFAULT getutcdate(),
	[VoteCollectionEndDate] DATETIME2 NOT NULL,
	[VoteValidationEndDate] DATETIME2 NOT NULL, 
    [IsPublic] BIT NOT NULL,
	[Status] INT NOT NULL DEFAULT 0, 
    [JoinCode] NVARCHAR(128) NULL
)
