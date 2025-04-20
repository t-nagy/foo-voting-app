CREATE TABLE [dbo].[PublishedVote]
(
	[PollId] INT NOT NULL, 
    [EncryptedBallot] NVARCHAR(1024) NOT NULL, 
    [AdminSignature] NVARCHAR(1024) NOT NULL, 
    [EncyptionKey] NVARCHAR(1024) NULL
    CONSTRAINT [FK_PublishedVote_Poll] FOREIGN KEY (PollId) REFERENCES [Poll]([Id]),
)
