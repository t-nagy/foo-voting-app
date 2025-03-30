CREATE TABLE [dbo].[ElectionAccess]
(
	[UserId] NVARCHAR(450) NOT NULL,
	[ElectionId] INT NOT NULL,
	[ElectionRole] INT NOT NULL,
	[HasVoted] BIT NOT NULL DEFAULT 0, 
    CONSTRAINT [FK_ElectionAccess_Election] FOREIGN KEY (ElectionId) REFERENCES [Election]([Id]),
)
