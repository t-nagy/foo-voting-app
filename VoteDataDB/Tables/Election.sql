CREATE TABLE [dbo].[Election]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY,
	[OwnerName] NVARCHAR(256) NOT NULL,
	[Title] NVARCHAR(100) NOT NULL,
	[ElectionDescription] NVARCHAR(500) NULL,
	[CreatedDate] DATETIME2 NOT NULL DEFAULT getutcdate(),
	[VoteCollectionEndDate] DATETIME2 NOT NULL,
	[VoteValidationEndDate] DATETIME2 NOT NULL, 
    [IsPublic] BIT NOT NULL,
	[ElectionStatus] INT NOT NULL DEFAULT 0, 
    [ElectionCode] NVARCHAR(128) NULL
)
